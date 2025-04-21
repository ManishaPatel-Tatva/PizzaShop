using System.Linq.Expressions;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class WaitingListService : IWaitingListService
{
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly ISectionService _sectionService;
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;
    private readonly ITableService _tableService;

    public WaitingListService(IGenericRepository<WaitingToken> waitingTokenRepository, IUserService userService, ICustomerService customerService, IGenericRepository<Section> sectionRepository, ITableService tableService, ISectionService sectionService)
    {
        _waitingTokenRepository = waitingTokenRepository;
        _userService = userService;
        _customerService = customerService;
        _sectionRepository = sectionRepository;
        _tableService = tableService;
        _sectionService = sectionService;

    }

    #region Get
    public async Task<List<SectionViewModel>> Get()
    {
        IEnumerable<Section>? list = await _sectionRepository.GetByCondition(
            predicate: s => !s.IsDeleted,

            orderBy: q => q.OrderBy(s => s.Id),
            includes: new List<Expression<Func<Section, object>>>
            {
                s => s.WaitingTokens
            }
        );

        List<SectionViewModel> sections = list.Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
            TokenCount = s.WaitingTokens.Where(wt => wt.SectionId == s.Id && !wt.IsAssigned && !wt.IsDeleted).Count()
        }).ToList();

        return sections;
    }

    public async Task<WaitingTokenViewModel> Get(long tokenId)
    {
        WaitingTokenViewModel? tokenVM = new()
        {
            Sections = await _sectionService.Get()
        };

        if (tokenId == 0)
        {
            return tokenVM;
        }

        WaitingToken? token = _waitingTokenRepository.GetByCondition(
            predicate: t => t.Id == tokenId && !t.IsDeleted,
            includes: new List<Expression<Func<WaitingToken, object>>>
            {
                t => t.Customer
            }
        ).Result.FirstOrDefault();

        if (token == null)
        {
            return tokenVM;
        }

        tokenVM.Id = tokenId;
        tokenVM.CustomerId = token.CustomerId;
        tokenVM.Name = token.Customer.Name;
        tokenVM.Email = token.Customer.Email;
        tokenVM.Phone = token.Customer.Phone;
        tokenVM.Members = token.Members;
        tokenVM.SectionId = token.SectionId;

        return tokenVM;
    }

    public async Task<List<WaitingTokenViewModel>> List(long sectionId)
    {
        try
        {
            IEnumerable<WaitingToken>? list = await _waitingTokenRepository.GetByCondition(
                predicate: wt => !wt.IsDeleted
                            && !wt.IsAssigned
                            && (sectionId == 0 || wt.SectionId == sectionId),
                orderBy: q => q.OrderBy(w => w.Id),
                includes: new List<Expression<Func<WaitingToken, object>>>
                {
                    w => w.Customer
                }
            );

            List<WaitingTokenViewModel> waitingTokens = list.Select(w => new WaitingTokenViewModel
            {
                Id = w.Id,
                CreatedAt = w.CreatedAt,
                CustomerId = w.CustomerId,
                Name = w.Customer.Name,
                Members = w.Members,
                Phone = w.Customer.Phone,
                Email = w.Customer.Email,
            }).ToList();

            return waitingTokens;
        }
        catch (Exception ex)
        {
            return new List<WaitingTokenViewModel>();
        }
    }

    #endregion Get

    #region Save

    public async Task<ResponseViewModel> Save(WaitingTokenViewModel wtokenVM)
    {
        try
        {
            ResponseViewModel response = new();

            //Add Customer
            CustomerViewModel customer = new()
            {
                Id = wtokenVM.CustomerId,
                Name = wtokenVM.Name,
                Email = wtokenVM.Email,
                Phone = wtokenVM.Phone
            };

            response = await _customerService.Save(customer);
            if (!response.Success)
            {
                return response;
            }

            // Waiting Token
            long createrId = await _userService.LoggedInUser();

            WaitingToken? token = new();

            if (wtokenVM.Id == 0)
            {
                //Check if waiting token already existing for that customer
                WaitingToken? existingToken = await _waitingTokenRepository.GetByStringAsync(wt => wt.CustomerId == wtokenVM.CustomerId && !wt.IsDeleted);
                if (existingToken != null)
                {
                    response.Success = false;
                    response.Message = NotificationMessages.AlreadyExisted.Replace("{0}", "Waiting Token");
                    return response;
                }
                token.CreatedBy = createrId;
            }
            else if (wtokenVM.Id > 0)
            {
                token = await _waitingTokenRepository.GetByIdAsync(wtokenVM.Id);
            }
            else
            {
                response.Success = false;
                response.Message = NotificationMessages.Invalid.Replace("{0}", "Waiting Token");
                return response;
            }

            token.CustomerId = response.EntityId;
            token.SectionId = wtokenVM.SectionId;
            token.Members = wtokenVM.Members;
            token.UpdatedBy = createrId;
            token.UpdatedAt = DateTime.Now;

            response.EntityId = token.Id;

            if (wtokenVM.Id == 0)
            {
                response.EntityId = await _waitingTokenRepository.AddAsyncReturnId(token);
                response.Success = response.EntityId > 0;
                response.Message = response.Success ? NotificationMessages.Added.Replace("{0}", "Waiting Token") : NotificationMessages.AddedFailed.Replace("{0}", "Waiting Token");
            }
            else
            {
                response.Success = await _waitingTokenRepository.UpdateAsync(token);
                response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Waiting Token") : NotificationMessages.UpdatedFailed.Replace("{0}", "Waiting Token");
            }

            return response;

        }
        catch (Exception ex)
        {
            return new ResponseViewModel()
            {
                Success = false,
                ExMessage = ex.Message
            };
        }
    }

    public async Task<bool> AssignTable(long tokenId)
    {
        WaitingToken? token = await _waitingTokenRepository.GetByIdAsync(tokenId);

        if (token == null)
        {
            return false;
        }

        token.IsAssigned = true;
        return await _waitingTokenRepository.UpdateAsync(token);
    }

    #endregion Save

    #region  Delete

    public async Task<ResponseViewModel> Delete(long tokenId)
    {
        WaitingToken? token = await _waitingTokenRepository.GetByIdAsync(tokenId);
        ResponseViewModel response = new();

        if (token == null)
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Waiting Token");
        }

        token.IsDeleted = true;
        response.Success = await _waitingTokenRepository.UpdateAsync(token);
        response.Message = response.Success ? NotificationMessages.Deleted.Replace("{0}", "Waiting Token") : NotificationMessages.DeletedFailed.Replace("{0}", "Waiting Token");
        return response;
    }

    #endregion Delete

}
