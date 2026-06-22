using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Contracts;
using DotnetNiger.UI.Services.Helpers;

namespace DotnetNiger.UI.Services.Mock;

public class EventService : IEventService
{
    private readonly IAuthService _authService;
    private List<EventDto> _events;
    private List<EventRegistrationDto> _registrations;

    private readonly INotificationService _notificationService;

    public EventService(IAuthService authService, INotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;

        _events = new List<EventDto>
        {
            new EventDto
            {
                Id = Guid.NewGuid(),
                Title = ".NET Niger Meetup #1",
                Slug = "dotnet-niger-meetup-1",
                Description = "Premier meetup de la communauté .NET Niger à Niamey. Venez découvrir les nouveautés de .NET 9 et échanger avec les développeurs locaux.",
                Location = "Niamey, Niger",
                EventType = "Physical",
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(10).AddHours(3),
                CoverImageUrl = "/Images/evenement.jpg",
                OrganizerName = "Équipe .NET Niger",
                Capacity = 50,
                RegisteredCount = 18,
                IsPublished = false,
                MeetupLink = "",
                Medias = new List<EventMediaDto>(),

                SubmittedBy = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            SubmittedAt = DateTime.Now.AddDays(-10),
            PublishedAt = DateTime.Now.AddDays(-10)
            },
            new EventDto
            {
                Id = Guid.NewGuid(),
                Title = "Workshop Blazor WebAssembly",
                Slug = "workshop-blazor-webassembly",
                Description = "Atelier pratique sur Blazor WebAssembly : créez votre première application SPA avec .NET.",
                Location = "Online",
                EventType = "Online",
                StartDate = DateTime.Now.AddDays(25),
                EndDate = DateTime.Now.AddDays(25).AddHours(4),
                CoverImageUrl = "/Images/evenement.jpg",
                OrganizerName = "Équipe .NET Niger",
                Capacity = 100,
                RegisteredCount = 42,
                IsPublished = true,
                MeetupLink = "https://meet.example.com/blazor-workshop",
                Medias = new List<EventMediaDto>(),
                SubmittedBy = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            SubmittedAt = DateTime.Now.AddDays(-10),
            PublishedAt = DateTime.Now.AddDays(-10),
            },
            new EventDto
            {
                Id = Guid.NewGuid(),
                Title = "Conférence ASP.NET Core & API REST",
                Slug = "conference-aspnet-core-api-rest",
                Description = "Conception d'API REST robustes avec ASP.NET Core, bonnes pratiques et retours d'expérience.",
                Location = "Niamey — Maison de la Jeunesse",
                EventType = "Hybrid",
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(-5).AddHours(5),
                CoverImageUrl = "/Images/evenement.jpg",
                OrganizerName = "Équipe .NET Niger",
                Capacity = 80,
                RegisteredCount = 80,
                IsPublished = true,
                MeetupLink = "https://meet.example.com/aspnet-conf",
                Medias = new List<EventMediaDto>
                {
                    new EventMediaDto { Id = Guid.NewGuid(), Type = "Image", Url = "/Images/evenement.jpg", Title = "Photo de l'événement" }
                },
                GalleryImageUrls = new List<string> { "/Images/evenement.jpg" }
            }
        };

        _registrations = new List<EventRegistrationDto>();
    }

