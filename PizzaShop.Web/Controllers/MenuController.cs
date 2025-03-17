using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
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

#region Category    
 
#region Edit Category
 /*-------------------------------------------------------- Edit Category---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public IActionResult EditCategory(long categoryId)
    {
        var category = _categoryItemService.GetCategoryById(categoryId);
        return Json(category);
    }

/*--------------------------------------------------------Edit Category Post---------------------------------------------------------------------------------------------------
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
/*--------------------------------------------------------AddCategory--------------------------------------------------------------------------------------------------------
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

#region  Display Item
/*--------------------------------------------------------Display Items--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    [HttpGet]
    public async Task<IActionResult> GetItems(long categoryId, int pageSize, int pageNumber = 1, string search="")
    {
        var model = await _categoryItemService.GetPagedItems(categoryId, pageSize, pageNumber,search);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_ItemsPartialView", model);
    }
#endregion Display Item

#region Get Add/Update
/*--------------------------------------------------------Get Add/Update Items--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetItemModal(long itemId)
    {
        AddItemViewModel model = await _categoryItemService.GetEditItem(itemId);
        return PartialView("_UpdateItemPartialView", model); 
    }

    public async Task<IActionResult> SelectModifierGroup(long modifierGroupId)
    {
        ItemModifierViewModel model = await _categoryItemService.GetModifierOnSelection(modifierGroupId);
        return PartialView("_ItemModifierPartialView", model); 
    }
    
#endregion Get Add/Update

#region Update Item
/*--------------------------------------------------------Add/Update Item--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    public async Task<IActionResult> AddUpdateItem(AddItemViewModel model, string modifierGroupList)
    {
        if (!ModelState.IsValid)
        {
            AddItemViewModel updatedModel = await _categoryItemService.GetEditItem(model.ItemId);
            return PartialView("_UpdateItemPartialView", updatedModel); 
        }

        if(!string.IsNullOrEmpty(modifierGroupList))
        {
            model.ItemModifierGroups = JsonSerializer.Deserialize<List<ItemModifierViewModel>>(modifierGroupList);
        }

        if(model.ItemId == 0)
        {
            var token = Request.Cookies["authToken"];
            var createrEmail = _jwtService.GetClaimValue(token, "email");

            bool isAdded = await _categoryItemService.AddItem(model, createrEmail);
            
            if (!isAdded)
            {
                AddItemViewModel updatedModel = await _categoryItemService.GetEditItem(model.ItemId);
                TempData["errorMessage"] = "Item Not Updated";
                return RedirectToAction("Index");
            }
            TempData["successMessage"] = "Item Added Successfully!";
            return RedirectToAction("Index");
        }

        bool isUpdated = await _categoryItemService.UpdateItem(model);
        if (!isUpdated)
        {
            AddItemViewModel updatedModel = await _categoryItemService.GetEditItem(model.ItemId);
            TempData["errorMessage"] = "Item Not Updated";
            return RedirectToAction("Index");
        }

        TempData["successMessage"] = "Item Updated";
        return RedirectToAction("Index");
    }


#endregion Update Item

#region Delete Item
/*--------------------------------------------------------Delete One Item--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IActionResult> SoftDeleteItem(long id)
    {
        bool success = await _categoryItemService.SoftDeleteItem(id);

        if(!success)
        {
            return Json(new {success = false, message="Item Not deleted"});
        }
        return Json(new {success = true, message="Item deleted Successfully!"});
    }

/*--------------------------------------------------------Delete Multiple Items--------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IActionResult> MassDeleteItems(List<long> itemsList)
    {
        bool success = await _categoryItemService.MassDeleteItems(itemsList);

        if(!success)
        {
            return Json(new {success = false, message="Items Not deleted"});
        }
        return Json(new {success = true, message="All selected Items deleted Successfully!"});
    }

#endregion Delete Item


#endregion Items



}
