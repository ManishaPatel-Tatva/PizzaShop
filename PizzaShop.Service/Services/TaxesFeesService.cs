using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class TaxesFeesService : ITaxesFeesService
{
    private readonly IGenericRepository<Taxis> _taxesRepository;
    private readonly IGenericRepository<User> _userRepository;

    public TaxesFeesService(IGenericRepository<Taxis> taxesRepository, IGenericRepository<User> userRepository)
    {
        _taxesRepository = taxesRepository;
        _userRepository = userRepository;
    }


    public async Task<TaxPaginationViewModel> Get(FilterViewModel filter)
    {
        filter.Search = string.IsNullOrEmpty(filter.Search) ? "" : filter.Search.Replace(" ", "");

        (IEnumerable<Taxis> taxes, int totalRecord) = await _taxesRepository.GetPagedRecordsAsync(
            filter.PageSize,
            filter.PageNumber,
            predicate: t => !t.IsDeleted &&
                        (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                        t.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id)
        );

        TaxPaginationViewModel model = new()
        {
            Page = new(),
            Taxes = taxes.Select(t => new TaxViewModel()
            {
                TaxId = t.Id,
                Name = t.Name,
                IsPercentage = (bool)t.IsPercentage,
                IsEnabled = t.IsEnabled,
                Default = t.DefaultTax,
                TaxValue = t.TaxValue
            }).ToList()
        };

        model.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return model;
    }

    public async Task<TaxViewModel> Get(long TaxId)
    {
        TaxViewModel model = new();

        if (TaxId == 0)
            return model;

        Taxis tax = await _taxesRepository.GetByIdAsync(TaxId);

        model.TaxId = TaxId;
        model.Name = tax.Name;
        model.IsPercentage = (bool)tax.IsPercentage;
        model.IsEnabled = tax.IsEnabled;
        model.Default = tax.DefaultTax;
        model.TaxValue = tax.TaxValue;

        return model;
    }

    public async Task<ResponseViewModel> Save(TaxViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        Taxis tax = new();

        if (model.TaxId == 0)
        {
            tax.CreatedBy = createrId;
        }
        else if (model.TaxId > 0)
        {
            tax = await _taxesRepository.GetByIdAsync(model.TaxId);
        }
        else
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.Invalid.Replace("{0}", "Tax")
            };
        }

        tax.Name = model.Name;
        tax.IsPercentage = model.IsPercentage;
        tax.IsEnabled = model.IsEnabled;
        tax.DefaultTax = model.Default;
        tax.TaxValue = model.TaxValue;
        tax.UpdatedBy = createrId;
        tax.UpdatedAt = DateTime.Now;

        if (model.TaxId == 0)
        {
            if (await _taxesRepository.AddAsync(tax))
            {
                return new ResponseViewModel
                {
                    Success = true,
                    Message = NotificationMessages.Added.Replace("{0}", "Tax")
                };
            }
            else{
                return new ResponseViewModel
                {
                    Success = false,
                    Message = NotificationMessages.AddedFailed.Replace("{0}", "Tax")
                };
            }
        }
        else
        {
            if (await _taxesRepository.UpdateAsync(tax))
            {
                return new ResponseViewModel
                {
                    Success = true,
                    Message = NotificationMessages.Updated.Replace("{0}", "Tax")
                };
            }
            else{
                return new ResponseViewModel
                {
                    Success = false,
                    Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Tax")
                };
            }
        }
    }

    public async Task<bool> Delete(long taxId, string createrEmail)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        if (user == null)
            return false;

        Taxis tax = await _taxesRepository.GetByIdAsync(taxId);

        tax.IsDeleted = true;
        tax.UpdatedBy = user.Id;
        tax.UpdatedAt = DateTime.Now;

        return await _taxesRepository.UpdateAsync(tax);
    }


}
