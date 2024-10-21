using ChatBackend;

public class UserService
{
    private readonly FirebaseService _firebaseService;

    public UserService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    // Method to retrieve all users
    public async Task<List<Users>> GetAllUsersAsync()
    {
        return await _firebaseService.GetAllUsersAsync();
    }

    // Method to get a specific user by ID
    public async Task<Users> GetUserByIdAsync(string userId)
    {
        return await _firebaseService.GetUserAsync(userId);
    }

    // Additional methods related to user logic can be added here
}
