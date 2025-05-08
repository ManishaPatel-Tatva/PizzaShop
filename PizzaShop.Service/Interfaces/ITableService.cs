using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableService
{
    Task<TablesPaginationViewModel> Get(long sectionId, FilterViewModel filter);
    Task<TableViewModel> Get(long tableId);
    Task<List<TableViewModel>> List(long sectionId);
    Task<ResponseViewModel> Save(TableViewModel model);
    Task<bool> SetTableAssign(long tableId);
    Task<bool> SetTableAvailable(long tableId);
    Task<bool> SetTableOccupied(long tableId);
    Task<ResponseViewModel> Delete(long tableId);
    Task<ResponseViewModel> Delete(List<long> tableIdList);
}
