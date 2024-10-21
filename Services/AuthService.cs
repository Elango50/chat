using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;

    public AuthService(IConfiguration configuration)
    {
        _jwtSecret = configuration["JwtSettings:Secret"];
        _jwtIssuer = configuration["JwtSettings:Issuer"];
    }

    public string GenerateJwtToken(string uid)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, uid),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtIssuer,
            _jwtIssuer,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
