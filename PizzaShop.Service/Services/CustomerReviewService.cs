using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CustomerReviewService : ICustomerReviewService
{
    private readonly IGenericRepository<CustomersReview> _customersReviewRepository;
    private readonly IUserService _userService;

    public CustomerReviewService(IGenericRepository<CustomersReview> customersReviewRepository, IUserService userService)
    {
        _customersReviewRepository = customersReviewRepository;
        _userService = userService;

    }

    public async Task<ResponseViewModel> Save(CustomerReviewViewModel reviewVM)
    {
        ResponseViewModel response = new();

        CustomersReview review = await _customersReviewRepository.GetByIdAsync(reviewVM.Id)
                                ?? new CustomersReview
                                {
                                    OrderId = reviewVM.OrderId,
                                    CustomerId = reviewVM.CustomerId,
                                    CreatedBy = await _userService.LoggedInUser()
                                };

        review.FoodRating = reviewVM.FoodRating;
        review.EnvRating = reviewVM.ServiceRating;
        review.AmbienceRating = reviewVM.AmbienceRating;
        review.Review = reviewVM.Comment;

        review.Rating = (int)((reviewVM.FoodRating + reviewVM.AmbienceRating + reviewVM.ServiceRating) / 3);

        if (reviewVM.Id == 0)
        {
            await _customersReviewRepository.AddAsync(review);
            response.Success = true;
            response.Message = NotificationMessages.Added.Replace("{0}","Review");
        }
        else
        {
            await _customersReviewRepository.UpdateAsync(review);
            response.Success = true;
            response.Message = NotificationMessages.Updated.Replace("{0}","Review");
        }
        return response;
    }
}
