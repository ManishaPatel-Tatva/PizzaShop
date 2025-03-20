using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableSectionService
{
    List<SectionViewModel> GetAllSections();
    Task<SectionViewModel> GetSection(long sectionId);
    Task<bool> SaveSection(SectionViewModel model, string createrEmail);
    Task<bool> AddSection(SectionViewModel model, long createrId);
    Task<bool> UpdateSection(SectionViewModel model, long createrId);
    Task<bool> DeleteSection(long sectionId);
    Task<TablesPaginationViewModel> GetPagedTables(long sectionId, int pageSize, int pageNumber, string search);
    Task<TableViewModel> GetTable(long tableId);
    Task<bool> SaveTable(TableViewModel model, string createrEmail);
    Task<bool> AddTable(TableViewModel model, long createrId);
    Task<bool> UpdateTable(TableViewModel model, long createrId);
    Task<bool> DeleteTable(long tableId);
    Task<bool> MassDeleteTables(List<long> tableIdList);
}
