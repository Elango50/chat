using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

public class FirebaseService
{
    private readonly FirebaseApp _firebaseApp;

    public FirebaseService(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var serviceAccountPath = configuration["Firebase:ServiceAccountFile"];
            _firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath),
            });
        }
    }

    // Method to verify the Firebase Token
    public async Task<(bool IsValid, FirebaseToken Token, string ErrorMessage)> VerifyFirebaseTokenAsync(string token)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            return (true, decodedToken, null);
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Firebase token verification failed: {ex.Message}");
            return (false, null, ex.Message); // Return error message
        }
    }

    public async Task<Users> GetUserAsync(string id)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserAsync(id);
            return new Users
            {
                userId = user.Uid,
                email = user.Email,
                // Add other user properties as needed
            };
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Failed to retrieve user: {ex.Message}");
            return null; // User not found or error occurred
        }
    }

    public async Task<UserRecord> CreateUserAsync(Users user)
    {
              var userRecordArgs = new UserRecordArgs
        {
            Email = user.email,
            // Set other properties such as password, display name, etc.
        };

        try
        {
            return await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Failed to create user: {ex.Message}");
            return null; // Handle user creation errors
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        try
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(id);
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Failed to delete user: {ex.Message}");
            // Handle deletion errors as needed
        }
    }

    public async Task<List<Users>> GetAllUsersAsync()
    {
       var allUsers = new List<Users>();
        var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
        var responses = pagedEnumerable.AsRawResponses().GetAsyncEnumerator();

        while (await responses.MoveNextAsync())
        {
            var response = responses.Current;
            foreach (var user in response.Users)
            {
                allUsers.Add(new Users
                {
                    userId = user.Uid,
                    email = user.Email,
                    // Add other user properties as needed
                });
            }
        }
        return allUsers;
    }
}