    // ---- Lecture --------------------------------------------------------------------------------

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.OrderByDescending(e => e.StartDate).ToList());
    }

    public async Task<List<EventDto>> GetPublishedEventsAsync()
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.Where(e => e.IsPublished)
                   .OrderBy(e => e.StartDate)
                   .ToList());
    }

    public async Task<List<EventDto>> GetUpcomingEventsAsync()
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.Where(e => e.IsPublished && e.StartDate >= DateTime.Now)
                   .OrderBy(e => e.StartDate)
                   .ToList());
    }

    public async Task<List<EventDto>> GetPastEventsAsync()
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.Where(e => e.IsPublished && e.EndDate < DateTime.Now)
                   .OrderByDescending(e => e.StartDate)
                   .ToList());
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        await Task.Delay(800);
        var ev = _events.FirstOrDefault(e => e.Id == id);
        return await Task.FromResult(ev);
    }

    public async Task<EventDto?> GetEventBySlugAsync(string slug)
    {
        await Task.Delay(800);
        var ev = _events.FirstOrDefault(e => e.Slug == slug);
        return await Task.FromResult(ev);
    }

    public async Task<List<EventDto>> SearchEventsAsync(string query)
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.Where(e =>
                    e.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    e.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    e.Location.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    e.OrganizerName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.StartDate)
                .ToList());
    }

    public async Task<List<EventDto>> GetEventsByTypeAsync(string eventType)
    {
        await Task.Delay(800);
        return await Task.FromResult(
            _events.Where(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase) && e.IsPublished)
                   .OrderBy(e => e.StartDate)
                   .ToList());
    }

    // ---- Création / Mise à jour / Suppression -------------------------------------------------


    public async Task<EventDto?> CreateEventAsync(CreateEventRequest request, Guid currentUserId, bool isAdmin)
    {
        await Task.Delay(500); // simuler appel API

        var resolvedIsAdmin = isAdmin || await _authService.IsAdminAsync();

        var slug = request.Title.ToLower().Replace(" ", "-");
        var now = DateTime.Now;

        var newEvent = new EventDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            Location = request.Location,
            EventType = request.EventType,
            Category = request.Category,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CoverImageUrl = request.CoverImageUrl ?? "/images/events/default.jpg",
            Capacity = request.Capacity,
            MeetupLink = request.MeetupLink ?? "",
            Medias = request.GalleryImageUrls.Select(url => new EventMediaDto
            {
                Id = Guid.NewGuid(),
                Type = "Image",
                Url = url,
                Title = "Galerie"
            }).ToList(),
            GalleryImageUrls = request.GalleryImageUrls,
            Speakers = request.Speakers?.Select(s => new SpeakerDto
            {
                UserId = s.UserId,
                Name = s.Name,
                Role = s.Role,
                AvatarUrl = s.AvatarUrl
            }).ToList() ?? new(),
            CreatedBy = currentUserId,
            OrganizerName = (await _authService.GetCurrentUserAsync())?.FullName ?? "Organisateur",
            RegisteredCount = 0,
            SubmittedBy = currentUserId,
            SubmittedAt = now
        };

        if (resolvedIsAdmin)
        {
            // Admin : publication immédiate
            newEvent.IsPublished = true;
            newEvent.PublishedAt = now;
        }
        else
        {
            // Membre : en attente de validation
            newEvent.IsPublished = false;
            newEvent.PublishedAt = null;
        }

        _events.Add(newEvent);
        return newEvent;
    }

      public async Task<List<EventDto>> GetPendingEventsAsync()
    {
        await Task.Delay(800);
        return _events.Where(e => !e.IsPublished).ToList();
    }

    public async Task<bool> ApproveEventAsync(Guid eventId, string? adminComment = null)
    {
        await Task.Delay(500);
        var evt = _events.FirstOrDefault(e => e.Id == eventId);
        if (evt == null || evt.IsPublished) return false;

        evt.IsPublished = true;
        evt.PublishedAt = DateTime.Now;
        // Optionnel : stocker le commentaire admin (à ajouter dans DTO si besoin)
        return true;
    }

    public async Task<bool> RejectEventAsync(Guid eventId, string reason)
    {
        await Task.Delay(500);
        var evt = _events.FirstOrDefault(e => e.Id == eventId);
        if (evt == null || evt.IsPublished) return false;

        // Stocker la raison puis supprimer l'événement rejeté des listes visibles
        evt.RejectionReason = reason;

        // notifier l'auteur de l'événement
        var submitterId = evt.SubmittedBy;
        var message = $"Votre événement '{evt.Title}' a été rejeté : {reason}";
        await _notificationService.SendNotificationAsync(submitterId, message);

        _events.Remove(evt);

        // Supprimer aussi les inscriptions associées
        _registrations.RemoveAll(r => r.EventId == eventId);

        return true;
    }

    public async Task<List<EventDto>> GetEventsBySubmitterAsync(Guid userId)
    {
        await Task.Delay(800);
        return _events.Where(e => e.SubmittedBy == userId).OrderByDescending(e => e.SubmittedAt).ToList();
    }



    public async Task<EventDto?> UpdateEventAsync(Guid id, CreateEventRequest request)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev is null) return await Task.FromResult<EventDto?>(null);

        ev.Title = request.Title;
        ev.Slug = GenerateSlug(request.Title);
        ev.Description = request.Description;
        ev.Location = request.Location;
        ev.EventType = request.EventType;
        ev.Category = request.Category;
        ev.StartDate = request.StartDate;
        ev.EndDate = request.EndDate;
        ev.CoverImageUrl = request.CoverImageUrl;
        ev.Capacity = request.Capacity;
        ev.MeetupLink = request.MeetupLink;
        ev.GalleryImageUrls = request.GalleryImageUrls;
        ev.Medias = request.GalleryImageUrls.Select(url => new EventMediaDto
        {
            Id = Guid.NewGuid(),
            Type = "Image",
            Url = url,
            Title = "Galerie"
        }).ToList();

        ev.Speakers = request.Speakers?.Select(s => new SpeakerDto
        {
            UserId = s.UserId,
            Name = s.Name,
            Role = s.Role,
            AvatarUrl = s.AvatarUrl
        }).ToList() ?? new();

        return await Task.FromResult<EventDto?>(ev);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev is null) return await Task.FromResult(false);

        _events.Remove(ev);
        return await Task.FromResult(true);
    }

    public async Task<bool> TogglePublishAsync(Guid id)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev is null) return await Task.FromResult(false);

        ev.IsPublished = !ev.IsPublished;
        return await Task.FromResult(true);
    }

    // -- Inscriptions -------------------------------------------

    public async Task<EventRegistrationDto?> RegisterToEventAsync(RegisterEventRequest request, Guid userId, string userName)
    {
        var ev = _events.FirstOrDefault(e => e.Id == request.EventId);
        if (ev is null || ev.RegisteredCount >= ev.Capacity)
            return await Task.FromResult<EventRegistrationDto?>(null);

        var alreadyRegistered = _registrations.Any(r => r.EventId == request.EventId && r.UserId == userId);
        if (alreadyRegistered)
            return await Task.FromResult<EventRegistrationDto?>(null);

        var registration = new EventRegistrationDto
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            EventTitle = ev.Title,
            UserId = userId,
            UserName = userName,
            AvatarUrl = request.AvatarUrl,
            RegisteredAt = DateTime.Now,
            IsAttended = false,
            RegistrationStatus = "Confirmed"
        };

        _registrations.Add(registration);
        ev.RegisteredCount++;

        return await Task.FromResult<EventRegistrationDto?>(registration);
    }

    public async Task<bool> CancelRegistrationAsync(Guid eventId, Guid userId)
    {
        var reg = _registrations.FirstOrDefault(r => r.EventId == eventId && r.UserId == userId);
        if (reg is null) return await Task.FromResult(false);

        _registrations.Remove(reg);
        var ev = _events.FirstOrDefault(e => e.Id == eventId);
        if (ev is not null) ev.RegisteredCount--;

        return await Task.FromResult(true);
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsByEventAsync(Guid eventId)
    {
        await Task.Delay(800);
        var registrations = _registrations
            .Where(r => r.EventId == eventId)
            .ToList();
        return await Task.FromResult(registrations);
    }

    private static string GenerateSlug(string title)
        => StringHelper.GenerateSlug(title);
}
