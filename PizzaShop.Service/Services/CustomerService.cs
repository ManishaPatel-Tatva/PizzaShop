using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CustomerService : ICustomerService
{
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IUserService _userService;
    public CustomerService(IGenericRepository<Customer> customerRepository, IUserService userService)
    {
        _customerRepository = customerRepository;
        _userService = userService;
    }

    #region Get
    /*----------------------------------------------------Get Customer by Email----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<CustomerViewModel> Get(string email)
    {
        Customer customer = await _customerRepository.GetByStringAsync(c => c.Email == email && !c.IsDeleted)
                            ?? new Customer();

        return Get(customer.Id);
    }

    /*----------------------------------------------------Get Customer by Id----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public CustomerViewModel Get(long id)
    {
        Customer customer = _customerRepository.GetByCondition(
            predicate: c => c.Id == id,
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.WaitingTokens
            }
        ).Result.FirstOrDefault()
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Customer"));

        CustomerViewModel customerVM = new()
        {
            Id = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Members = customer.WaitingTokens
               .Where(t => t.CustomerId == id)
               .Select(t => t.Members)
               .LastOrDefault()
        };

        return customerVM;
    }

    public async Task<CustomerPaginationViewModel> Get(FilterViewModel filter)
    {
        List<CustomerViewModel> customers = await List(filter);

        //Pagination
        int totalRecord = customers.Count;
        customers = customers
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        CustomerPaginationViewModel customersVM = new()
        {
            Customers = customers,
        };

        customersVM.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return customersVM;
    }
    #endregion

    #region List
    /*----------------------------------------------------Customer Pagination----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<List<CustomerViewModel>> List(FilterViewModel filter)
    {
        IEnumerable<Customer> list = await _customerRepository.GetByCondition(
            predicate: c => !c.IsDeleted &&
                    (string.IsNullOrEmpty(filter.Search) ||
                    c.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.Orders
            }
        );

        IEnumerable<CustomerViewModel>? customers = list.Select(c => new CustomerViewModel()
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Date = DateOnly.FromDateTime(c.CreatedAt),
            TotalOrder = c.Orders.Where(o => o.CustomerId == c.Id).Count()
        });


        //For applying date range filter
        if (!string.IsNullOrEmpty(filter.DateRange) && filter.DateRange.ToLower() != "all time")
        {
            switch (filter.DateRange.ToLower())
            {
                case "today":
                    DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                    customers = customers.Where(c => c.Date.Day == today.Day && c.Date.Month == today.Month && c.Date.Year == today.Year);
                    break;
                case "last 7 days":
                    customers = customers.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    customers = customers.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    customers = customers.Where(x => x.Date.Month == startDate.Month && x.Date.Year == startDate.Year);
                    break;
                case "custom date":
                    //Filtering Custom Dates
                    if (filter.FromDate.HasValue)
                        customers = customers.Where(c => c.Date >= filter.FromDate.Value);
                    if (filter.ToDate.HasValue)
                        customers = customers.Where(c => c.Date <= filter.ToDate.Value);
                    break;
                default:
                    break;
            }
        }

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(filter.Column))
        {
            switch (filter.Column)
            {
                case "name":
                    customers = filter.Sort == "asc" ? customers.OrderBy(c => c.Name) : customers.OrderByDescending(c => c.Name);
                    break;
                case "date":
                    customers = filter.Sort == "asc" ? customers.OrderBy(c => c.Date) : customers.OrderByDescending(c => c.Date);
                    break;
                case "total order":
                    customers = filter.Sort == "asc" ? customers.OrderBy(c => c.TotalOrder) : customers.OrderByDescending(c => c.TotalOrder);
                    break;
                default:
                    break;
            }
        }

        return customers.ToList();
    }
    #endregion

    #region Customer History

    public CustomerHistoryViewModel GetHistory(long customerId)
    {
        Customer customer = _customerRepository.GetByCondition(
            c => c.Id == customerId && !c.IsDeleted,
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.Orders
            },
            thenIncludes: new List<Func<IQueryable<Customer>, IQueryable<Customer>>>
            {
                q => q.Include(c => c.Orders)
                    .ThenInclude(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod),
                q => q.Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
            }
        ).Result.FirstOrDefault()
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Customer"));

        CustomerHistoryViewModel customerVM = new()
        {
            CustomerId = customerId,
            CustomerName = customer.Name,
            Phone = customer.Phone,
            MaxOrder = customer.Orders.Where(o => o.CustomerId == customerId).Max(o => o.FinalAmount),
            AvgBill = customer.Orders.Where(o => o.CustomerId == customerId).Average(o => o.FinalAmount),
            ComingSince = customer.CreatedAt,
            Visits = customer.Orders.Where(o => o.CustomerId == customerId).Count(),
            Orders = customer.Orders.Where(o => o.CustomerId == customerId)
                    .Select(o => new OrderViewModel
                    {
                        Date = DateOnly.FromDateTime(o.CreatedAt),
                        IsDineIn = o.IsDineIn,
                        PaymentMode = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).SingleOrDefault(),
                        NoOfItems = o.OrderItems.Where(oi => oi.OrderId == o.Id).Count(),
                        TotalAmount = o.FinalAmount
                    }).ToList()
        };

        return customerVM;
    }

    #endregion

    #region Export Excel
    public async Task<byte[]> ExportExcel(FilterViewModel filter)
    {
        return ExcelTemplateHelper.Customers(await List(filter), filter.DateRange, filter.Search);
    }
    #endregion

    #region Save

    public async Task<ResponseViewModel> Save(CustomerViewModel customerVM)
    {
        Customer customer = await _customerRepository.GetByIdAsync(customerVM.Id)
                            ?? new Customer()
                            {
                                CreatedBy = await _userService.LoggedInUser()
                            };
        ResponseViewModel response = new();

        customer.Name = customerVM.Name;
        customer.Email = customerVM.Email.ToLower();
        customer.Phone = customerVM.Phone;
        customer.UpdatedBy = await _userService.LoggedInUser();
        customer.UpdatedAt = DateTime.Now;

        response.EntityId = customerVM.Id;

        if (customerVM.Id == 0)
        {
            response.EntityId = await _customerRepository.AddAsyncReturnId(customer);
            response.Success = response.EntityId > 0;
            response.Message = response.EntityId > 0 ? NotificationMessages.Added.Replace("{0}", "Customer") : NotificationMessages.AddedFailed.Replace("{0}", "Customer");
        }
        else
        {
            await _customerRepository.UpdateAsync(customer);
            response.Message = NotificationMessages.Updated.Replace("{0}", "Customer");
        }

        return response;
    }

    #endregion

}
