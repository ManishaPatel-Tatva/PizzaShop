using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly PizzaShopContext _context;


    public CountryRepository(PizzaShopContext context)
    {
        _context = context;

    }
    public List<City> GetCities(long stateId)
    {
        return _context.Cities.Where(c => c.StateId == stateId).ToList();
    }

    public List<State> GetStates(long countryId)
    {
         return _context.States.Where(s => s.CountryId == countryId).ToList();
    }

    public List<Country> GetCountries()
    {
        return _context.Countries.ToList();
    }

    
}

