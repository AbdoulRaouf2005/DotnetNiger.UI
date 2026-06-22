using DotnetNiger.UI;
using DotnetNiger.UI.Services.Browser;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Api;
using DotnetNiger.UI.Services.Mock;
using DotnetNiger.UI.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Client HTTP pour les ressources statiques de l'application
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Client HTTP dédié pour AuthService — configurez ApiBaseUrl dans wwwroot/appsettings.json
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
var clientId = builder.Configuration["ClientId"] ?? "web-ui";
builder.Services.AddScoped<ClientIdentifierProvider>();

builder.Services.AddScoped<AuthService>(sp => new AuthService(
    CreateGatewayHttpClient(
        apiBaseUrl,
        sp.GetRequiredService<ClientIdentifierProvider>(),
        sp.GetRequiredService<CustomAuthStateProvider>(),
        sp,
        sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>()),
    sp.GetRequiredService<CustomAuthStateProvider>(),
    sp.GetRequiredService<IUserStateService>(),
    clientId
));

// Auth
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("ModeratorOrAbove", policy =>
        policy.RequireRole("Admin", "SuperAdmin", "Moderator"));
});
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ILocalStorageService, JsLocalStorageService>();

// Theme
builder.Services.AddScoped<ThemeService>();

// Services applicatifs — basculer entre Mock et API via "UseMockServices" dans appsettings.json
var useMock = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMock)
{
    builder.Services.AddScoped<IToastService, ToastService>();
    builder.Services.AddScoped<IUploadService, MockUploadService>();
    builder.Services.AddScoped<IAuthService, MockAuthService>();
    builder.Services.AddScoped<IUserService, MockUserService>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IResourceService, ResourceService>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IRegistrationService, MockRegistrationService>();
    builder.Services.AddScoped<IUserStateService, UserStateService>();
    builder.Services.AddScoped<IProjectService, MockProjectService>();
    builder.Services.AddScoped<IPartnerService, MockPartnerService>();
    builder.Services.AddScoped<INewsletterService, MockNewsletterService>();
    builder.Services.AddScoped<IMemberDirectoryService, MockMemberDirectoryService>();
    builder.Services.AddScoped<ISearchService, MockSearchService>();
    builder.Services.AddScoped<IContactService, MockContactService>();
}
else
{
    builder.Services.AddScoped<IToastService, ToastService>();
    builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<AuthService>());
    builder.Services.AddScoped<IUserService>(sp =>
        new ApiUserService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<IPostService>(sp =>
        new ApiPostService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<IEventService>(sp =>
        new ApiEventService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<IResourceService>(sp =>
        new ApiResourceService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IProfileService>(sp =>
        new ApiProfileService(
            CreateGatewayHttpClient(
                apiBaseUrl,
                sp.GetRequiredService<ClientIdentifierProvider>(),
                sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<ICommentService>(sp =>
        new ApiCommentService(
            CreateGatewayHttpClient(
                apiBaseUrl,
                sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>()),
            sp.GetRequiredService<CustomAuthStateProvider>()));

    builder.Services.AddScoped<IRegistrationService>(sp =>
        new ApiRegistrationService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<INotificationService>(sp =>
        new ApiNotificationService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IContactService>(sp =>
        new ApiContactService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IProjectService>(sp =>
        new ApiProjectService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IPartnerService>(sp =>
        new ApiPartnerService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<INewsletterService>(sp =>
        new ApiNewsletterService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IMemberDirectoryService>(sp =>
        new ApiMemberDirectoryService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<ISearchService>(sp =>
        new ApiSearchService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IUserStateService, UserStateService>();

    builder.Services.AddScoped<IUploadService>(sp =>
        new ApiUploadService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp,
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
}

await builder.Build().RunAsync();

static HttpClient CreateGatewayHttpClient(
    string baseUrl,
    ClientIdentifierProvider clientIdentifierProvider,
    CustomAuthStateProvider authStateProvider,
    IServiceProvider serviceProvider,
    ILogger<ClientIdHeaderHandler> logger)
{
    var headerHandler = new ClientIdHeaderHandler(clientIdentifierProvider, authStateProvider, serviceProvider, logger)
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(headerHandler)
    {
        BaseAddress = new Uri(baseUrl)
    };
}
