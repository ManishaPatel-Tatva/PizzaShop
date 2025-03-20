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

var builder = WebApplication.CreateBuilder(args);

#region Services
/*---------------Add services to the container.-----------------------------------------------
-------------------------------------------------------------------------------------------*/

builder.Services.AddControllersWithViews();

//HttpContext
builder.Services.AddHttpContextAccessor();

//Database Connection
var conn = builder.Configuration.GetConnectionString("PizzaShopDbConnection");
builder.Services.AddDbContext<PizzaShopContext>(q => q.UseNpgsql(conn));

//Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Email Service and Setting
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

//Jwt Service
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddScoped<IJwtService, JwtService>();

//Profile Service
builder.Services.AddScoped<IProfileService,ProfileService>();

//Address Service
builder.Services.AddScoped<IAddressService, AddressService>();

//Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();

//User Service
builder.Services.AddScoped<IUserService, UserService>();

//Role and Permission Service and Repository
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

//Menu service
builder.Services.AddScoped<ICategoryItemService, CategoryItemService>();
builder.Services.AddScoped<IModifierService, ModifierService>();

//Table and Section
builder.Services.AddScoped<ITableSectionService, TableSectionService>();

//Session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensure session is only accessible via HTTP
    options.Cookie.IsEssential = true;
});

//Authentication
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

if (string.IsNullOrEmpty(jwtConfig?.Key))   // Ensure Key is Not Null or Empty
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
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]))
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

#endregion