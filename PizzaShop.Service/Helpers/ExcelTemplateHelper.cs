using System.ComponentModel;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace PizzaShop.Service.Helpers;

public class ExcelTemplateHelper
{
    #region Orders
    public static byte[] Orders(List<OrderViewModel> orders,string status, string dateRange, string search)
    {
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
        foreach (OrderViewModel order in orders)
        {
            int startCol = 2;

            worksheet.Cells[row, startCol].Value = order.OrderId;
            startCol += 1;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Date;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.CustomerName;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Status;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = order.PaymentMode;
            startCol += 2;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = order.Rating;
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
        return package.GetAsByteArray();

    }
    #endregion Orders

    #region  Customers
    public static byte[] Customers(IEnumerable<CustomerViewModel> customersList, string dateRange, string search)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();

        var worksheet = package.Workbook.Worksheets.Add("Orders");
        var currentRow = 3;
        var currentCol = 2;

        // this is first row....................................
        worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
        worksheet.Cells[currentRow, currentCol].Value = "Account: ";
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
        worksheet.Cells[currentRow, currentCol].Value = "";
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
        worksheet.Cells[currentRow, currentCol].Value = customersList.Count();
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

        worksheet.Cells[headingRow, headingCol].Value = "Id";
        headingCol++;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Name";
        headingCol += 3;  // Move to next unmerged column

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Email";
        headingCol += 3;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Date";
        headingCol += 3;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Mobile Number";
        headingCol += 2;

        worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
        worksheet.Cells[headingRow, headingCol].Value = "Total Order";
        headingCol += 2;

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
        foreach (CustomerViewModel c in customersList)
        {
            int startCol = 2;

            worksheet.Cells[row, startCol].Value = c.CustomerId;
            startCol += 1;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = c.Name;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = c.Email;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
            worksheet.Cells[row, startCol].Value = c.Date;
            startCol += 3;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = c.Phone;
            startCol += 2;

            worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
            worksheet.Cells[row, startCol].Value = c.TotalOrder;
            startCol += 2;

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
        return package.GetAsByteArray();
    }

    #endregion Customers
}
