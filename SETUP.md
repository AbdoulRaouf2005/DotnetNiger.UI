# DotnetNiger.UI -- Setup Guide

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for Tailwind CSS)
- [Git](https://git-scm.com/)

## Clone

```bash
git clone https://github.com/AbdoulRaouf2005/DotnetNiger.UI.git
cd DotnetNiger.UI
```

## Install Dependencies

```bash
# .NET packages
dotnet restore

# Node packages (Tailwind CSS, TinyMCE)
npm install
```

## Build Tailwind CSS

```bash
# One-time build
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css

# Watch mode (auto-rebuild on .razor/.html changes)
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
```

Or use the provided script:

```bash
./Tailwind-Watch.sh
```

## Run

```bash
dotnet run
```

The application starts at `http://localhost:5201` by default (see `Properties/launchSettings.json`).

## Backend Setup

The frontend requires these backend services when running in live mode:

| Service        | Port | Repository                                 |
|----------------|------|--------------------------------------------|
| Identity       | 5075 | [DotnetNiger](https://github.com/DelaliAbel/DotnetNiger) |
| Community API  | 5050 | [DotnetNiger](https://github.com/DelaliAbel/DotnetNiger) |
| Ocelot Gateway | 5000 | [DotnetNiger](https://github.com/DelaliAbel/DotnetNiger) |

Refer to the backend repo's `README.md` for setup instructions.

## Mock Mode (No Backend Required)

For frontend-only development, use mock mode:

1. Open `wwwroot/appsettings.json`
2. Ensure `"UseMockServices": true`
3. Run `dotnet run` -- no backend processes needed

All service calls return hardcoded/in-memory data. Authentication always succeeds with a mock user.

## Configuration

All settings are in `wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5000",
  "UseMockServices": true,
  "IdentityEndpoints": {
    "Login": "/Account/Login",
    "Register": "/Account/Register",
    "ForgotPassword": "/Account/ForgotPassword",
    "ResetPassword": "/Account/ResetPassword"
  },
  "Community": {
    "MaxUpcomingEvents": 6
  }
}
```

| Key                   | Description                          | Default                    |
|-----------------------|--------------------------------------|----------------------------|
| `ApiBaseUrl`          | Ocelot Gateway URL                   | `http://localhost:5000`    |
| `UseMockServices`     | Enable mock data (no backend needed)  | `true`                     |
| `IdentityEndpoints`   | Redirect URLs for auth flows          | `/Account/*`               |
| `MaxUpcomingEvents`   | Max upcoming events on home page      | `6`                        |

## Running with Backend (Full Setup)

1. Start the backend services in order:
   - Identity Server (port 5075)
   - Community API (port 5050)
   - Ocelot Gateway (port 5000)

2. Edit `wwwroot/appsettings.json`:
   ```json
   {
     "ApiBaseUrl": "http://localhost:5000",
     "UseMockServices": false
   }
   ```

3. Run the frontend:
   ```bash
   dotnet run
   ```

4. Open `http://localhost:5201` in your browser.

## Troubleshooting

### CORS Errors

If the browser blocks requests to the Gateway, ensure the Gateway's CORS configuration allows the frontend origin (`http://localhost:5201`). The Gateway typically includes the necessary CORS headers, but verify if you see CORS errors in the browser console.

### Gateway Not Running

If all pages show errors or loading spinners don't resolve:

```bash
curl http://localhost:5000/api/v1/posts
```

If this fails, the Gateway is not running or not accessible. Start the backend stack or switch to mock mode.

### Blank Page / Blazor Fails to Load

- Check the browser console for Blazor errors.
- Ensure `dotnet restore` and `dotnet run` completed without errors.
- Verify `wwwroot/css/output.css` exists (run `npx tailwindcss` if missing).

### Tailwind Changes Not Applied

- Ensure the watch command is actively running.
- Verify `tailwind.config.js` content paths include the files you're editing:

```js
content: [
    "./Pages/**/*.razor",
    "./Components/**/*.razor",
    "./Layout/*.razor"
]
```

### Authentication Issues

- In mock mode, mock auth succeeds automatically.
- In live mode, ensure Identity server is running and accessible from the Gateway.
- Check `http://localhost:5000/health` (if available) to verify Gateway state.

### Port Conflicts

If port 5201 is in use, edit `Properties/launchSettings.json` to change the `applicationUrl`.
