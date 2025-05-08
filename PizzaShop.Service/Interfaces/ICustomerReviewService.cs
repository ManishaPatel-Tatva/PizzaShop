using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerReviewService
{
    Task<ResponseViewModel> Save(CustomerReviewViewModel review);
}
