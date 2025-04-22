using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAppTableService
{
    Task<List<SectionViewModel>> Get();
    Task<AssignTableViewModel> Get(long tokenId);
    Task<ResponseViewModel> Add(AssignTableViewModel assignTableVM);
    Task<ResponseViewModel> AssignTable(AssignTableViewModel assignTableVM);
}
