using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class MenuController : Controller
{
    private readonly ICategoryItemService _categoryItemService;
    private readonly IJwtService _jwtService;

    public MenuController(ICategoryItemService categoryItemService, IJwtService jwtService)
    {
        _categoryItemService = categoryItemService;
        _jwtService = jwtService;
    }

/*--------------------------------------------------------Menu Index---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult Index()
    {
        var categoriesList = _categoryItemService.GetCategory();
    
        MenuViewModel model = new MenuViewModel
        {
            Categories = categoriesList,
            ItemsPageVM = new ItemsPaginationViewModel{
                Items = Enumerable.Empty<ItemInfoViewModel>(),
                Page = new Pagination() 
            }
        };
        ViewData["sidebar-active"] = "Menu";
        return View(model);
    }

#region  Category    
 
 #region Edit Category
 /*--------------------------------------------------------Menu Index---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public IActionResult EditCategory(long categoryId)
    {
        var category = _categoryItemService.GetCategoryById(categoryId);
        return Json(category);
    }

/*--------------------------------------------------------Menu Index---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    public async Task<IActionResult> EditCategory(MenuViewModel model)
    {
        CategoryViewModel categoryVM = model.CategoryVM;
        var success = await _categoryItemService.EditCategory(categoryVM);
        return RedirectToAction("Index", "Menu");
    }
#endregion Edit Category

#region Add Category
/*--------------------------------------------------------Menu Index--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    public async Task<IActionResult> AddCategory(MenuViewModel model)
    {
        var token = Request.Cookies["authToken"];
        var createrEmail = _jwtService.GetClaimValue(token, "email");

        CategoryViewModel categoryVM = model.CategoryVM;
        var success = await _categoryItemService.AddCategory(categoryVM, createrEmail);
        return RedirectToAction("Index", "Menu");
    }
#endregion Add Category

#region Delete Category
/*--------------------------------------------------------Menu Index--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> SoftDelete(long categoryId)
    {
        var success = await _categoryItemService.SoftDelete(categoryId);
        return RedirectToAction("Index","Menu");
    }

#endregion Delete Category

#endregion Category

#region Items

/*--------------------------------------------------------Display Items--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region  Display Item
    [HttpGet]
    public async Task<IActionResult> GetItems(int pageSize, int pageNumber = 1)
    {
        var model = _categoryItemService.GetItems(pageSize, pageNumber);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_ItemsPartialView", model);
    }
#endregion Display Item

/*--------------------------------------------------------Add/Update Items--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Add/Update

    [HttpGet]
    public async Task<IActionResult> GetItemModal(long itemId)
    {
        if(itemId == 0)
        {
            AddItemViewModel model = new AddItemViewModel{Name = ""};
            return PartialView("_UpdateItemPartialView", model);
        }
        else
        {
            AddItemViewModel model = await _categoryItemService.GetEditItem(itemId);
            return PartialView("_UpdateItemPartialView", model);
        }
        
    }

#endregion Add/Update

#endregion Items



}
