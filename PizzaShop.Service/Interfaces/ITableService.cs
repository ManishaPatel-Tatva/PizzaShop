using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableService
{
    Task<TablesPaginationViewModel> Get(long sectionId, FilterViewModel filter);
    Task<TableViewModel> Get(long tableId);
    Task<ResponseViewModel> Save(TableViewModel model);
    Task<bool> AssignTable(long tableId);
    Task<ResponseViewModel> Delete(long tableId);
    Task<ResponseViewModel> Delete(List<long> tableIdList);
}
