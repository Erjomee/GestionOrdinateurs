
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken(
        [FromBody] LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient("UsersApi");

        var response = await client.GetAsync(
            $"users/{Uri.EscapeDataString(request.Login)}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Unauthorized("Utilisateur inconnu.");

        if (!response.IsSuccessStatusCode)
            return StatusCode(502, "Erreur lors de l'appel à l'API utilisateurs.");

        var user = await response.Content.ReadFromJsonAsync<User>();

        if (user == null)
            return StatusCode(502, "Utilisateur invalide.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = _configuration["Jwt:Key"]!;
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires = token.ValidTo
        });
    }
}
