using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class MenuController : Controller
{
    private readonly IJwtService _jwtService;
    private readonly ICategoryService _categoryService;
    private readonly IItemService _ItemService;
    private readonly IModifierService _modifierService;

    public MenuController(IItemService ItemService, IJwtService jwtService, IModifierService modifierService, ICategoryService categoryService)
    {
        _ItemService = ItemService;
        _jwtService = jwtService;
        _modifierService = modifierService;
        _categoryService = categoryService;

    }

    /*--------------------------------------------------------Menu Index---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Menu")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<CategoryViewModel>? categoriesList = await _categoryService.Get();

        MenuViewModel model = new()
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
     /*-------------------------------------------------------- Get Category---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpGet]
    public async Task<IActionResult> GetCategoryModal(long categoryId)
    {
        CategoryViewModel category = await _categoryService.Get(categoryId);
        return PartialView("_CategoryPartialView", category);
    }

    /*--------------------------------------------------------Add/Edit Category--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpPost]
    public async Task<IActionResult> SaveCategory(CategoryViewModel category)
    {
        ResponseViewModel response = await _categoryService.Save(category);
        if (!response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return PartialView("_CategoryPartialView", category);
        }
        return Json(response);
    }

    /*--------------------------------------------------------Menu Index--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Delete_Menu")]
    [HttpGet]
    public async Task<IActionResult> DeleteCategory(long categoryId)
    {
        ResponseViewModel response = await _categoryService.Delete(categoryId);
        return Json(response);
    }
    
    #endregion Category

    #region Items
    /*--------------------------------------------------------Display Items--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Menu")]
    [HttpGet]
    public async Task<IActionResult> GetItems(long categoryId, int pageSize, int pageNumber = 1, string search = "")
    {
        ItemsPaginationViewModel? model = await _ItemService.GetPagedItems(categoryId, pageSize, pageNumber, search);
        return PartialView("_ItemsPartialView", model);
    }

    /*--------------------------------------------------------Get Add/Update Items--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpGet]
    public async Task<IActionResult> GetItemModal(long itemId)
    {
        AddItemViewModel model = await _ItemService.GetEditItem(itemId);
        return PartialView("_UpdateItemPartialView", model);
    }

    [CustomAuthorize("Edit_Menu")]
    public async Task<IActionResult> SelectModifierGroup(long modifierGroupId)
    {
        ItemModifierViewModel model = await _ItemService.GetModifierOnSelection(modifierGroupId);
        return PartialView("_ItemModifierPartialView", model);
    }

    /*--------------------------------------------------------Add/Update Item--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpPost]
    public async Task<IActionResult> AddUpdateItem(AddItemViewModel model, string modifierGroupList)
    {
        if (!ModelState.IsValid)
        {
            AddItemViewModel updatedModel = await _ItemService.GetEditItem(model.ItemId);
            return PartialView("_UpdateItemPartialView", updatedModel);
        }

        if (!string.IsNullOrEmpty(modifierGroupList))
        {
            model.ItemModifierGroups = JsonSerializer.Deserialize<List<ItemModifierViewModel>>(modifierGroupList);
        }

        string? token = Request.Cookies["authToken"];
        string? createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _ItemService.AddUpdateItem(model, createrEmail);

        if (!success)
        {
            AddItemViewModel updatedModel = await _ItemService.GetEditItem(model.ItemId);
            return PartialView("_UpdateItemPartialView", updatedModel);
        }

        if(model.ItemId == 0)
        {
            return Json(new { success = true, message = "Item Added Successfully!" });
        }
        else
        {
            return Json(new { success = true, message = "Item Updated Successfully!" });
        }
    }

    /*--------------------------------------------------------Delete One Item--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Delete_Menu")]
    public async Task<IActionResult> SoftDeleteItem(long id)
    {
        bool success = await _ItemService.SoftDeleteItem(id);

        if (!success)
        {
            return Json(new { success = false, message = "Item Not deleted" });
        }
        return Json(new { success = true, message = "Item deleted Successfully!" });
    }

    /*--------------------------------------------------------Delete Multiple Items--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Delete_Menu")]
    public async Task<IActionResult> MassDeleteItems(List<long> itemsList)
    {
        bool success = await _ItemService.MassDeleteItems(itemsList);

        if (!success)
        {
            return Json(new { success = false, message = "Items Not deleted" });
        }
        return Json(new { success = true, message = "All selected Items deleted Successfully!" });
    }

    #endregion Items

    #region Modifier Group
    /*-------------------------------------------------------- Read Modifier Group---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Menu")]
    [HttpGet]
    public IActionResult GetModifierTab()
    {
        ModifierTabViewModel model = new()
        {
            ModifierGroups = _modifierService.GetModifierGroups()
        };

        return PartialView("_ModifierTabPartialView", model);
    }

    /*-------------------------------------------------------- Get Modifier Group---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpGet]
    public async Task<IActionResult> GetModifierGroupModal(long modifierGroupId)
    {
        ModifierGroupViewModel model = await _modifierService.GetModifierGroup(modifierGroupId);
        return PartialView("_ModifierGroupPartialView", model);
    }

    [CustomAuthorize("Edit_Menu")]
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

        if(model.ModifierGroupId == 0)
        {
            return Json(new { success = true, message = "Modifier Group Added Successful!" });
        }
        else
        {
            return Json(new { success = true, message = "Modifier Group Updated Successful!" });
        }

    }

    [CustomAuthorize("Delete_Menu")]
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
    #endregion

    #region Modifier
    /*-------------------------------------------------------- Get Modifier ---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Menu")]
    [HttpGet]
    public async Task<IActionResult> GetModifierModal(long modifierId)
    {
        ModifierViewModel model = await _modifierService.GetModifier(modifierId);
        return PartialView("_ModifierPartialView", model);
    }

    [CustomAuthorize("View_Menu")]
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

    [CustomAuthorize("Edit_Menu")]
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

    [CustomAuthorize("Edit_Menu")]
    [HttpPost]
    public async Task<IActionResult> SaveModifier(ModifierViewModel model, string selectedMG)
    {
        if (!ModelState.IsValid)
        {
            ModifierViewModel updatedModel = await _modifierService.GetModifier(model.Id);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }

        if (!string.IsNullOrEmpty(selectedMG))
        {
            model.SelectedMgList = JsonSerializer.Deserialize<List<long>>(selectedMG);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _modifierService.SaveModifier(model, createrEmail);
        if (!success)
        {
            ModifierViewModel updatedModel = await _modifierService.GetModifier(model.Id);
            return PartialView("_ModifierGroupPartialView", updatedModel);
        }
        return Json(new { success = true, message = "Modifier Added Successful!" });

    }

    /*--------------------------------------------------------Delete One Modifier--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Delete_Menu")]
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
    [CustomAuthorize("Delete_Menu")]
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

    #endregion Modifier


}
