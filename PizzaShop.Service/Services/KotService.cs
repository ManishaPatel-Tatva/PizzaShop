using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class KotService : IKotService
{
    private readonly ICategoryService _categoryService;
    private readonly IKotRepository _kotRepository;

    public KotService(ICategoryService categoryService, IKotRepository kotRepository)
    {
        _categoryService = categoryService;
        _kotRepository = kotRepository;
    }

    #region Get

    public async Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady)
    {
        KotViewModel kotVM = new() {};
        IEnumerable<KotDbViewModel>? KotDB = await _kotRepository.Get(categoryId, isReady);

        kotVM.CategoryId = categoryId;
        kotVM.CategoryName = categoryId == 0 ? "All" : _categoryService.Get(categoryId).Result.Name;
        kotVM.IsReady = isReady;
        kotVM.KotCards = KotDB.Select(kot => new KotCardViewModel
        {
            OrderId = kot.OrderId,
            SectionName = kot.SectionName,
            Tables = kot.Tables.ToList(),
            Time = kot.Time,
            Items = JsonSerializer.Deserialize<List<OrderItemViewModel>>(kot.Items) ?? new(),
            Instruction = kot.Instruction,
        }).Where(c => c.Items.Count > 0).ToList();

        return kotVM;

        }

    // public async Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady)
    // {

        //     long completeId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Completed").Result!.Id;
        //     long cancelId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Cancelled").Result!.Id;

        //     IEnumerable<Order> orders = await _orderRepository.GetByCondition(
        //     predicate: o => !o.IsDeleted
        //                     && o.StatusId != completeId
        //                     && o.StatusId != cancelId
        //                     && o.OrderItems.Any(oi => !oi.IsDeleted
        //                     && (categoryId == 0 || oi.Item.CategoryId == categoryId)),
        //     orderBy: q => q.OrderBy(o => o.Id),
        //     thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
        //     {
        //             q => q.Include(o => o.OrderTableMappings)
        //                 .ThenInclude(otm => otm.Table)
        //                 .ThenInclude(t => t.Section),
        //             q => q.Include(o => o.OrderItems)
        //                 .ThenInclude(oi => oi.Item)
        //                 .ThenInclude(i => i.Category),
        //             q => q.Include(o => o.OrderItems)
        //                 .ThenInclude(oi => oi.OrderItemsModifiers)
        //                 .ThenInclude(m => m.Modifier),
        //     });

        //     KotViewModel kot = new()
        //     {
        //         CategoryId = categoryId,
        //         CategoryName = categoryId == 0 ? "All" : _categoryService.Get(categoryId).Result.Name,
        //         IsReady = isReady,
        //         KotCards = orders.Select(o => new KotCardViewModel
        //         {
        //             OrderId = o.Id,

        //             SectionName = o.OrderTableMappings
        //                         .Where(otm => otm.OrderId == o.Id)
        //                         .Select(otm => otm.Table.Section.Name)
        //                         .FirstOrDefault()!,

        //             Tables = o.OrderTableMappings
        //                         .Where(otm => otm.OrderId == o.Id)
        //                         .Select(otm => otm.Table.Name)
        //                         .ToList(),

        //             Time = o.CreatedAt,

        //             Items = o.OrderItems
        //                     .Where(oi => (categoryId == 0 || oi.Item.CategoryId == categoryId) && (!oi.IsDeleted) && ((isReady && oi.ReadyQuantity > 0) || (!isReady && oi.Quantity - oi.ReadyQuantity > 0)))
        //                     .Select(oi => new OrderItemViewModel
        //                     {
        //                         Id = oi.Id,
        //                         ItemId = oi.ItemId,
        //                         Name = oi.Item.Name,
        //                         Quantity = isReady ? oi.ReadyQuantity : oi.Quantity - oi.ReadyQuantity,
        //                         ModifiersList = oi.OrderItemsModifiers
        //                                         .Select(oim => new ModifierViewModel
        //                                         {
        //                                             Name = oim.Modifier.Name,
        //                                             Quantity = oim.Quantity
        //                                         }).ToList(),
        //                         Instruction = oi.Instructions,
        //                     }).ToList(),

        //             Instruction = o.Instructions

        //         }).Where(c => c.Items.Count > 0).ToList(),
        //     };

        //     return kot;
        // }

        #endregion

        #region Update

    public async Task<ResponseViewModel> Update(KotCardViewModel kot)
    {
        ResponseViewModel response = new();

        if (kot.Items.All(i => i.IsSelected == false))
        {
            response.Success = false;
            response.Message = NotificationMessages.AtleastOne.Replace("{0}", "Item");
            return response;
        }

        foreach (OrderItemViewModel? item in kot.Items)
        {
            if (item.IsSelected)
            {
                await _kotRepository.Update(kot.IsReady, item.Quantity, item.Id);
            }
        }
        response.Success = true;
        response.Message = NotificationMessages.Updated.Replace("{0}", "Item Status");
        return response;
    }

    #endregion

}
