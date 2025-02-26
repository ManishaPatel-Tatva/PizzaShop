using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces;

public interface ICountryRepository
{
    public List<Country> GetCountries();
    public List<State> GetStates(long countryId);
    public List<City> GetCities(long stateId);
    
}
