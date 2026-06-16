# DotnetNiger.UI

Blazor WebAssembly frontend for the DotnetNiger community platform -- a .NET developer community in Niger.

## Tech Stack

- **.NET 8.0** (Blazor WebAssembly)
- **Tailwind CSS 3** (via npm)
- **TinyMCE** (rich text editor)
- **Font Awesome 6** (icons)
- **C# service contracts** (interface-based DI)

## Architecture

Single-page application that communicates with the backend through an **Ocelot API Gateway** at `http://localhost:5000`. The frontend never talks to microservices directly -- all HTTP traffic is routed through the Gateway.

Authentication uses **OpenIddict** (OIDC) with httpOnly cookie-stored tokens. The frontend obtains tokens via the `connect/token` endpoint routed through the Gateway.

## Service Pattern

All backend interactions are abstracted behind C# interfaces in `Services/Contracts/`. Each interface has:

- A **Mock** implementation (hardcoded/in-memory data) for development without a backend
- An **Api** implementation (real HTTP calls to the Gateway) for production

The `UseMockServices` flag in `wwwroot/appsettings.json` controls which set of services is registered at startup.

## Quick Start

```bash
# 1. Clone the repository
git clone <repo-url>
cd DotnetNiger.UI

# 2. Install Tailwind CSS dependencies
npm install

# 3. Restore .NET packages
dotnet restore

# 4. Run the application (mock mode by default)
dotnet run
```

The frontend launches at `http://localhost:5201` by default.

> **Note:** In mock mode (`UseMockServices: true`) no backend is required. To work with a live backend, see the "Live Mode" section below.

## Mock Mode

Set `UseMockServices: true` in `wwwroot/appsettings.json`. All service calls return in-memory data. No backend processes needed.

## Live Mode

Set `UseMockServices: false` and ensure the full backend stack is running:

| Service        | Default Port | Description               |
|----------------|-------------|---------------------------|
| Identity       | 5075        | OpenIddict auth server    |
| Community API  | 5050        | Business logic API        |
| Ocelot Gateway | **5000**    | Entry point for frontend  |

The frontend reads the Gateway URL from the `ApiBaseUrl` config key.

## Service Contracts

| Interface                | Mock Implementation          | API Implementation          |
|--------------------------|------------------------------|-----------------------------|
| `IAuthService`           | `MockAuthService`            | `AuthService`               |
| `IPostService`           | `PostService` (Mock)         | `ApiPostService`            |
| `IEventService`          | `EventService` (Mock)        | `ApiEventService`           |
| `ICommentService`        | `CommentService` (Mock)      | `ApiCommentService`         |
| `IResourceService`       | `ResourceService` (Mock)     | `ApiResourceService`        |
| `IProjectService`        | `MockProjectService`         | `ApiProjectService`         |
| `IPartnerService`        | `MockPartnerService`         | `ApiPartnerService`         |
| `ISearchService`         | `MockSearchService`          | `ApiSearchService`          |
| `IMemberDirectoryService`| `MockMemberDirectoryService` | `ApiMemberDirectoryService` |
| `INewsletterService`     | `MockNewsletterService`      | `ApiNewsletterService`      |
| `IContactService`        | `MockContactService`         | `ApiContactService`         |
| `IUploadService`         | `MockUploadService`          | `ApiUploadService`          |
| `IProfileService`        | `ProfileService` (Mock)      | `ApiProfileService`         |
| `IUserService`           | `MockUserService`            | `ApiUserService`            |
| `INotificationService`   | `NotificationService` (Mock) | `ApiNotificationService`    |
| `IRegistrationService`   | `MockRegistrationService`    | `ApiRegistrationService`    |
| `IUserStateService`      | `MockUserStateService`       | `UserStateService`          |
| `IToastService`          | `ToastService`               | `ToastService`              |
| `ILocalStorageService`   | (always real via JS interop) | `JsLocalStorageService`     |

## Tailwind CSS

Build the CSS manually or use the watch script:

```bash
# One-time build
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css

# Watch mode (auto-rebuild on changes)
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch

# Or use the provided script
./Tailwind-Watch.sh
```

## Project Structure

```
DotnetNiger.UI/
├── Program.cs                   # DI registration, startup config
├── App.razor                    # Blazor router + auth cascade
├── Components/
│   ├── Shared/                  # Topbar, Sidebar, Footer
│   ├── Admin/                   # Admin-specific components
│   └── *.razor                  # BlogCard, EventCard, etc.
├── Pages/
│   ├── Home.razor
│   ├── Community.razor
│   ├── Events/
│   ├── Blog/
│   ├── Auth/
│   ├── Profile/
│   ├── Member/
│   ├── Projects/
│   ├── Admin/
│   └── ...                      # About, Contact, Partners, etc.
├── Services/
│   ├── Contracts/               # Interface definitions
│   ├── Api/                     # Real HTTP implementations
│   ├── Mock/                    # In-memory mock implementations
│   ├── Auth/                    # AuthService, state provider, handlers
│   └── Browser/                 # JS interop services
├── Models/
│   ├── Requests/                # Request DTOs (37)
│   └── Responses/               # Response DTOs (28)
├── Layouts/                     # MainLayout, AdminLayout, AuthLayout
├── wwwroot/
│   ├── css/                     # input.css, output.css, app.css
│   ├── lib/                     # TinyMCE, Font Awesome
│   └── index.html               # SPA entry point
├── tailwind.config.js
├── package.json
└── DotnetNiger.UI.csproj
```

## Git Workflow

- **`main` / `FrontEnd`** -- production/stable
- **`develop`** -- integration branch
- **`feature/...`** -- individual work branches

All work is done on feature branches branched from `develop`. Pull requests target `develop`. Never push directly to `main` or `FrontEnd`.

See `git_github_frontend_guidelines.md` for the full workflow.

## Backend Repo

The backend (Identity, Community API, Ocelot Gateway) lives in a separate repository. Refer to its `README.md` for setup instructions.

## Deployment

See [DEPLOY.md](DEPLOY.md) for all deployment options:
- **Blazor WASM**: GitHub Pages, Netlify, Vercel, Cloudflare Pages, Firebase, S3, Docker
- **Identity.Web**: Docker, MonsterASP, Somee, Oracle Cloud, Azure, VPS

## Links

- **Backend repo**: [DotnetNiger (backend)](https://github.com/DelaliAbel/DotnetNiger)
- **Frontend repo**: [DotnetNiger.UI](https://github.com/AbdoulRaouf2005/DotnetNiger.UI)
- **Issues**: [GitHub Issues](https://github.com/AbdoulRaouf2005/DotnetNiger.UI/issues)
