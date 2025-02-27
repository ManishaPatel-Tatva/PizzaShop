using DataAccessLayer.Models;

namespace BusinessLogicLayer.Interfaces;

public interface ICountryService
{
    public List<Country> GetCountries();
    public List<State> GetStates(long countryId);
    public List<City> GetCities(long stateId);
}
