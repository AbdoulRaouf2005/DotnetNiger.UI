using DotnetNiger.UI.Helpers;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiEventService : ApiServiceBase, IEventService
{
    public ApiEventService(HttpClient http) : base(http)
    {
    }

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        return await GetCollectionAsync<EventDto>(ApiEndpoints.Events);
    }

    public async Task<List<EventDto>> GetPublishedEventsAsync()
    {
        var events = await GetCollectionAsync<EventDto>(ApiEndpoints.Events, new Dictionary<string, string?>
        {
            ["published"] = "true"
        });

        return events.Where(e => e.IsPublished).ToList();
    }

    public async Task<List<EventDto>> GetUpcomingEventsAsync()
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Events}/upcoming?page=1&pageSize=10");
        if (!response.IsSuccessStatusCode)
            return new List<EventDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventDto>(response);
    }

    public async Task<List<EventDto>> GetPastEventsAsync()
    {
        var events = await GetCollectionAsync<EventDto>(ApiEndpoints.Events, new Dictionary<string, string?>
        {
            ["past"] = "true"
        });

        return events.Where(e => e.EndDate < DateTime.Now).OrderByDescending(e => e.StartDate).ToList();
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Events}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<EventDto?> GetEventBySlugAsync(string slug)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Events}/{slug}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<List<EventDto>> SearchEventsAsync(string query)
    {
        return await GetCollectionAsync<EventDto>(ApiEndpoints.Events, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = "1",
            ["pageSize"] = "100"
        });
    }

    public async Task<List<EventDto>> GetEventsByTypeAsync(string eventType)
    {
        var events = await GetCollectionAsync<EventDto>(ApiEndpoints.Events, new Dictionary<string, string?>
        {
            ["eventType"] = eventType
        });

        return events.Where(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<EventDto?> CreateEventAsync(CreateEventRequest request, Guid currentUserId, bool isAdmin)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Events, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<EventDto?> UpdateEventAsync(Guid id, CreateEventRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Events}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Events}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> TogglePublishAsync(Guid id)
    {
        var current = await GetEventByIdAsync(id);
        if (current is null)
            return false;

        var endpoint = current.IsPublished
            ? $"{ApiEndpoints.AdminEvents}/{id}/unpublish"
            : $"{ApiEndpoints.AdminEvents}/{id}/publish";

        var response = await Http.PatchAsync(endpoint, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<EventRegistrationDto?> RegisterToEventAsync(RegisterEventRequest request, Guid userId, string userName)
    {
        var response = await Http.PostAsJsonAsync($"{ApiEndpoints.Events}/registrations", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventRegistrationDto>(response);
    }

    public async Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Events}/{eventId}/registrations");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsByEventAsync(Guid eventId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Events}/{eventId}/registrations");
        if (!response.IsSuccessStatusCode)
            return new List<EventRegistrationDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventRegistrationDto>(response);
    }

    public async Task<List<EventDto>> GetPendingEventsAsync()
    {
        var response = await Http.GetAsync($"{ApiEndpoints.AdminEvents}?status=pending");
        if (!response.IsSuccessStatusCode)
            return new List<EventDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventDto>(response);
    }

    public async Task<bool> ApproveEventAsync(Guid eventId, string? adminComment = null)
    {
        var endpoint = string.IsNullOrWhiteSpace(adminComment)
            ? $"{ApiEndpoints.AdminEvents}/{eventId}/approve"
            : $"{ApiEndpoints.AdminEvents}/{eventId}/approve?comment={Uri.EscapeDataString(adminComment)}";

        var response = await Http.PatchAsync(endpoint, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RejectEventAsync(Guid eventId, string reason)
    {
        var endpoint = $"{ApiEndpoints.AdminEvents}/{eventId}/reject?reason={Uri.EscapeDataString(reason)}";
        var response = await Http.PatchAsync(endpoint, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<EventDto>> GetEventsBySubmitterAsync(Guid userId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Events}?submitterId={userId}");
        if (!response.IsSuccessStatusCode)
            return new List<EventDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventDto>(response);
    }

}
