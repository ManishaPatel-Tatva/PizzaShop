using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAppTableService
{
    Task<List<SectionViewModel>> Get();
    Task<ResponseViewModel> Add(AssignTableViewModel assignTableVM);
}
