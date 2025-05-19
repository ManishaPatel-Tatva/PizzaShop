using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Repositories;
using PizzaShop.Service.Configuration;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Services;
using PizzaShop.Web.Middleware;
using Rotativa.AspNetCore;
using Serilog;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Serilog
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "myapp-log.txt");
 
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: logFilePath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true,
        shared: true,
        outputTemplate: "[{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}"
    )
    .CreateLogger();
 
// Replace built-in logger with Serilog
// builder.Host.UseSerilog();
builder.Logging.AddSerilog(Log.Logger);

#region Services
/*---------------Add services to the container.-----------------------------------------------
-------------------------------------------------------------------------------------------*/
builder.Services.AddControllersWithViews();

//HttpContext
builder.Services.AddHttpContextAccessor();

//Database Connection
string? conn = builder.Configuration.GetConnectionString("PizzaShopDbConnection");
builder.Services.AddDbContext<PizzaShopContext>(q => q.UseNpgsql(conn));

//Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IKotRepository, KotRepository>();

//Email Service and Setting
EmailConfig.LoadEmailConfiguration(builder.Configuration);
builder.Services.AddScoped<IEmailService, EmailService>();

//Jwt Service
JwtConfig.LoadJwtConfiguration(builder.Configuration);
builder.Services.AddScoped<IJwtService, JwtService>();

//Profile Service
builder.Services.AddScoped<IProfileService,ProfileService>();
builder.Services.AddScoped<IAddressService, AddressService>();

//Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Dashboard Service
builder.Services.AddScoped<IDashboardService, DashboardService>();

//User Service
builder.Services.AddScoped<IUserService, UserService>();

//Role and Permission Service
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

//Menu service
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IItemModifierService, ItemModifierService>();
builder.Services.AddScoped<IModifierService, ModifierService>();
builder.Services.AddScoped<IModifierGroupService, ModifierGroupService>();
builder.Services.AddScoped<IModifierMappingService, ModifierMappingService>();

//Table and Section
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<ISectionService, SectionService>();

// Taxes and Fees
builder.Services.AddScoped<ITaxesFeesService, TaxesFeesService>();

//Orders
builder.Services.AddScoped<IOrderService, OrderService>();

//Customers
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerReviewService, CustomerReviewService>();

//OrderApp - KOT, Tables, Waiting List, Menu
builder.Services.AddScoped<IKotService, KotService>();
builder.Services.AddScoped<IAppTableService, AppTableService>();
builder.Services.AddScoped<IWaitingListService, WaitingListService>();
builder.Services.AddScoped<IAppMenuService, AppMenuService>();

//Invoice, Payment, Order Mapping with - table, tax & item
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderTableService, OrderTableService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderItemModifierService, OrderItemModifierService>();
builder.Services.AddScoped<IOrderTaxService, OrderTaxService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();

//Event
builder.Services.AddScoped<IEventService, EventService>();

//Session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensure session is only accessible via HTTP
    options.Cookie.IsEssential = true;
});

//Authentication
if (string.IsNullOrEmpty(JwtConfig.Key))   // Ensure Key is Not Null or Empty
{
    throw new InvalidOperationException("JWT Secret Key is missing in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Extract token from the "JwtToken" cookie
                var token = context.Request.Cookies["authToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtConfig.Issuer,
            ValidAudience = JwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Key))
        };
    });



// Add Authorization Middleware
builder.Services.AddAuthorization();

#endregion Services

#region Build
/*-------------------Build and Run---------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Add("Pragma", "no-cache");
    context.Response.Headers.Add("Expires", "0");

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseStatusCodePagesWithReExecute("/Auth/Error/{0}");

app.UseAuthentication();
app.UseMiddleware<RolePermissionMiddleware>();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Rotativa
app.UseRotativa();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

#endregion