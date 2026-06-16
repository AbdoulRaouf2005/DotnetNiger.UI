# DotnetNiger.UI -- Architecture

## Overview

```
[Browser] --> DotnetNiger.UI (Blazor WASM)
                 |
                 | HTTP (via Ocelot Gateway)
                 v
         [Gateway @ :5000]
            /           \
           v             v
    [Identity]      [Community API]
    - OpenIddict      - Posts, Events
    - Auth            - Resources, Comments
    - Users           - Members, Projects
                      - Partners, Newsletter
```

The frontend is a Blazor WebAssembly single-page application. It never communicates directly with backend microservices -- every HTTP request is routed through the **Ocelot API Gateway** at the URL configured in `ApiBaseUrl` (default: `http://localhost:5000`).

## Component Tree

```
App.razor
  +-- HeadOutlet
  +-- CascadingAuthenticationState
        +-- Router (AuthorizeRouteView)
              +-- [Layout]
                    +-- MainLayout.razor       (public pages)
                    +-- AdminLayout.razor      (admin pages)
                    +-- AuthLayout.razor        (login/register)
                          +-- [Page]
                                +-- [Components]
                                      +-- Shared (Topbar, Sidebar, Footer)
                                      +-- Cards (BlogCard, EventCard, ProjectCard, ...)
                                      +-- Admin (admin-specific reusable UI)
                                      +-- Forms
```

- **Layouts** define the chrome (header, sidebar, footer).
- **Pages** are route targets and orchestrate data fetching via services.
- **Components** are reusable UI blocks (cards, forms, shared widgets).

## Service Layer

All backend interaction is abstracted behind C# interfaces in `Services/Contracts/`. The architecture follows a **strategy pattern** -- the `UseMockServices` config flag selects which set of implementations is registered in the DI container at startup.

### Interface-Based Design

```
I[Service]Interface          (Services/Contracts/)
  +-- Mock[Service]         (Services/Mock/) -- hardcoded/in-memory data
  +-- Api[Service]          (Services/Api/)  -- real HTTP calls to Gateway
```

### DI Registration (Program.cs)

```csharp
var useMock = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMock)
{
    // Register all mock implementations
    builder.Services.AddScoped<IPostService, PostService>();      // Mock
    builder.Services.AddScoped<IEventService, EventService>();    // Mock
    // ... 17 total mock services
}
else
{
    // Register all API implementations
    builder.Services.AddScoped<IPostService>(sp => new ApiPostService(GatewayHttp(sp)));
    builder.Services.AddScoped<IEventService>(sp => new ApiEventService(GatewayHttp(sp)));
    // ... 15 total API services
}
```

### Service Categories

| Category       | Always Present        | Mock Only                      | API Only           |
|----------------|-----------------------|--------------------------------|--------------------|
| Auth           | `IAuthService`        | --                             | --                 |
| User State     | `IUserStateService`   | --                             | --                 |
| Browser/UI     | `IToastService`       | --                             | --                 |
|                | `ILocalStorageService`|                                |                    |
| Business Logic | --                    | --                             | All entity services|
| Limited API    | --                    | `INotificationService`*        | --                 |
|                |                       | `IRegistrationService`**       |                    |

\* `INotificationService` -- the Community API has no dedicated notification endpoint yet; the mock and API implementations both simulate local behavior.

\*\* `IRegistrationService` -- the certification workflow (`SubmitStep2Async`) calls `api/v1/profile/certificates` in the API implementation; the mock covers both steps locally.

### Special Services

- **`IUserStateService`**: Always registered in both modes. Manages the current user in memory (mock uses simple properties; real version reads claims from the auth state).
- **`IAuthService`**: The real `AuthService` is always registered; in mock mode `MockAuthService` overrides it. Handles OIDC password grant, token refresh, session restoration, forgot/reset password, email verification, and external login.
- **`CustomAuthStateProvider`**: Always real. Wraps the Blazor `AuthenticationStateProvider`. Calls `api/auth/session` on the Gateway to restore authentication state from httpOnly cookies. Caches state for 5 minutes.

## Data Flow

```
User Action (click, form submit)
       |
       v
Razor Component code-behind (OnInitializedAsync, event handlers)
       |
       v
Service Interface method call (e.g., IPostService.GetAllPostsAsync)
       |
       v
  +--- Api[Service].Method()              +--- Mock[Service].Method()
  |    HttpClient.Get/Put/Post/Delete()    |    Returns hardcoded List<T>
  |    to Gateway URL                      |
  |    e.g., GET /api/v1/posts             |
  |       |                                |
  |       v                                |
  |   Ocelot Gateway routes to             |
  |   Community API or Identity            |
  |       |                                |
  |       v                                |
  |   Backend returns JSON                 |
  +----------------------------------------+
       |
       v
ApiResponseReader parses wrapped response
       |
       v
Component receives typed DTO, renders UI
```

### Response Format

All API responses are parsed by `ApiResponseReader` (`Services/Api/ApiResponseReader.cs`), which handles multiple formats:

```json
{
  "success": true,
  "data": { ... },              // Single object
  "message": "optional message"
}
```

```json
{
  "success": true,
  "data": [ ... ]               // Collection
}
```

```json
{
  "items": [ ... ],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10
}
```

## Mock vs Live Mode

| Aspect            | Mock Mode                      | Live Mode                        |
|-------------------|--------------------------------|----------------------------------|
| Config            | `UseMockServices: true`        | `UseMockServices: false`         |
| Backend required  | No                             | Yes (Identity + Community + Gateway) |
| Data              | Hardcoded/in-memory            | Real database                    |
| Auth              | MockAuthService (always passes)| Real OpenIddict flow             |
| HTTP calls        | None                           | All through Gateway              |
| File upload       | Returns fake URLs              | Uploads to server storage        |
| Search            | Filters in-memory list         | Backend search index             |

## Authentication

The authentication flow uses **OpenIddict** (OIDC) with the **resource owner password grant** (ROP) and external login support:

1. User submits credentials to `connect/token` (routed through Gateway to Identity).
2. Gateway returns access/refresh tokens.
3. Tokens are stored in **httpOnly cookies** via `api/auth/tokens`.
4. `CustomAuthStateProvider` calls `api/auth/session` to restore state from cookies on page load.
5. Token refresh happens automatically via `api/auth/refresh` (protected by a semaphore to prevent concurrent refreshes).
6. Logout calls `DELETE api/auth/tokens` and clears local state.

**Claims extraction**: User info (ID, email, name, roles, avatar) is extracted from the JWT claims on the client side after login.

## Routing

Blazor router in `App.razor` uses `AuthorizeRouteView` with `CascadingAuthenticationState`:

- Routes annotated with `[Authorize]` require authentication.
- `NotAuthorized` content redirects to the login page via `RedirectToLogin`.
- 404 falls through to a "Not found" message in `MainLayout`.

Layout selection is done per-page via `@layout` directives.

## Styling

- **Tailwind CSS 3** for utility-first styling.
- **Custom CSS** in `wwwroot/css/app.css` for application-specific styles.
- CSS variables defined in `app.css` control the theme colors, mapped in `tailwind.config.js`:

```js
colors: {
    primary: 'var(--color-primary)',
    secondary: 'var(--color-secondary)',
    // ...
}
```

- **Font Awesome 6** (pro) for icons (loaded from `lib/fontawesome/`).
- **TinyMCE** for rich text editing in blog post and event editors.
