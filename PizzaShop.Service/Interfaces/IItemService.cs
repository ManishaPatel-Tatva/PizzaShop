using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IItemService
{

    // Task<ItemsPaginationViewModel> GetPagedItems(long categoryId, int pageSize, int pageNumber, string search);
    Task<ItemsPaginationViewModel> Get(long categoryId, FilterViewModel filter);
    Task<ItemViewModel> Get(long itemId);
    Task<ResponseViewModel> Save(ItemViewModel itemVM);
    Task Delete(long id);
    Task Delete(List<long> itemsList);
}
