using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IWaitingListService
{
    Task<List<WaitingTokenViewModel>> List(long sectionId);
    Task<ResponseViewModel> Save(WaitingTokenViewModel wtokenVM);
}
