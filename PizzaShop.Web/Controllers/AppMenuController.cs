using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class AppMenuController : Controller
{
    private readonly IAppMenuService _appMenuService;
    private readonly IItemService _itemService;
    private readonly ICustomerReviewService _customerReviewService;
    private readonly IOrderService _orderService;

    public AppMenuController(IAppMenuService appMenuService, IItemService itemService, ICustomerReviewService customerReviewService, IOrderService orderService)
    {
        _appMenuService = appMenuService;
        _itemService = itemService;
        _customerReviewService = customerReviewService;
        _orderService = orderService;
    }

    public async Task<ActionResult> Index(long id)
    {
        AppMenuViewModel appMenu = await _appMenuService.Get(id);
        ViewData["app-active"] = "Menu";
        return View(appMenu);
    }

    public async Task<IActionResult> GetCards(long categoryId, string search = "")
    {
        List<ItemInfoViewModel> items = await _appMenuService.List(categoryId, search);
        return PartialView("_CardsPartialView", items);
    }

    [HttpPost]
    public async Task<IActionResult> FavouriteItem(long itemId)
    {
        ResponseViewModel response = await _itemService.Favourite(itemId);
        return Json(response);
    }

    [HttpGet]
    public async Task<IActionResult> ItemDetail(long itemId)
    {
        ItemViewModel item = await _itemService.Get(itemId);
        return PartialView("_ItemModifierPartialView", item);
    }

    [HttpPost]
    public IActionResult AddItem(OrderItemViewModel item)
    {
        return PartialView("_ItemPartialView", item);
    }

    [HttpPost]
    public async Task<IActionResult> SaveOrder([FromBody] OrderDetailViewModel orderVM)
    {
        ResponseViewModel response = await _orderService.Save(orderVM);

        if (response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Success.ToString();
        }

        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> CompleteOrder(long orderId)
    {
        ResponseViewModel response = await _orderService.CompleteOrder(orderId);
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(long orderId)
    {
        ResponseViewModel response = await _orderService.CancelOrder(orderId);
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> CustomerReview([FromBody] CustomerReviewViewModel review)
    {
        ResponseViewModel response = await _customerReviewService.Save(review);
        return Json(response);
    }

}
