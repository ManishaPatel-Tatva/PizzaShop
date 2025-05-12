using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class TaxesFeesService : ITaxesFeesService
{
    private readonly IGenericRepository<Taxis> _taxesRepository;
    private readonly IUserService _userService;

    public TaxesFeesService(IGenericRepository<Taxis> taxesRepository, IUserService userService)
    {
        _taxesRepository = taxesRepository;
        _userService = userService;
    }


    public async Task<TaxPaginationViewModel> Get(FilterViewModel filter)
    {
        filter.Search = string.IsNullOrEmpty(filter.Search) ? "" : filter.Search.Replace(" ", "");

        IEnumerable<Taxis> list = await _taxesRepository.GetByCondition(
            predicate: t => !t.IsDeleted &&
                        (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                        t.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id)
        );

        (IEnumerable<Taxis> taxes, int totalRecord) = _taxesRepository.GetPagedRecords(filter.PageSize, filter.PageNumber, list);

        TaxPaginationViewModel taxVM = new()
        {
            Taxes = taxes.Select(t => new TaxViewModel()
            {
                TaxId = t.Id,
                Name = t.Name,
                IsPercentage = t.IsPercentage,
                IsEnabled = t.IsEnabled,
                Default = t.DefaultTax,
                TaxValue = t.TaxValue
            }).ToList()
        };

        taxVM.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return taxVM;
    }

    public async Task<TaxViewModel> Get(long TaxId)
    {
        Taxis tax = await _taxesRepository.GetByIdAsync(TaxId)
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Tax"));

        TaxViewModel taxVM = new()
        {
            TaxId = TaxId,
            Name = tax.Name,
            IsPercentage = tax.IsPercentage,
            IsEnabled = tax.IsEnabled,
            Default = tax.DefaultTax,
            TaxValue = tax.TaxValue
        };

        return taxVM;
    }

    public async Task<ResponseViewModel> Save(TaxViewModel taxVM)
    {
        Taxis tax = await _taxesRepository.GetByIdAsync(taxVM.TaxId)
        ?? new()
        {
            CreatedBy = await _userService.LoggedInUser()
        };

        ResponseViewModel response = new();

        tax.Name = taxVM.Name;
        tax.IsPercentage = taxVM.IsPercentage;
        tax.IsEnabled = taxVM.IsEnabled;
        tax.DefaultTax = taxVM.Default;
        tax.TaxValue = taxVM.TaxValue;
        tax.UpdatedBy = await _userService.LoggedInUser();
        tax.UpdatedAt = DateTime.Now;

        if (taxVM.TaxId == 0)
        {
            await _taxesRepository.AddAsync(tax);
            response.Success = true;
            response.Message = NotificationMessages.Added.Replace("{0}", "Tax");
        }
        else
        {
            await _taxesRepository.UpdateAsync(tax);
            response.Success = true;
            response.Message = NotificationMessages.Updated.Replace("{0}", "Tax");
        }
        return response;
    }

    public async Task Delete(long taxId)
    {
        Taxis tax = await _taxesRepository.GetByIdAsync(taxId)
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Tax"));

        tax.IsDeleted = true;
        tax.UpdatedBy = await _userService.LoggedInUser();
        tax.UpdatedAt = DateTime.Now;

        await _taxesRepository.UpdateAsync(tax);
    }


}
