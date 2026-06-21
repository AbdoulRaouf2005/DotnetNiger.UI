# Plan de correction — Prévisualisation sociale (Open Graph)

## Problème

L'application Blazor WASM tourne 100 % côté client. Les crawlers sociaux (Facebook, Twitter, LinkedIn, WhatsApp, Slack, Discord, Telegram, Pinterest) ne voient que le `index.html` statique **sans aucune balise OG**. Les `<HeadContent>` dynamiques injectés dans `BlogDetail.razor`, `Events/Details.razor` et `RessourceDetail.razor` ne sont jamais exécutés côté serveur.

**Backend :** `C:\Users\ABDOUL RAOUF\Desktop\Backend\DotnetNiger`
- **Gateway** (`DotnetNiger.Gateway`, port 5000) — Ocelot, n'héberge pas le WASM
- **Community** (`DotnetNiger.Community`, port 5050) — API contenant les données (posts, events, resources)

**Frontend :** `C:\Users\ABDOUL RAOUF\Desktop\DotnetNiger` — WASM standalone déployé séparément

---

## Modifications Backend

### 1. Gateway — Nouveau middleware `OpenGraphMiddleware`

**Fichier :** `DotnetNiger.Gateway/Middleware/OpenGraphMiddleware.cs`

Intercepte les requêtes des crawlers sociaux et retourne une page HTML statique avec les balises OG.

Logique :
1. Lire le header `User-Agent`
2. Si contient un crawler connu → continuer, sinon → `await _next(context)` et retour
3. Analyser le chemin : `/blog/{slug}`, `/evenements/{slug}`, `/ressources/{slug}`
4. Appeler le service `IOpenGraphService` pour récupérer les métadonnées via l'API Community
5. Construire une page HTML minimale via `OpenGraphHtmlBuilder`
6. Retourner `Content-Type: text/html; charset=utf-8`

**Liste des User-Agents crawlers à détecter :**
- `facebookexternalhit`
- `Twitterbot`
- `LinkedInBot`
- `WhatsApp`
- `Slack`
- `Discordbot`
- `TelegramBot`
- `Pinterest`

### 2. Gateway — Nouveau service `IOpenGraphService`

**Fichier :** `DotnetNiger.Gateway/Services/IOpenGraphService.cs`

```csharp
public interface IOpenGraphService
{
    Task<OGMetadata?> FetchMetadataAsync(string type, string slug);
}

public class OGMetadata
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 3. Gateway — Nouveau service `OpenGraphService`

**Fichier :** `DotnetNiger.Gateway/Services/OpenGraphService.cs`

Appelle les endpoints Community via HTTP :
- `GET /api/v1/posts/by-slug/{slug}` → Blog
- `GET /api/v1/events/by-slug/{slug}` → Événements
- `GET /api/v1/resources/by-slug/{slug}` → Ressources

Utilise `IHttpClientFactory` pour créer un client pointant sur le service Community.

### 4. Gateway — Nouveau builder `OpenGraphHtmlBuilder`

**Fichier :** `DotnetNiger.Gateway/Services/OpenGraphHtmlBuilder.cs`

Construit une page HTML minimale :

```html
<!DOCTYPE html>
<html lang="fr">
<head>
  <meta charset="utf-8" />
  <title>{title} — Dotnet Niger</title>
  <meta property="og:title" content="{title}" />
  <meta property="og:description" content="{description}" />
  <meta property="og:image" content="{absolute image URL}" />
  <meta property="og:url" content="{canonical URL}" />
  <meta property="og:type" content="article" />
  <meta property="og:site_name" content="Dotnet Niger" />
  <meta property="og:locale" content="fr_FR" />
  <meta name="twitter:card" content="summary_large_image" />
  <meta name="twitter:title" content="{title}" />
  <meta name="twitter:description" content="{description}" />
  <meta name="twitter:image" content="{absolute image URL}" />
  <meta http-equiv="refresh" content="0; url={frontend URL}" />
</head>
<body>
  <script>window.location.href = "{frontend URL}";</script>
