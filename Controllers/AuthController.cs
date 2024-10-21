using ChatBackend.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FirebaseService _firebaseService;
    private AuthService _authService;

    public AuthController(FirebaseService firebaseService, AuthService authService)
    {
        _firebaseService = firebaseService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] FirebaseLoginRequest request)
    {
        var firebaseTokenResult = await _firebaseService.VerifyFirebaseTokenAsync(request.FirebaseToken);

        if (firebaseTokenResult.IsValid) // Check if token validation was successful
        {
            var jwtToken = _authService.GenerateJwtToken(firebaseTokenResult.Token.Uid); // Use the Uid from the validated Firebase token
            return Ok(new { token = jwtToken });
        } else {
            return Unauthorized(firebaseTokenResult.ErrorMessage); // Return 401 if token validation fails
        }

    }
}
