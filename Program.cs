using DotnetNiger.UI;
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
builder.Services.AddScoped<ClientIdentifierProvider>();

builder.Services.AddScoped<AuthService>(sp => new AuthService(
    CreateGatewayHttpClient(
        apiBaseUrl,
        sp.GetRequiredService<ClientIdentifierProvider>(),
        sp.GetRequiredService<CustomAuthStateProvider>(),
        sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>()),
    sp.GetRequiredService<CustomAuthStateProvider>()
));

// Auth
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

// Services applicatifs — basculer entre Mock et API via "UseMockServices" dans appsettings.json
var useMock = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMock)
{
    builder.Services.AddScoped<IAuthService, MockAuthService>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<IResourceService, ResourceService>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IRegistrationService, MockRegistrationService>();
}
else
{
    builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<AuthService>());
    builder.Services.AddScoped<IPostService>(sp =>
        new ApiPostService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<IEventService>(sp =>
        new ApiEventService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));
    builder.Services.AddScoped<IResourceService>(sp =>
        new ApiResourceService(CreateGatewayHttpClient(
            apiBaseUrl,
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<CustomAuthStateProvider>(),
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>())));

    builder.Services.AddScoped<IProfileService>(sp =>
        new ApiProfileService(
            CreateGatewayHttpClient(
                apiBaseUrl,
                sp.GetRequiredService<ClientIdentifierProvider>(),
                sp.GetRequiredService<CustomAuthStateProvider>(),
                sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>()),
            sp.GetRequiredService<CustomAuthStateProvider>()));

    builder.Services.AddScoped<IRegistrationService, MockRegistrationService>();
}

await builder.Build().RunAsync();

static HttpClient CreateGatewayHttpClient(
    string baseUrl,
    ClientIdentifierProvider clientIdentifierProvider,
    CustomAuthStateProvider authStateProvider,
    ILogger<ClientIdHeaderHandler> logger)
{
    var headerHandler = new ClientIdHeaderHandler(clientIdentifierProvider, authStateProvider, logger)
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(headerHandler)
    {
        BaseAddress = new Uri(baseUrl)
    };
}
