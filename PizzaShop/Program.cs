using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataAccessLayer.Models;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.Services;

var builder = WebApplication.CreateBuilder(args);

//For adding reposirory
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ICountryRepository,CountryRepository>();

//For adding services of business logic layer
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddScoped<ICountryService,CountryService>();
builder.Services.AddScoped<IUserService,UserService>();


// Add services to the container.
var conn = builder.Configuration.GetConnectionString("PizzashopDbConnection");
builder.Services.AddDbContext<PizzaShopContext>(q => q.UseNpgsql(conn));
builder.Services.AddControllersWithViews();
builder.Services.Configure<EmailSettings>
(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
