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
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;
    private readonly ISectionService _sectionService;

    public WaitingListService(IGenericRepository<WaitingToken> waitingTokenRepository, IUserService userService, ICustomerService customerService, ISectionService sectionService)
    {
        _waitingTokenRepository = waitingTokenRepository;
        _userService = userService;
        _customerService = customerService;
        _sectionService = sectionService;
    }

    #region Get

    // public async Task<WaitingToken> Get(long tokenId)
    // {

    //     if (tokenId == 0)
    // }


    public async Task<List<WaitingTokenViewModel>> List(long sectionId)
    {
        try
        {
            IEnumerable<WaitingToken>? list = await _waitingTokenRepository.GetByCondition(
                predicate: wt => !wt.IsDeleted
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
                if (response.EntityId > 0)
                {
                    response.Success = true;
                    response.Message = NotificationMessages.Added.Replace("{0}", "Waiting Token");
                }
                else
                {
                    response.Success = false;
                    response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Waiting Token");
                }
            }
            else
            {
                if (await _waitingTokenRepository.UpdateAsync(token))
                {
                    response.Success = true;
                    response.Message = NotificationMessages.Updated.Replace("{0}", "Waiting Token");
                }
                else
                {
                    response.Success = false;
                    response.Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Waiting Token");
                }
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
        if (await _waitingTokenRepository.UpdateAsync(token))
        {
            response.Success = true;
            response.Message = NotificationMessages.Deleted.Replace("{0}", "Waiting Token");
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.DeletedFailed.Replace("{0}", "Waiting Token");
        }

        return response;

    }

    #endregion Delete

}
