using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class KotService : IKotService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<Order> _orderRepository;

    public KotService(IGenericRepository<Category> categoryRepository, IGenericRepository<Order> orderRepository)
    {
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;

    }

    #region Get
    /*----------------------------------------------------Get Category List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<List<CategoryViewModel>> Get()
    {
        IEnumerable<Category>? categories = await _categoryRepository.GetByCondition(
            predicate: c => !c.IsDeleted,
            orderBy: q => q.OrderBy(c => c.Id));

        List<CategoryViewModel> list = categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return list;
    }

    public async Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady)
    {
        try
        {
            IEnumerable<Order> orders = await _orderRepository.GetByCondition(
            predicate: o => !o.IsDeleted
                            && o.OrderItems.Any(oi => !oi.IsDeleted
                                                    && (categoryId == 0 || oi.Item.CategoryId == categoryId)),
            orderBy: q => q.OrderBy(o => o.Id),
            thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
            {
                q => q.Include(o => o.OrderTableMappings)
                    .ThenInclude(otm => otm.Table)
                    .ThenInclude(t => t.Section),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .ThenInclude(i => i.Category),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemsModifiers)
                    .ThenInclude(m => m.Modifier),
            }
        );

            (orders, int totalRecord) = await _orderRepository.GetPagedRecords(pageSize, pageNumber, orders);

            KotViewModel kot = new()
            {
                CategoryId = categoryId,
                CategoryName = categoryId == 0 ? "All" : orders.SelectMany(o => o.OrderItems)
                                                                .Where(oi => oi.Item.CategoryId == categoryId)
                                                                .Select(oi => oi.Item.Category.Name)
                                                                .FirstOrDefault()!,
                IsReady = isReady,
                KotCards = orders.Select(o => new KotCardViewModel
                {
                    OrderId = o.Id,
                    SectionName = o.OrderTableMappings
                                .Where(otm => otm.OrderId == o.Id)
                                .Select(otm => otm.Table.Section.Name)
                                .FirstOrDefault()!,
                    Tables = o.OrderTableMappings
                                .Where(otm => otm.OrderId == o.Id)
                                .Select(otm => otm.Table.Name)
                                .ToList(),
                    Time = o.CreatedAt,
                    Items = o.OrderItems
                            .Where(oi => (categoryId == 0 || oi.Item.CategoryId == categoryId) && ((isReady &&  oi.ReadyQuantity > 0) || (!isReady && oi.Quantity - oi.ReadyQuantity > 0)) )
                            .Select(oi => new OrderItemViewModel
                            {
                                ItemName = oi.Item.Name,
                                Quantity = isReady ? oi.ReadyQuantity : oi.Quantity - oi.ReadyQuantity,
                                ModifiersList = oi.OrderItemsModifiers
                                                .Select(oim => new ModifierViewModel
                                                {
                                                    ModifierName = oim.Modifier.Name,
                                                    Quantity = oim.Quantity
                                                }).ToList(),
                                Instruction = oi.Instructions,
                            }).ToList(),
                    Instruction = o.Instructions
                }).Where(c => c.Items.Count > 0).ToList(),
                Page = new()
            };

            kot.Page.SetPagination(totalRecord, pageSize, pageNumber);

            return kot;
        }
        catch (Exception ex)
        {
            return null;
        }


    }

    #endregion



}
