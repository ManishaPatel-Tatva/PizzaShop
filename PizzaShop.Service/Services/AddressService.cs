using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AddressService : IAddressService
{
    private readonly IGenericRepository<City> _cityRepository;
    private readonly IGenericRepository<State> _stateRepository;
    private readonly IGenericRepository<Country> _countryRepository;

    public AddressService(IGenericRepository<City> cityRepository, IGenericRepository<State> stateRepository, IGenericRepository<Country> countryRepository)
    {
        _cityRepository = cityRepository;
        _stateRepository = stateRepository;
        _countryRepository = countryRepository;
    }

    public List<City> GetCities(long stateId)
    {
        return _cityRepository.GetByCondition(c => c.StateId == stateId).ToList();
    }

    public List<State> GetStates(long countryId)
    {
        return _stateRepository.GetByCondition(s => s.CountryId == countryId).ToList();
    }

    public List<Country> GetCountries()
    {
        return _countryRepository.GetAll().ToList();
    }

    

}
