using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class SectionService : ISectionService
{
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IUserService _userService;
    private readonly ITransactionRepository _transaction;


    public SectionService(IGenericRepository<Section> sectionRepository, IUserService userService, ITransactionRepository transaction)
    {
        _sectionRepository = sectionRepository;
        _userService = userService;
        _transaction = transaction;

    }

    #region Get
    public async Task<List<SectionViewModel>> Get()
    {
        IEnumerable<Section>? list = await _sectionRepository.GetByCondition(
            predicate: s => !s.IsDeleted,
            orderBy: q => q.OrderBy(s => s.Id)
            );

        List<SectionViewModel> sections = list.Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
        }).ToList();

        return sections;
    }

    public async Task<SectionViewModel> Get(long sectionId)
    {
        SectionViewModel sectionVM = new();

        Section? section = await _sectionRepository.GetByIdAsync(sectionId);
        if (section == null)
        {
            return sectionVM;
        }

        sectionVM.Id = section.Id;
        sectionVM.Name = section.Name;
        sectionVM.Description = section.Description;

        return sectionVM;
    }
    #endregion Get

    #region  Save
    public async Task<ResponseViewModel> Save(SectionViewModel sectionVM)
    {
        Section section = await _sectionRepository.GetByIdAsync(sectionVM.Id)
                        ?? new()
                        {
                            CreatedBy = await _userService.LoggedInUser()
                        };

        ResponseViewModel response = new();


        section.Name = sectionVM.Name;
        section.Description = sectionVM.Description;
        section.UpdatedAt = DateTime.Now;
        section.UpdatedBy = await _userService.LoggedInUser();

        if (sectionVM.Id == 0)
        {
            await _sectionRepository.AddAsync(section);
            response.Success = true;
            response.Message = NotificationMessages.Added.Replace("{0}", "Section");
        }
        else
        {
            await _sectionRepository.UpdateAsync(section);
            response.Success = true;
            response.Message = NotificationMessages.Updated.Replace("{0}", "Section");
        }

        return response;
    }
    #endregion Save

    #region  Delete
    public async Task Delete(long sectionId)
    {
        try
        {
            await _transaction.BeginTransactionAsync();
            Section section = await _sectionRepository.GetByIdAsync(sectionId)
                            ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Section"));

            section.IsDeleted = true;
            section.UpdatedBy = await _userService.LoggedInUser();
            section.UpdatedAt = DateTime.Now;

            



            await _sectionRepository.UpdateAsync(section);
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }
    #endregion Delete

}
