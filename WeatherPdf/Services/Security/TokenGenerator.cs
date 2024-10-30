using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherPdf.Services.Security;

public class TokenGenerator
{
    private readonly IConfiguration _config;
    private readonly byte[] _key;

    public TokenGenerator(IConfiguration config)
    {
        _config = config;
        _key = Encoding.UTF8.GetBytes(_config.GetValue<string>("Secret")!);
    }

    public string GenerateToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub,email),
            new(JwtRegisteredClaimNames.Email, email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = _config.GetSection("BearerSection:ValidIssuer").Value,
            Audience = _config.GetSection("BearerSection:ValidAudience").Value,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
