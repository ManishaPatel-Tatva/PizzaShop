namespace PizzaShop.Entity.ViewModels;

public class CustomerReviewViewModel
{
    public long CustomerId { get; set; } = 0;
    public long OrderId { get; set; } = 0;
    public int FoodRating { get; set; } = 0;
    public int ServiceRating { get; set; } = 0;
    public int AmbienceRating { get; set; } = 0;
    public string Comment { get; set; } = "";

}
