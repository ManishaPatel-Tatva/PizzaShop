using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppTableService : IAppTableService
{
    private readonly IGenericRepository<Section> _sectionrepository;

    public AppTableService(IGenericRepository<Section> sectionrepository)
    {
        _sectionrepository = sectionrepository;
    }

    public async Task<List<SectionViewModel>> Get()
    {
        try
        {
            IEnumerable<Section>? list = await _sectionrepository.GetByCondition(
                s => !s.IsDeleted,
                thenIncludes: new List<Func<IQueryable<Section>, IQueryable<Section>>>
                {
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.Status),
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.OrderTableMappings)
                    .ThenInclude(otm => otm.Order)
                }
            );

            List<SectionViewModel> sections = list.Select(s => new SectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Tables = s.Tables.Select(t => new TableCardViewModel
                {
                    Id = t.Id,
                    TableName = t.Name,
                    TableStatus = t.Status.Name,
                    OrderAmount = t.OrderTableMappings
                                .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                                .Select(otm => otm.Order.FinalAmount)
                                .FirstOrDefault(),
                    OrderTime = t.OrderTableMappings
                                .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                                .Select(otm => otm.CreatedAt)
                                .FirstOrDefault(),
                    Capacity = t.Capacity
                }).ToList()
            }).ToList();

            return sections;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
