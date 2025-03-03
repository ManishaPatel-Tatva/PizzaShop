using PizzaShop.Entity.Models;

namespace PizzaShop.Service.Interfaces;

public interface IAddressService
{
    List<Country> GetCountries();
    List<State> GetStates(long countryId);
    List<City> GetCities(long stateId);
}
