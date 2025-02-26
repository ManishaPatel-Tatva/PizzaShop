using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;


    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;

    }
    public List<City> GetCities(long stateId)
    {
        return _countryRepository.GetCities(stateId);
    }

    public List<Country> GetCountries()
    {
        return _countryRepository.GetCountries();
    }

    public List<State> GetStates(long countryId)
    {
        return _countryRepository.GetStates(countryId);
    }

}
