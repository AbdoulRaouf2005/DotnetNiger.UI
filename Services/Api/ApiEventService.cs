using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiEventService : IEventService
{
    private readonly HttpClient _http;
    private const string PublicBase = "api/v1/events";
    private const string AdminBase = "api/v1/admin/events";
    private const string SearchBase = "api/v1/search";

    public ApiEventService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        return await GetCollectionAsync<EventDto>(PublicBase);
    }

    public async Task<List<EventDto>> GetPublishedEventsAsync()
    {
        var events = await GetCollectionAsync<EventDto>(PublicBase, new Dictionary<string, string?>
        {
            ["published"] = "true"
        });

        return events.Where(e => e.IsPublished).ToList();
    }

    public async Task<List<EventDto>> GetUpcomingEventsAsync()
    {
        var response = await _http.GetAsync($"{PublicBase}/upcoming?page=1&pageSize=10");
        if (!response.IsSuccessStatusCode)
            return new List<EventDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventDto>(response);
    }

    public async Task<List<EventDto>> GetPastEventsAsync()
    {
        var events = await GetCollectionAsync<EventDto>(PublicBase, new Dictionary<string, string?>
        {
            ["past"] = "true"
        });

        return events.Where(e => e.EndDate < DateTime.Now).OrderByDescending(e => e.StartDate).ToList();
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{PublicBase}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<EventDto?> GetEventBySlugAsync(string slug)
    {
        var events = await SearchEventsAsync(slug);
        var bySlug = events.FirstOrDefault(e => e.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        if (bySlug is not null)
            return bySlug;

        events = await GetAllEventsAsync();
        return events.FirstOrDefault(e => e.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<EventDto>> SearchEventsAsync(string query)
    {
        var searchResults = await GetCollectionAsync<SearchResultDto>(SearchBase, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["type"] = "Event",
            ["page"] = "1",
            ["pageSize"] = "100"
        });

        var ids = searchResults
            .Where(r => r.Type.Equals("Event", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        if (ids.Count > 0)
        {
            var fetchTasks = ids.Select(GetEventByIdAsync);
            var eventsByIds = await Task.WhenAll(fetchTasks);
            return eventsByIds.Where(e => e is not null).Select(e => e!).ToList();
        }

        var events = await GetCollectionAsync<EventDto>(PublicBase, new Dictionary<string, string?>
        {
            ["query"] = query
        });

        return events.Where(e =>
                e.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<List<EventDto>> GetEventsByTypeAsync(string eventType)
    {
        var events = await GetCollectionAsync<EventDto>(PublicBase, new Dictionary<string, string?>
        {
            ["eventType"] = eventType
        });

        return events.Where(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<EventDto> CreateEventAsync(CreateEventRequest request)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<EventDto>(response)
               ?? throw new InvalidOperationException("La réponse API est vide pour la création de l'événement.");
    }

    public async Task<EventDto?> UpdateEventAsync(Guid id, CreateEventRequest request)
    {
        var response = await _http.PutAsJsonAsync($"{PublicBase}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventDto>(response);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{PublicBase}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> TogglePublishAsync(Guid id)
    {
        var current = await GetEventByIdAsync(id);
        if (current is null)
            return false;

        var endpoint = current.IsPublished
            ? $"{AdminBase}/{id}/unpublish"
            : $"{AdminBase}/{id}/publish";

        var response = await _http.PatchAsync(endpoint, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<EventRegistrationDto?> RegisterToEventAsync(RegisterEventRequest request, Guid userId, string userName)
    {
        var response = await _http.PostAsJsonAsync("api/events/registrations", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<EventRegistrationDto>(response);
    }

    public async Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId)
    {
        var response = await _http.DeleteAsync($"api/events/{eventId}/registrations");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsByEventAsync(Guid eventId)
    {
        var response = await _http.GetAsync($"api/events/{eventId}/registrations");
        if (!response.IsSuccessStatusCode)
            return new List<EventRegistrationDto>();

        return await ApiResponseReader.ReadCollectionAsync<EventRegistrationDto>(response);
    }

    private async Task<List<T>> GetCollectionAsync<T>(string path, Dictionary<string, string?>? query = null)
    {
        var url = BuildUrl(path, query);
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new List<T>();

        return await ApiResponseReader.ReadCollectionAsync<T>(response);
    }

    private static string BuildUrl(string path, Dictionary<string, string?>? query = null)
    {
        if (query is null || query.Count == 0)
            return path;

        var queryString = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }
}
