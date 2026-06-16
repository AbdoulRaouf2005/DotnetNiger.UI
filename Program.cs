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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped<ClientIdentifierProvider>();

var useMock = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMock)
{
    builder.Services.AddScoped<MockAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider>(
        sp => sp.GetRequiredService<MockAuthenticationStateProvider>());

    builder.Services.AddScoped<IToastService, ToastService>();
    builder.Services.AddScoped<IContactService, MockContactService>();
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
    builder.Services.AddScoped<IUserStateService, MockUserStateService>();
    builder.Services.AddScoped<IProjectService, MockProjectService>();
    builder.Services.AddScoped<IPartnerService, MockPartnerService>();
    builder.Services.AddScoped<INewsletterService, MockNewsletterService>();
    builder.Services.AddScoped<IMemberDirectoryService, MockMemberDirectoryService>();
    builder.Services.AddScoped<ISearchService, MockSearchService>();
    builder.Services.AddScoped<IContactService, MockContactService>();
}
else
{
    builder.Services.AddScoped<TokenStorageService>();
    builder.Services.AddTransient<BearerTokenHandler>();

    builder.Services.AddScoped<HttpClient>(sp =>
    {
        var bearer = sp.GetRequiredService<BearerTokenHandler>();
        var clientId = new ClientIdHeaderHandler(
            sp.GetRequiredService<ClientIdentifierProvider>(),
            sp.GetRequiredService<ILogger<ClientIdHeaderHandler>>());
        clientId.InnerHandler = new HttpClientHandler();
        bearer.InnerHandler = clientId;
        return new HttpClient(bearer) { BaseAddress = new Uri(apiBaseUrl) };
    });

    builder.Services.AddScoped<CustomAuthStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider>(
        sp => sp.GetRequiredService<CustomAuthStateProvider>());

    builder.Services.AddScoped<AuthService>(sp => new AuthService(
        sp.GetRequiredService<HttpClient>(),
        sp.GetRequiredService<CustomAuthStateProvider>(),
        sp.GetRequiredService<TokenStorageService>()
    ));

    HttpClient GatewayHttp(IServiceProvider sp) => sp.GetRequiredService<HttpClient>();

    builder.Services.AddScoped<IToastService, ToastService>();
    builder.Services.AddScoped<IUserStateService>(sp => new UserStateService(sp.GetRequiredService<IAuthService>()));
    builder.Services.AddScoped<IContactService>(sp => new ApiContactService(GatewayHttp(sp)));
    builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<AuthService>());
    builder.Services.AddScoped<IUserService>(sp => new ApiUserService(GatewayHttp(sp)));
    builder.Services.AddScoped<IPostService>(sp => new ApiPostService(GatewayHttp(sp)));
    builder.Services.AddScoped<IEventService>(sp => new ApiEventService(GatewayHttp(sp)));
    builder.Services.AddScoped<IResourceService>(sp => new ApiResourceService(GatewayHttp(sp)));
    builder.Services.AddScoped<IProfileService>(sp => new ApiProfileService(GatewayHttp(sp), sp.GetRequiredService<IUserStateService>()));
    builder.Services.AddScoped<ICommentService>(sp => new ApiCommentService(GatewayHttp(sp)));
    builder.Services.AddScoped<IRegistrationService>(sp => new ApiRegistrationService(GatewayHttp(sp)));
    builder.Services.AddScoped<INotificationService>(sp => new ApiNotificationService(GatewayHttp(sp)));
    builder.Services.AddScoped<IProjectService>(sp => new ApiProjectService(GatewayHttp(sp)));
    builder.Services.AddScoped<IPartnerService>(sp => new ApiPartnerService(GatewayHttp(sp)));
    builder.Services.AddScoped<INewsletterService>(sp => new ApiNewsletterService(GatewayHttp(sp)));
    builder.Services.AddScoped<IMemberDirectoryService>(sp => new ApiMemberDirectoryService(GatewayHttp(sp)));
    builder.Services.AddScoped<ISearchService>(sp => new ApiSearchService(GatewayHttp(sp)));
    builder.Services.AddScoped<IUploadService>(sp => new ApiUploadService(GatewayHttp(sp)));
}

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<DotnetNiger.UI.Services.Contracts.ILocalStorageService, JsLocalStorageService>();

await builder.Build().RunAsync();
