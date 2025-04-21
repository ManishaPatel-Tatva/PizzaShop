using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IWaitingListService
{
    Task<List<SectionViewModel>> Get();
    Task<WaitingTokenViewModel> Get(long tokenId);
    Task<List<WaitingTokenViewModel>> List(long sectionId);
    Task<ResponseViewModel> Save(WaitingTokenViewModel wtokenVM);
    Task<bool> AssignTable(long tokenId);
    Task<ResponseViewModel> Delete(long tokenId);
}
