using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class KotController : Controller
{
    private readonly IKotService _kotService;
    private readonly ICategoryService _categoryService;

    public KotController(IKotService kotService, ICategoryService categoryService)
    {
        _kotService = kotService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        List<CategoryViewModel> list = await _categoryService.Get();
        ViewData["app-active"] = "Kot";
        return View(list);
    }

    public async Task<IActionResult> GetCards(long categoryId, bool isReady, int pageNumber = 1, int pageSize = 4)
    {
        KotViewModel kot = await _kotService.Get(categoryId, pageSize, pageNumber, isReady);
        return PartialView("_CardsPartialView", kot);
    }
    
    [HttpGet]
    public IActionResult GetOrderItems(string kotCard, bool isReady)
    {
        KotCardViewModel kot = new();
        if(!string.IsNullOrEmpty(kotCard)) 
        {
            kot = JsonConvert.DeserializeObject<KotCardViewModel>(kotCard)!;
            kot.IsReady = isReady;
        }

        return PartialView("_OrderItemPartialView",kot);
    }

    [HttpPost]
    public async Task<IActionResult> Update(KotCardViewModel kot)
    {
        ResponseViewModel response = await _kotService.Update(kot);
        return Json(response);
    }



}
