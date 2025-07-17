using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPlannerAPI.Services;
using EventPlannerAPI.DTOs;
using System.Security.Claims;

namespace EventPlannerAPI.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Inscription d'un nouvel utilisateur
    /// </summary>
    /// <param name="registerDto">Données d'inscription (nom, email, mot de passe)</param>
    /// <returns>Token JWT et informations utilisateur</returns>
    /// <response code="200">Inscription réussie</response>
    /// <response code="400">Email déjà utilisé ou données invalides</response>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        if (result == null)
        {
            return BadRequest("Email already exists or registration failed");
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Connexion utilisateur
    /// </summary>
    /// <param name="loginDto">Données de connexion (email, mot de passe)</param>
    /// <returns>Token JWT et informations utilisateur</returns>
    /// <response code="200">Connexion réussie</response>
    /// <response code="401">Email ou mot de passe incorrect</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
        {
            return Unauthorized();
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Récupère les informations du profil de l'utilisateur connecté
    /// </summary>
    /// <returns>Informations utilisateur</returns>
    /// <response code="200">Profil utilisateur récupéré</response>
    /// <response code="401">Token manquant ou invalide</response>
    /// <response code="404">Utilisateur non trouvé</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var userDto = await _authService.GetUserByIdAsync(userId.Value);
        if (userDto == null)
        {
            return NotFound();
        }
        
        return Ok(userDto);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
