using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;
public class ModifierService : IModifierService
{
    private readonly IGenericRepository<ModifierGroup> _modifierGroup;

    public ModifierService(IGenericRepository<ModifierGroup> modifierGroup)
    {
        _modifierGroup = modifierGroup;
    }

    #region Read Modifier Groups
    /*-----------------------------------------------------------Read Modifier Groups---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public List<ModifierGroupViewModel> GetModifierGroups()
    {
        List<ModifierGroupViewModel> modifierGroups = _modifierGroup.GetByCondition(mg => mg.IsDeleted == false)
        .Select(mg => new ModifierGroupViewModel
        {
            ModifierGroupId = mg.Id,
            Name = mg.Name,
            Description = mg.Description
        }).ToList();

        return modifierGroups;
    }
    #endregion Read Modifier Groups


}
