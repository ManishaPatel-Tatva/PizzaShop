namespace DataAccessLayer.ViewModels;
public class UserInfoViewModel
{
    public long UserId {get; set;}
    
    public string ProfileImageUrl { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; } = null!;

    public long Phone { get; set; } 
    
    public string Role { get; set; } =null!;

    public bool? Status { get; set; }

    public bool IsDeleted { get; set; }
    
}