</body>
</html>
```

### 5. Gateway — `appsettings.json`

**Fichier :** `DotnetNiger.Gateway/appsettings.json`

Ajouter la clé :
```json
"FrontendBaseUrl": "https://dotnetniger.org"
```

Utilisée par `OpenGraphHtmlBuilder` pour construire les URLs canoniques et le refresh.

### 6. Gateway — Enregistrement dans `ServiceCollectionExtensions.cs`

**Fichier :** `DotnetNiger.Gateway/Extensions/ServiceCollectionExtensions.cs`

Ajouter :
```csharp
services.AddHttpClient<IOpenGraphService, OpenGraphService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5050");
});
services.AddSingleton<OpenGraphHtmlBuilder>();
```

### 7. Gateway — Enregistrement dans `ApplicationBuilderExtensions.cs`

**Fichier :** `DotnetNiger.Gateway/Extensions/ApplicationBuilderExtensions.cs`

Ajouter `app.UseMiddleware<OpenGraphMiddleware>()` dans la pipeline, après le middleware de sécurité et avant Ocelot.

### 8. Community — Endpoint `by-slug` pour Posts

**Fichier :** `DotnetNiger.Community/Api/Controllers/PostsController.cs`

Ajouter :
```csharp
[HttpGet("by-slug/{slug}")]
public async Task<ActionResult<OGMetadata>> GetBySlug(string slug)
```

Retourne `Title`, `Description` (excerpt), `ImageUrl` (cover), `UpdatedAt`.

### 9. Community — Endpoint `by-slug` pour Events

**Fichier :** `DotnetNiger.Community/Api/Controllers/EventsController.cs`

Ajouter :
```csharp
[HttpGet("by-slug/{slug}")]
public async Task<ActionResult<OGMetadata>> GetBySlug(string slug)
```

### 10. Community — Endpoint `by-slug` pour Resources

**Fichier :** `DotnetNiger.Community/Api/Controllers/ResourcesController.cs`

Ajouter :
```csharp
[HttpGet("by-slug/{slug}")]
public async Task<ActionResult<OGMetadata>> GetBySlug(string slug)
```

### 11. Gateway — Routes Ocelot

**Fichier :** `DotnetNiger.Gateway/ocelot.community.routes.json`

Ajouter les routes (si inexistantes) :
```json
{
  "UpstreamPathTemplate": "/api/posts/by-slug/{slug}",
  "DownstreamPathTemplate": "/api/v1/posts/by-slug/{slug}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [{ "Host": "localhost", "Port": 5050 }]
}
```
Idem pour `events/by-slug` et `resources/by-slug`.

---

## Modifications Frontend

### 12. Fix `GetAbsoluteUrl` dans `BlogDetail.razor`

**Fichier :** `Pages/Blog/BlogDetail.razor`

**Bug :** `GetAbsoluteUrl(NavManager.Uri)` produit une URL doublonnée (base URI + URI déjà absolu).

**Correction :**
- `og:url` → utiliser `NavManager.Uri` directement
- Image → créer une méthode `ResolveImageUrl(string?)` qui :
  - Si null ou vide → retourne `{BaseUri}/images/og-default.jpg`
  - Si commence par `http` → retourne tel quel
  - Sinon → concatène `{BaseUri}/{path}`

### 13. Fix `GetAbsoluteUrl` dans `RessourceDetail.razor`

**Fichier :** `Pages/RessourceDetail.razor`

Même correctif que le blog.

### 14. Image OG par défaut pour les ressources

**Fichier :** `Pages/RessourceDetail.razor`

Remplacer l'image SVG (`/Logo.svg`) par une image PNG/JPG (`/images/og-default.jpg`) pour la compatibilité avec les crawlers.

### 15. Ajouter `og:site_name` et `og:locale` dans les pages détail

**Fichier :** `Pages/Blog/BlogDetail.razor`
**Fichier :** `Pages/Events/Details.razor`
**Fichier :** `Pages/RessourceDetail.razor`

Ajouter dans le `<HeadContent>` de chaque page :
```razor
<meta property="og:site_name" content="Dotnet Niger" />
<meta property="og:locale" content="fr_FR" />
```

### 16. Balises OG par défaut dans `index.html`

**Fichier :** `wwwroot/index.html`

Ajouter dans la balise `<head>` :
```html
<meta property="og:title" content="Dotnet Niger" />
<meta property="og:description" content="Communauté .NET du Niger" />
<meta property="og:image" content="{URL}/images/og-default.jpg" />
<meta property="og:type" content="website" />
<meta property="og:site_name" content="Dotnet Niger" />
<meta property="og:locale" content="fr_FR" />
<meta name="twitter:card" content="summary_large_image" />
```

Ces balises servent de fallback pour toutes les pages qui n'ont pas de middleware OG (accueil, liste, contact, etc.).

---

## Résumé des fichiers à modifier/créer

| # | Fichier | Action |
|---|---------|--------|
| 1 | `Backend/DotnetNiger.Gateway/Middleware/OpenGraphMiddleware.cs` | **Créer** |
| 2 | `Backend/DotnetNiger.Gateway/Services/IOpenGraphService.cs` | **Créer** |
| 3 | `Backend/DotnetNiger.Gateway/Services/OpenGraphService.cs` | **Créer** |
| 4 | `Backend/DotnetNiger.Gateway/Services/OpenGraphHtmlBuilder.cs` | **Créer** |
| 5 | `Backend/DotnetNiger.Gateway/appsettings.json` | **Modifier** |
| 6 | `Backend/DotnetNiger.Gateway/Extensions/ServiceCollectionExtensions.cs` | **Modifier** |
| 7 | `Backend/DotnetNiger.Gateway/Extensions/ApplicationBuilderExtensions.cs` | **Modifier** |
| 8 | `Backend/DotnetNiger.Community/Api/Controllers/PostsController.cs` | **Modifier** |
| 9 | `Backend/DotnetNiger.Community/Api/Controllers/EventsController.cs` | **Modifier** |
| 10 | `Backend/DotnetNiger.Community/Api/Controllers/ResourcesController.cs` | **Modifier** |
| 11 | `Backend/DotnetNiger.Gateway/ocelot.community.routes.json` | **Modifier** |
| 12 | `Frontend/Pages/Blog/BlogDetail.razor` | **Modifier** |
| 13 | `Frontend/Pages/RessourceDetail.razor` | **Modifier** |
| 14 | `Frontend/Pages/Events/Details.razor` | **Modifier** |
| 15 | `Frontend/wwwroot/index.html` | **Modifier** |
| 16 | `Frontend/wwwroot/images/og-default.jpg` | **Créer/Ajouter** |

---

## Ordre d'implémentation suggéré

1. Community : endpoints `by-slug` (Posts, Events, Resources)
2. Community : DTO `OGMetadata`
3. Gateway : services `IOpenGraphService`, `OpenGraphService`, `OpenGraphHtmlBuilder`
4. Gateway : `OpenGraphMiddleware`
5. Gateway : DI + middleware pipeline
6. Gateway : routes Ocelot
7. Frontend : fix `GetAbsoluteUrl` et OG tags dans les 3 pages détail
8. Frontend : balises OG par défaut dans `index.html`
9. Frontend : image `og-default.jpg`
