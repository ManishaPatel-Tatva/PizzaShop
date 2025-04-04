using System.Drawing;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CustomerService : ICustomerService
{
    private readonly IGenericRepository<Customer> _customerRepository;

    public CustomerService(IGenericRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    #region Order Pagination
    /*----------------------------------------------------Order Pagination----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<CustomerPaginationViewModel> GetPagedCustomers(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, int pageSize, int pageNumber, string search)
    {
        (IEnumerable<Customer> customers, int totalRecord) = await _customerRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            predicate: c => !c.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    c.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.Orders
            }
        );

        CustomerPaginationViewModel model = new()
        {
            Customers = customers.Select(c => new CustomerViewModel()
            {
                CustomerId = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Date = DateOnly.FromDateTime(c.Orders.Where(o => o.CustomerId == c.Id).Select(o => o.CreatedAt).LastOrDefault()),
                TotalOrder = c.Orders.Where(o => o.CustomerId == c.Id).Count()
            }),
            Page = new()
        };

        //For applying date range filter
        if (!string.IsNullOrEmpty(dateRange) && dateRange.ToLower() != "all time")
        {
            switch (dateRange.ToLower())
            {
                case "today":
                    DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                    model.Customers = model.Customers.Where(c => c.Date.Day == today.Day && c.Date.Month == today.Month && c.Date.Year == today.Year);
                    break;
                case "last 7 days":
                    model.Customers = model.Customers.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    model.Customers = model.Customers.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    model.Customers = model.Customers.Where(x => x.Date.Month == startDate.Month && x.Date.Year == startDate.Year);
                    break;
                case "custom date":
                    //Filtering Custom Dates
                    if (fromDate.HasValue)
                        model.Customers = model.Customers.Where(c => c.Date >= fromDate.Value);
                    if (toDate.HasValue)
                        model.Customers = model.Customers.Where(c => c.Date <= toDate.Value);
                    break;
                default:
                    break;
            }
        }

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(column))
        {
            switch (column)
            {
                case "name":
                    model.Customers = sort == "asc" ? model.Customers.OrderBy(c => c.Name) : model.Customers.OrderByDescending(c => c.Name);
                    break;
                case "date":
                    model.Customers = sort == "asc" ? model.Customers.OrderBy(c => c.Date) : model.Customers.OrderByDescending(c => c.Date);
                    break;
                case "total order":
                    model.Customers = sort == "asc" ? model.Customers.OrderBy(c => c.TotalOrder) : model.Customers.OrderByDescending(c => c.TotalOrder);
                    break;
                default:
                    break;
            }
        }

        totalRecord = model.Customers.Count();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }
    #endregion

    #region Customer History

    public async Task<CustomerHistoryViewModel> CustomerHistory(long customerId)
    {
        IEnumerable<Customer>? customer = _customerRepository.GetByCondition(
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
        ).Result;

        if (customer == null)
            return null;

        CustomerHistoryViewModel? model = customer.Select(c => new CustomerHistoryViewModel{
            CustomerId = customerId,
            CustomerName = c.Name,
            Phone = c.Phone,
            MaxOrder = c.Orders.Where(o => o.CustomerId == customerId).Max(o => o.FinalAmount),
            AvgBill = c.Orders.Where(o => o.CustomerId == customerId).Average(o => o.FinalAmount),
            ComingSince = c.CreatedAt,
            Visits = c.Orders.Where(o => o.CustomerId == customerId).Count(),
            Orders = c.Orders.Where(o => o.CustomerId == customerId)
                    .Select(o => new OrderViewModel{
                        Date = DateOnly.FromDateTime(o.CreatedAt),
                        IsDineIn = o.IsDineIn,
                        PaymentMode = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).SingleOrDefault(),
                        NoOfItems = o.OrderItems.Where(oi => oi.OrderId == o.Id).Count(),
                        TotalAmount = o.FinalAmount
            }).ToList()
        }).FirstOrDefault();
        
        return model;
    }

    #endregion

    #region Export Excel
        /*----------------------------------------------------Export Order List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<byte[]> ExportExcel(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search)
    {
        IEnumerable<Customer> customers = await _customerRepository.GetByCondition(
            predicate: c => !c.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    c.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.Orders
            }
        );

        IEnumerable<CustomerViewModel> customersList = customers.Select(c => new CustomerViewModel()
            {
                CustomerId = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Date = DateOnly.FromDateTime(c.Orders.Where(o => o.CustomerId == c.Id).Select(o => o.CreatedAt).LastOrDefault()),
                TotalOrder = c.Orders.Where(o => o.CustomerId == c.Id).Count()
            });
        

        //For applying date range filter
        if (!string.IsNullOrEmpty(dateRange) && dateRange.ToLower() != "all time")
        {
            switch (dateRange.ToLower())
            {
                case "today":
                    DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                    customersList = customersList.Where(c => c.Date.Day == today.Day && c.Date.Month == today.Month && c.Date.Year == today.Year);
                    break;
                case "last 7 days":
                    customersList = customersList.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    customersList = customersList.Where(c => c.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && c.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    customersList = customersList.Where(x => x.Date.Month == startDate.Month && x.Date.Year == startDate.Year);
                    break;
                case "custom date":
                    //Filtering Custom Dates
                    if (fromDate.HasValue)
                        customersList = customersList.Where(c => c.Date >= fromDate.Value);
                    if (toDate.HasValue)
                        customersList = customersList.Where(c => c.Date <= toDate.Value);
                    break;
                default:
                    break;
            }
        }

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(column))
        {
            switch (column)
            {
                case "name":
                    customersList = sort == "asc" ? customersList.OrderBy(c => c.Name) : customersList.OrderByDescending(c => c.Name);
                    break;
                case "date":
                    customersList = sort == "asc" ? customersList.OrderBy(c => c.Date) : customersList.OrderByDescending(c => c.Date);
                    break;
                case "total order":
                    customersList = sort == "asc" ? customersList.OrderBy(c => c.TotalOrder) : customersList.OrderByDescending(c => c.TotalOrder);
                    break;
                default:
                    break;
            }
        }

        return ExcelTemplateHelper.Customers(customersList,  dateRange, search);

    }


    #endregion

}
