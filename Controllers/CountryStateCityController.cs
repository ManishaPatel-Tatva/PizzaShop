using Microsoft.AspNetCore.Mvc;
using PizzaShop.Models;
using PizzaShop.Services;

public class CountryStateCityController : Controller
{
    private readonly ILogger<CountryStateCityController> _logger;
    private readonly PizzashopContext _context;

    public CountryStateCityController(ILogger<CountryStateCityController> logger, PizzashopContext context)
    {
        _logger = logger;
        _context = context;
    }

    
}