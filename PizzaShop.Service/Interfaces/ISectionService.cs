using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ISectionService
{
    Task<List<SectionViewModel>> Get();
    Task<SectionViewModel> Get(long sectionId);
    Task<ResponseViewModel> Save(SectionViewModel model);
    Task Delete(long sectionId);
}
