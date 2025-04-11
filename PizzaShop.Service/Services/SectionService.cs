using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class SectionService : ISectionService
{
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IUserService _userService;


    public SectionService(IGenericRepository<Section> sectionRepository, IUserService userService)
    {
        _sectionRepository = sectionRepository;
        _userService = userService;
    }

    #region Get
    public async Task<List<SectionViewModel>> Get()
    {
        IEnumerable<Section>? list = await _sectionRepository.GetByCondition(s => !s.IsDeleted);

        List<SectionViewModel> sections = list.Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
        }).ToList();

        return sections;
    }

    public async Task<SectionViewModel> Get(long sectionId)
    {
        if (sectionId == 0)
        {
            return new SectionViewModel();
        }

        Section section = await _sectionRepository.GetByIdAsync(sectionId);

        SectionViewModel sectionVM = new()
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description
        };

        return sectionVM;
    }
    #endregion Get

    #region  Save
    public async Task<ResponseViewModel> Save(SectionViewModel sectionVM)
    {
        long createrId = await _userService.LoggedInUser();

        Section section = new();
        ResponseViewModel response = new();

        if (sectionVM.Id == 0)
        {
            section.CreatedBy = createrId;
        }
        else if (sectionVM.Id > 0)
        {
            section = await _sectionRepository.GetByIdAsync(sectionVM.Id);
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Section");
            return response;
        }

        section.Name = sectionVM.Name;
        section.Description = sectionVM.Description;
        section.UpdatedBy = createrId;
        section.UpdatedAt = DateTime.Now;

        if (sectionVM.Id == 0)
        {
            if (await _sectionRepository.AddAsync(section))
            {
                response.Success = true;
                response.Message = NotificationMessages.Added.Replace("{0}", "Section");
            }
            else
            {
                response.Success = false;
                response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Section");
            }
        }
        else
        {
            if (await _sectionRepository.UpdateAsync(section))
            {
                response.Success = true;
                response.Message = NotificationMessages.Updated.Replace("{0}", "Section");
            }
            else
            {
                response.Success = false;
                response.Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Section");
            }
        }

        return response;
    }
    #endregion Save

    #region  Delete
    public async Task<ResponseViewModel> Delete(long sectionId)
    {
        Section? section = await _sectionRepository.GetByIdAsync(sectionId);

        section.IsDeleted = true;
        section.UpdatedBy = await _userService.LoggedInUser();
        section.UpdatedAt = DateTime.Now;

        if (await _sectionRepository.UpdateAsync(section))
        {
            return new ResponseViewModel
            {
                Success = true,
                Message = NotificationMessages.Deleted.Replace("{0}", "Section")
            };
        }
        else
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.DeletedFailed.Replace("{0}", "Section")
            };
        }
    }
    #endregion Delete

}
