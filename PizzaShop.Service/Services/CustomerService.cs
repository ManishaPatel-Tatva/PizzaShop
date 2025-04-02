using System.Linq.Expressions;
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
            filter: c => !c.IsDeleted &&
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

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }
    #endregion

    #region 

    public async Task<CustomerHistoryViewModel> CustomerHistory(long customerId)
    {
        IEnumerable<Customer>? customer =  _customerRepository.GetByConditionInclude(
            c => c.Id == customerId && !c.IsDeleted,
            includes: new List<Expression<Func<Customer, object>>>
            {
                c => c.Orders
            }
        ).Result;

        if (customer == null)
            return null;

        CustomerHistoryViewModel model = customer.Select(c => new CustomerHistoryViewModel{
            CustomerId = customerId,
            Phone = c.Phone,
            MaxOrder = c.Orders.Where(o => o.CustomerId == customerId).Max(o => o.TotalAmount),
            AvgBill = c.Orders.Where(o => o.CustomerId == customerId).Average(o => o.TotalAmount),
            ComingSince = c.CreatedAt,
            Visits = c.Orders.Where(o => o.CustomerId == customerId).Count()
        }).First();
        

        return null;


    }

    #endregion

}
