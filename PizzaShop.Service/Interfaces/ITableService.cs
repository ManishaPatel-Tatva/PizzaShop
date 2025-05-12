using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableService
{
    Task<TablesPaginationViewModel> Get(long sectionId, FilterViewModel filter);
    Task<TableViewModel> Get(long tableId);
    Task<List<TableViewModel>> List(long sectionId);
    Task<ResponseViewModel> Save(TableViewModel model);
    Task ChangeStatus(long tableId, string status);
    Task Delete(long tableId);
    Task Delete(List<long> tableIdList);
}
