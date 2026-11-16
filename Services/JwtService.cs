using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BibliotecaApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaApi.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Uuid),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim("role", role.Name));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpirationMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}