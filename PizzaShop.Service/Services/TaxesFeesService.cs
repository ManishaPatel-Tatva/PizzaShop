using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
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

    public async Task<List<TaxViewModel>> GetAllTaxes(string search)
    {
        IEnumerable<Taxis> taxes = _taxesRepository.GetByCondition(
           t => t.IsDeleted == false &&
           string.IsNullOrEmpty(search.ToLower()) ||
           t.Name.ToLower().Contains(search.ToLower())
        );

        List<TaxViewModel> TaxList = taxes.Select(t => new TaxViewModel()
        {
            TaxId = t.Id,
            Name = t.Name,
            Type = t.Type,
            IsEnabled = t.IsEnabled,
            Default = t.DefaultTax,
            TaxValue = t.TaxValue
        }).ToList();

        return TaxList;
    }


    public async Task<TaxViewModel> GetTax(long TaxId)
    {
        TaxViewModel model = new();

        if (TaxId == 0)
            return model;

        Taxis tax = await _taxesRepository.GetByIdAsync(TaxId);

        model.Name = tax.Name;
        model.Type = tax.Type;
        model.IsEnabled = tax.IsEnabled;
        model.Default = tax.DefaultTax;
        model.TaxValue = tax.TaxValue;

        return model;
    }

    public async Task<bool> SaveTax(TaxViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.TaxId == 0)
        {
            return await AddTax(model, createrId);
        }
        else if (model.TaxId > 0)
        {
            return await UpdateTax(model, createrId);
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> AddTax(TaxViewModel model, long createrId)
    {
        Taxis tax = new()
        {
            Name = model.Name,
            Type = model.Type,
            IsEnabled = model.IsEnabled,
            DefaultTax = model.Default,
            TaxValue = model.TaxValue,
            CreatedBy = createrId
        };

        return await _taxesRepository.AddAsync(tax);

    }


    public async Task<bool> UpdateTax(TaxViewModel model, long createrId)
    {
        Taxis tax = await _taxesRepository.GetByIdAsync(model.TaxId);

        tax.Name = model.Name;
        tax.Type = model.Type;
        tax.IsEnabled = model.IsEnabled;
        tax.DefaultTax = model.Default;
        tax.TaxValue = model.TaxValue;
        tax.UpdatedBy = createrId;
        tax.UpdatedAt = DateTime.Now;

        return await _taxesRepository.UpdateAsync(tax);
    }

    public async Task<bool> DeleteTax(long taxId, string createrEmail)
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
