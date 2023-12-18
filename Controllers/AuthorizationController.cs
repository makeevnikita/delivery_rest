using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using delivery.Helpers;
using System.Security.Claims;



namespace delivery.Controllers;

[ApiController]
public class AuthorizationController : Controller
{
    public AuthorizationController()
    {

    }

    [HttpGet("login")]
    public string Login([FromQuery] string username)
    {
        List<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Name, username) };

        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthHelper.GetKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}