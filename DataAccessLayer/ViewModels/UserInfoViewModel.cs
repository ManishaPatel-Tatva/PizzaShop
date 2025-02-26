namespace DataAccessLayer.ViewModels;
public class UserInfoViewModel
{
    public string ProfileImageUrl { get; set; } = "~/images/Default_pfp.svg.png";

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; } = null!;

    public long Phone { get; set; } 
    
    public string Role { get; set; } =null!;

    public bool? Status { get; set; }
    
}