using System.Drawing;
using System.Linq.Expressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderService : IOrderService
{
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;

    public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderStatus> orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;

    }

    public async Task<OrderIndexViewModel> GetOrderIndex()
    {
        OrderIndexViewModel model = new()
        {
            Statuses = _orderStatusRepository.GetAll().ToList()
        };
        return model;
    }


    public async Task<OrderPaginationViewModel> GetPagedOrder(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, int pageSize, int pageNumber, string search)
    {
        (IEnumerable<Order> orders, int totalRecord) = await _orderRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Order, object>>>
            {
                o => o.Customer,
                o => o.PaymentMethod,
                o => o.Status,
                o => o.CustomersReviews
            }
        );

        //For applying status filter
        if (!string.IsNullOrEmpty(status) && status.ToLower() != "all status")
        {
            orders = orders.Where(o => o.Status.Name.ToLower() == status.ToLower());
        }

        //For applying date range filter
        if (!string.IsNullOrEmpty(dateRange) && dateRange.ToLower() != "all time" && !fromDate.HasValue && !toDate.HasValue)
        {
            switch (dateRange.ToLower())
            {
                case "last 7 days":
                    orders = orders.Where(o => o.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && o.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    orders = orders.Where(o => o.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && o.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    orders = orders.Where(x => x.Date.Month == startDate.Month && x.Date.Year == startDate.Year);
                    break;
                default:
                    break;
            }
        }

        //Filtering Custom Dates
        if(fromDate.HasValue)
            orders = orders.Where(o => o.Date >= fromDate.Value);
        if(toDate.HasValue)
            orders = orders.Where(o => o.Date <= toDate.Value);

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(column))
        {
            switch (column)
            {
                case "order":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Id) : orders.OrderByDescending(o => o.Id);
                    break;
                case "date":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Date) : orders.OrderByDescending(o => o.Date);
                    break;
                case "customer":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Customer.Name) : orders.OrderByDescending(o => o.Customer.Name);
                    break;
                case "amount":
                    orders = sort == "asc" ? orders.OrderBy(o => o.TotalAmount) : orders.OrderByDescending(o => o.TotalAmount);
                    break;
                default:
                    break;
            }
        }

        //Setting the filtered and sorted values in View Model
        OrderPaginationViewModel model = new()
        {
            Page = new(),
            Orders = orders.Select(o => new OrderViewModel()
            {
                OrderId = o.Id,
                Date = o.Date,
                CustomerName = o.Customer.Name,
                Status = o.Status.Name,
                PaymentMode = o.PaymentMethod.Name,
                Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) : 0),
                TotalAmount = o.TotalAmount
            })
        };

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    public async Task<byte[]> ExportOrderDetails(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search)
    {
        IEnumerable<Order> orders = await _orderRepository.GetRecordDetails(
            filter: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Order, object>>>
            {
                o => o.Customer,
                o => o.PaymentMethod,
                o => o.Status,
                o => o.CustomersReviews
            }
        );

        //For applying status filter
        if (!string.IsNullOrEmpty(status) && status.ToLower() != "all status")
        {
            orders = orders.Where(o => o.Status.Name.ToLower() == status.ToLower());
        }

        //For applying date range filter
        if (!string.IsNullOrEmpty(dateRange) && dateRange.ToLower() != "all time" && !fromDate.HasValue && !toDate.HasValue)
        {
            switch (dateRange.ToLower())
            {
                case "last 7 days":
                    orders = orders.Where(o => o.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && o.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    orders = orders.Where(o => o.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && o.Date <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    orders = orders.Where(x => x.Date.Month == startDate.Month && x.Date.Year == startDate.Year);
                    break;
                default:
                    break;
            }
        }

        //Filtering Custom Dates
        if(fromDate.HasValue)
            orders = orders.Where(o => o.Date >= fromDate.Value);
        if(toDate.HasValue)
            orders = orders.Where(o => o.Date <= toDate.Value);

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(column))
        {
            switch (column)
            {
                case "order":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Id) : orders.OrderByDescending(o => o.Id);
                    break;
                case "date":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Date) : orders.OrderByDescending(o => o.Date);
                    break;
                case "customer":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Customer.Name) : orders.OrderByDescending(o => o.Customer.Name);
                    break;
                case "amount":
                    orders = sort == "asc" ? orders.OrderBy(o => o.TotalAmount) : orders.OrderByDescending(o => o.TotalAmount);
                    break;
                default:
                    break;
            }
        }

        //Setting the filtered and sorted values in View Model
        OrderPaginationViewModel model = new()
        {
            Page = new(),
            Orders = orders.Select(o => new OrderViewModel()
            {
                OrderId = o.Id,
                Date = o.Date,
                CustomerName = o.Customer.Name,
                Status = o.Status.Name,
                PaymentMode = o.PaymentMethod.Name,
                Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) : 0),
                TotalAmount = o.TotalAmount
            })
        };

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();

        var worksheet = package.Workbook.Worksheets.Add("Orders");
        var currentRow = 3;
        var currentCol = 2;

        // this is first row....................................
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = "Status: ";
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
            headingCells.Style.Font.Bold = true;
            headingCells.Style.Font.Color.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
        currentCol += 2;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = status;
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 5;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = "Search Text: ";
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
            headingCells.Style.Font.Bold = true;
            headingCells.Style.Font.Color.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 2;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = search;
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 5;

        worksheet.Cells[currentRow, currentCol, currentRow + 4, currentCol + 1].Merge = true;

        // Insert Logo
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");

        if (File.Exists(imagePath))
        {
            var picture = worksheet.Drawings.AddPicture("Image", new FileInfo(imagePath));
            picture.SetPosition(currentRow - 1, 1, currentCol - 1, 1);
            picture.SetSize(125, 95);
        }
        else
        {
            worksheet.Cells[currentRow, currentCol].Value = "Image not found";
        }

        // this is second row....................................
        currentRow += 3;
        currentCol = 2;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = "Date: ";
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
            headingCells.Style.Font.Bold = true;
            headingCells.Style.Font.Color.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 2;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = dateRange;
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 5;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
            headingCells.Style.Font.Bold = true;
            headingCells.Style.Font.Color.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        currentCol += 2;
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = orders.Count();
        using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }



        // this is table ....................................
        int headingRow = currentRow + 4;
        int headingCol = 2;

        worksheet.Cells[headingRow, headingCol].Value = "Order No";
        headingCol++;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Order Date";
        headingCol += 3;  // Move to next unmerged column

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Customer Name";
        headingCol += 3;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Status";
        headingCol += 3;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Payment Mode";
        headingCol += 2;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Average Rating";
        headingCol += 2;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Total Amount";


        using (var headingCells = worksheet.Cells[headingRow, 2, headingRow, headingCol + 1])
        {
            headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
            headingCells.Style.Font.Bold = true;
            headingCells.Style.Font.Color.SetColor(Color.White);

            headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }


        // Populate data
        int row = headingRow + 1;
        foreach (Order order in orders)
        {
            int startCol = 2;

            worksheet.Cells[row, startCol].Value = order.Id;
            startCol += 1;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Date;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Customer.Name;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Status.Name;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = order.PaymentMethod.Name;
            startCol += 2;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = (int)(order.CustomersReviews.Any() ? order.CustomersReviews.Average(r => r.Rating) : 0);
            startCol += 2;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = order.TotalAmount;

            using (var rowCells = worksheet.Cells[row, 2, row, startCol + 1])
            {
                // Apply alternating row colors (light gray for better readability)
                if (row % 2 == 0)
                {
                    rowCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rowCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Apply black borders to each row
                rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                rowCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            row++;
        }
        return await Task.FromResult(package.GetAsByteArray());
    }
    
}