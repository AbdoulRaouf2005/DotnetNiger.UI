using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IEventService
{
    Task<List<EventDto>> GetAllEventsAsync();
    Task<List<EventDto>> GetPublishedEventsAsync();
    Task<List<EventDto>> GetUpcomingEventsAsync();
    Task<List<EventDto>> GetPastEventsAsync();
    Task<EventDto?> GetEventByIdAsync(Guid id);
    Task<EventDto?> GetEventBySlugAsync(string slug);
    Task<List<EventDto>> SearchEventsAsync(string query);
    Task<List<EventDto>> GetEventsByTypeAsync(string eventType);
    // Task<EventDto> CreateEventAsync(CreateEventRequest request);
    Task<EventDto?> UpdateEventAsync(Guid id, CreateEventRequest request);
    Task<bool> DeleteEventAsync(Guid id);
    Task<bool> TogglePublishAsync(Guid id);
    Task<EventRegistrationDto?> RegisterToEventAsync(RegisterEventRequest request, Guid userId, string userName);
    Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId);
    Task<List<EventRegistrationDto>> GetRegistrationsByEventAsync(Guid eventId);

     // Création d'événement (soumis ou publié selon rôle)
    Task<EventDto?> CreateEventAsync(CreateEventRequest request, Guid currentUserId, bool isAdmin);

    // Admin : récupérer les événements en attente
    Task<List<EventDto>> GetPendingEventsAsync();

    // Admin : approuver un événement
    Task<bool> ApproveEventAsync(Guid eventId, string? adminComment = null);

    // Admin : rejeter un événement avec motif
    Task<bool> RejectEventAsync(Guid eventId, string reason);

    // Récupérer les événements créés par un utilisateur (ses soumissions)
    Task<List<EventDto>> GetEventsBySubmitterAsync(Guid userId);
}
