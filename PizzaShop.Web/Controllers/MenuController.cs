using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class MenuController : Controller
{
    private readonly IJwtService _jwtService;
    private readonly ICategoryItemService _categoryItemService;
    private readonly IModifierService _modifierService;

    public MenuController(ICategoryItemService categoryItemService, IJwtService jwtService, IModifierService modifierService)
    {
        _categoryItemService = categoryItemService;
        _jwtService = jwtService;
        _modifierService = modifierService;
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
            ItemsPageVM = new ItemsPaginationViewModel
            {
                Items = Enumerable.Empty<ItemInfoViewModel>(),
                Page = new Pagination()
            }
        };
        ViewData["sidebar-active"] = "Menu";
        return View(model);
    }

    #region Category   

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

    #region Delete Category
    /*--------------------------------------------------------Menu Index--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> SoftDelete(long categoryId)
    {
        var success = await _categoryItemService.SoftDelete(categoryId);
        return RedirectToAction("Index", "Menu");
    }

    #endregion Delete Category

    #endregion Category

    #region Items

    #region  Display Item
    /*--------------------------------------------------------Display Items--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetItems(long categoryId, int pageSize, int pageNumber = 1, string search = "")
    {
        var model = await _categoryItemService.GetPagedItems(categoryId, pageSize, pageNumber, search);
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

        if (!string.IsNullOrEmpty(modifierGroupList))
        {
            model.ItemModifierGroups = JsonSerializer.Deserialize<List<ItemModifierViewModel>>(modifierGroupList);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _categoryItemService.AddUpdateItem(model, createrEmail);

        if (!success)
        {
            AddItemViewModel updatedModel = await _categoryItemService.GetEditItem(model.ItemId);
            return PartialView("_UpdateItemPartialView", updatedModel);
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

        if (!success)
        {
            return Json(new { success = false, message = "Item Not deleted" });
        }
        return Json(new { success = true, message = "Item deleted Successfully!" });
    }

    /*--------------------------------------------------------Delete Multiple Items--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IActionResult> MassDeleteItems(List<long> itemsList)
    {
        bool success = await _categoryItemService.MassDeleteItems(itemsList);

        if (!success)
        {
            return Json(new { success = false, message = "Items Not deleted" });
        }
        return Json(new { success = true, message = "All selected Items deleted Successfully!" });
    }

    #endregion Delete Item

    #endregion Items

    #region Modifier Group

    #region Modifier Tab
    /*-------------------------------------------------------- Read Modifier Group---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult GetModifierTab()
    {
        ModifierTabViewModel model = new()
        {
            ModifierGroups = _modifierService.GetModifierGroups()
        };

        return PartialView("_ModifierTabPartialView", model);
    }
    #endregion Get Modifier Tab

    #region Display Modifier Group
    /*-------------------------------------------------------- Get Modifier Group---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetModifierGroupModal(long modifierGroupId)
    {
        ModifierGroupViewModel model = await _modifierService.GetModifierGroup(modifierGroupId);
        return PartialView("_ModifierGroupPartialView", model);
    }
    #endregion Display Modifier Group

    #region Add/Update Modifier Group

    [HttpPost]
    public async Task<IActionResult> SaveModifierGroup(ModifierGroupViewModel model, string modifierList)
    {
        if (!ModelState.IsValid)
        {
            ModifierGroupViewModel updatedModel = await _modifierService.GetModifierGroup(model.ModifierGroupId);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }

        if (!string.IsNullOrEmpty(modifierList))
        {
            model.ModifierIdList = JsonSerializer.Deserialize<List<long>>(modifierList);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.SaveModifierGroup(model, createrEmail);
        if (!success)
        {
            ModifierGroupViewModel updatedModel = await _modifierService.GetModifierGroup(model.ModifierGroupId);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }
        return Json(new { success = true, message = "Modifier Group Added Successful!" });

    }
    #endregion Add/Update Modifier Group

    #region Delete Modifier Group
    [HttpPost]
    public async Task<IActionResult> DeleteModifierGroup(long modifierGroupId)
    {
        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.DeleteModifierGroup(modifierGroupId, createrEmail);
        if (!success)
        {
            return Json(new { success = false, message = "Modifier Group Not deleted!" });
        }
        return Json(new { success = true, message = "Modifier Group deleted Successfully!" });
    }
    #endregion Delete Modifier Group

    #endregion Modifier Group

    #region Modifier

    #region Display Modifiers
    /*-------------------------------------------------------- Get Modifier ---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetModifierModal(long modifierId)
    {
        ModifierViewModel model = await _modifierService.GetModifier(modifierId);
        return PartialView("_ModifierPartialView", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiersList(long modifierGroupId, int pageSize, int pageNumber = 1, string search = "")
    {
        ModifiersPaginationViewModel model = await _modifierService.GetPagedModifiers(modifierGroupId, pageSize, pageNumber, search);

        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }

        return PartialView("_ModifiersListPartialView", model);
    }

    [HttpGet]
    public async Task<IActionResult> ExistingModifiers(int pageSize, int pageNumber = 1, string search = "")
    {
        ModifiersPaginationViewModel model = await _modifierService.GetAllModifiers(pageSize, pageNumber, search);

        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }

        return PartialView("_ExistingModifierPartialView", model);
    }

    #endregion Display Modifiers

    #region Add/Update Modifier

    [HttpPost]
    public async Task<IActionResult> SaveModifier(ModifierViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModifierViewModel updatedModel = await _modifierService.GetModifier(model.ModifierId);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.SaveModifier(model, createrEmail);
        if (!success)
        {
            ModifierViewModel updatedModel = await _modifierService.GetModifier(model.ModifierId);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }
        return Json(new { success = true, message = "Modifier Added Successful!" });

    }
    #endregion Add/Update Modifier

    #region Delete Modifier
    /*--------------------------------------------------------Delete One Modifier--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    public async Task<IActionResult> DeleteModifier(long modifierId,long modifierGroupId)
    {
        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.DeleteModifier(modifierId, modifierGroupId, createrEmail);

        if (!success)
        {
            return Json(new { success = false, message = "Modifier Not deleted!" });
        }
        return Json(new { success = true, message = "Modifier deleted Successfully!" });
    }

    /*--------------------------------------------------------Delete Multiple Modifiers--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    public async Task<IActionResult> MassDeleteModifiers(List<long> modifierIdList,long modifierGroupId)
    {
        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.MassDeleteModifiers(modifierIdList, modifierGroupId, createrEmail);

        if (!success)
        {
            return Json(new { success = false, message = "Items Not deleted" });
        }
        return Json(new { success = true, message = "All selected Items deleted Successfully!" });
    }

    #endregion Delete Modifier

    #endregion Modifier


}
