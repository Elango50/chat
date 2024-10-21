using System.ComponentModel.DataAnnotations;

public class Users
{
    [Key]
    public String userId { get; set; }  // Keep property names in lowercase

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    public string username { get; set; } = string.Empty; // Default to an empty string

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? email { get; set; } // Nullable email property

    public string? profilePicture { get; set; } // Optional profile picture URL

    public DateTime createdDate { get; set; } = DateTime.UtcNow; // Default to current UTC time
}
