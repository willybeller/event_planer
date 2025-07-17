using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventPlannerAPI.Services;
using EventPlannerAPI.DTOs;
using System.Security.Claims;

namespace EventPlannerAPI.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
[Tags("Participants")]
[Authorize]
public class ParticipantsController : ControllerBase
{
    private readonly IParticipantService _participantService;

    public ParticipantsController(IParticipantService participantService)
    {
        _participantService = participantService;
    }

    /// <summary>
    /// Invite un participant à un événement
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <param name="inviteDto">Email de la personne à inviter</param>
    /// <returns>Confirmation d'invitation</returns>
    /// <response code="200">Invitation envoyée avec succès</response>
    /// <response code="400">Échec de l'invitation (email déjà invité, etc.)</response>
    /// <response code="401">Authentification requise</response>
    [HttpPost("{id:int}/invite")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<string>> InviteParticipant(int id, [FromBody] InviteDto inviteDto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var success = await _participantService.InviteParticipantAsync(id, inviteDto, userId.Value);
        if (!success)
        {
            return BadRequest("Failed to send invitation");
        }
        
        return Ok("Invitation sent");
    }

    /// <summary>
    /// Rejoint un événement (public ou via invitation)
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <param name="inviteEmail">Email d'invitation (optionnel)</param>
    /// <returns>Confirmation de participation</returns>
    [HttpPost("{id:int}/join")]
    public async Task<ActionResult<string>> JoinEvent(int id, [FromQuery] string? inviteEmail = null)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var success = await _participantService.JoinEventAsync(id, userId.Value, inviteEmail);
        if (!success)
        {
            return BadRequest("Failed to join event");
        }
        
        return Ok("Successfully joined event");
    }

    /// <summary>
    /// Met à jour le statut RSVP d'un participant
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <param name="rsvpDto">Nouveau statut RSVP</param>
    /// <returns>Confirmation de mise à jour</returns>
    [HttpPatch("{id:int}/rsvp")]
    public async Task<ActionResult<string>> UpdateRsvp(int id, [FromBody] RsvpDto rsvpDto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var success = await _participantService.UpdateRsvpAsync(id, rsvpDto, userId.Value);
        if (!success)
        {
            return BadRequest("Failed to update RSVP");
        }
        
        return Ok("RSVP updated");
    }

    /// <summary>
    /// Récupère la liste des participants d'un événement
    /// </summary>
    /// <param name="id">ID de l'événement</param>
    /// <returns>Liste des participants</returns>
    [HttpGet("{id:int}/participants")]
    public async Task<ActionResult<List<ParticipantDto>>> GetEventParticipants(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }
        
        var participants = await _participantService.GetEventParticipantsAsync(id, userId.Value);
        return Ok(participants);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
