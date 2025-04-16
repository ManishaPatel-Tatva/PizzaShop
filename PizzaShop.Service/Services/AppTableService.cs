using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppTableService : IAppTableService
{
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;

    public AppTableService(IGenericRepository<Section> sectionRepository, IGenericRepository<WaitingToken> waitingTokenRepository, IUserService userService, ICustomerService customerService)
    {
        _sectionRepository = sectionRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _userService = userService;
        _customerService = customerService;
    }

    #region  Get

    public async Task<List<SectionViewModel>> Get()
    {
        try
        {
            IEnumerable<Section>? list = await _sectionRepository.GetByCondition(
                s => !s.IsDeleted,
                thenIncludes: new List<Func<IQueryable<Section>, IQueryable<Section>>>
                {
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.Status),
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.OrderTableMappings)
                    .ThenInclude(otm => otm.Order)
                }
            );

            List<SectionViewModel> sections = list.Select(s => new SectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Tables = s.Tables.Select(t => new TableCardViewModel
                {
                    Id = t.Id,
                    TableName = t.Name,
                    TableStatus = t.Status.Name,
                    OrderAmount = t.OrderTableMappings
                                .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                                .Select(otm => otm.Order.FinalAmount)
                                .FirstOrDefault(),
                    OrderTime = t.OrderTableMappings
                                .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                                .Select(otm => otm.CreatedAt)
                                .FirstOrDefault(),
                    Capacity = t.Capacity
                }).ToList()
            }).ToList();

            return sections;
        }
        catch (Exception ex)
        {
            return null;
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

            if (wtokenVM.Id == 0)
            {
                if (await _waitingTokenRepository.AddAsync(token))
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

}
