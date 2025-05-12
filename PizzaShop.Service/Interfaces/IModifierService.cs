using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierService
{
    Task<ModifierViewModel> Get(long modifierId);
    Task<ModifiersPaginationViewModel> Get(int pageSize, int pageNumber, string search);
    Task<ModifiersPaginationViewModel> Get(long modifierGroupId, int pageSize, int pageNumber, string search);
    Task<ResponseViewModel> Save(ModifierViewModel model);
    Task Delete(long modifierGroupId, List<long> modifierIdList);
}
