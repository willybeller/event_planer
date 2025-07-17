using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPlannerAPI.Services;
using EventPlannerAPI.DTOs;
using System.Security.Claims;

namespace EventPlannerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Crée un nouvel événement
    /// </summary>
    /// <param name="createEventDto">Données de l'événement à créer</param>
    /// <returns>Événement créé</returns>
    /// <response code="201">Événement créé avec succès</response>
    /// <response code="400">Données invalides</response>
    /// <response code="401">Authentification requise</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EventDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var result = await _eventService.CreateEventAsync(createEventDto, userId.Value);
        if (result == null)
        {
            return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
    }

    /// <summary>
    /// Récupère la liste des événements accessibles à l'utilisateur
    /// </summary>
    /// <returns>Liste des événements (publics + événements de l'utilisateur si connecté)</returns>
    /// <response code="200">Liste des événements récupérée</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventDto>), 200)]
    public async Task<ActionResult<List<EventDto>>> GetEvents()
    {
        var userId = GetCurrentUserId();
        var events = await _eventService.GetEventsAsync(userId);
        return Ok(events);
    }

    /// <summary>
    /// Récupère les détails d'un événement spécifique
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <returns>Détails de l'événement</returns>
    /// <response code="200">Événement trouvé</response>
    /// <response code="404">Événement non trouvé ou accès refusé</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        var userId = GetCurrentUserId();
        var eventDto = await _eventService.GetEventByIdAsync(id, userId);
        
        if (eventDto == null)
        {
            return NotFound();
        }
        
        return Ok(eventDto);
    }

    /// <summary>
    /// Modifie un événement existant
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <param name="updateEventDto">Nouvelles données de l'événement</param>
    /// <returns>Événement modifié</returns>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<EventDto>> UpdateEvent(int id, [FromBody] UpdateEventDto updateEventDto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var result = await _eventService.UpdateEventAsync(id, updateEventDto, userId.Value);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Supprime un événement
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <returns>Confirmation de suppression</returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var success = await _eventService.DeleteEventAsync(id, userId.Value);
        if (!success)
        {
            return NotFound();
        }
        
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
