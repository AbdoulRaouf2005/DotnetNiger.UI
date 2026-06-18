# Modifications après le commit 27ddcdd (15 juin 2026)

> Fichier exclu de git — conservé localement.
> Commit de référence : `27ddcdd` (dernier commit ABDOUL RAOUF)
> HEAD : `b9096d4`

---

## Liste des commits (22 au total)

### 1. `e0a796a` — Migration API complète
- Tous les services utilisent l'API backend au lieu du mock
- Modifié : `Program.cs`, `AuthService.cs`, services API

### 2. `9905120` — Merge remote 'origin/develop'
- Résolution de conflit dans `CommentItem.razor`

### 3. `a51acd4` — Ajout champ isSubmitting dans CommentForm.razor

### 4. `dba2f98` — 19 bugs techniques + endpoints identity configurables
- Corrections de bugs frontend
- Endpoints Identity configurables via appsettings.json

### 5. `d84551b` — Suppression fichiers documentation obsolètes
- Supprimé : `parties_ui_manquantes.txt`, `modifications-recentes.md`

### 6. `3d77308` — Documentation frontend
- Ajout : `API.md`, `ARCHITECTURE.md`, `SETUP.md`

### 7. `b66b8ad` — Fichiers déploiement Docker
- Ajout : `Dockerfile`, `deploy/nginx.conf`, `deploy/start.sh`, `docker-compose.yml`

### 8. `4837733` — Fichiers déploiement Hugging Face
- Ajout : `Dockerfile.hf`

### 9-22. Corrections auth, CORS, déploiement, configuration
- Système d'auth complet : Bearer token via localStorage, refresh token, CustomAuthStateProvider
- Correction CORS, Blazor crash, scrolling
- Mode mock avec formulaire login
- Déploiements (DEPLOY.md, Docker, Hugging Face)

---

## Fichiers créés (nouveaux)

| Fichier | Description |
|---|---|
| `API.md` | Documentation API backend |
| `ARCHITECTURE.md` | Documentation architecture |
| `Dockerfile` | Déploiement Docker production |
| `Dockerfile.hf` | Déploiement Hugging Face |
| `DEPLOY.md` | Guide déploiement |
| `README.md` | Documentation projet |
| `SETUP.md` | Guide installation |
| `Services/Auth/BearerTokenHandler.cs` | Intercepteur HTTP pour token Bearer |
| `Services/Auth/TokenStorageService.cs` | Gestion stockage tokens |
| `Services/Auth/MockAuthenticationStateProvider.cs` | Provider auth mock |
| `Services/Api/ApiRegistrationService.cs` | Service inscription API |
| `Services/Api/UserStateService.cs` | Gestion état utilisateur |
| `Services/HtmlSanitizer.cs` | Assainissement HTML |
| `Models/DashboardTypes.cs` | Types dashboard admin |
| `deploy/nginx.conf` | Configuration Nginx production |
| `deploy/start.sh` | Script démarrage production |
| `docker-compose.yml` | Docker Compose production |
| `nginx.conf` | Configuration Nginx |
| `wwwroot/lib/iconify/iconify-icon.min.js` | Librairie icônes |

## Fichiers modifiés (existants)

### Core / Configuration
- `.gitignore` — ajout exclusions
- `Program.cs` — refonte DI, auth, CORS, HttpClient
- `App.razor` — ajout composants auth
- `wwwroot/appsettings.json` — endpoints configurables, Oidc, ApiEndpoints, AppRoutes
- `wwwroot/index.html` — iconify CDN

### Layout
- `Layouts/AdminLayout.razor` — contrôle auth manuel, diagnostic
- `Layouts/MainLayout.razor` — correction affichage

### Auth
- `Services/Auth/AuthService.cs` — refonte complète : PKCE, refresh token, state validation, configurable
- `Services/Auth/CustomAuthStateProvider.cs` — refonte : localStorage, double role claims
- `Services/Auth/ClientIdHeaderHandler.cs` — supprimé
- `Services/Contracts/IAuthService.cs` — ajout ExchangeCodeAsync, state optionnel
- `Services/Mock/MockAuthService.cs` — synchronisation interface

### Pages Auth
- `Pages/Auth/ExternalCallback.razor` — gestion ticket + code OIDC
- `Pages/Auth/Login.razor` — mode connexion Identity avec forceLoad
- `Pages/Auth/Register.razor` — redirection identity
- `Pages/Auth/ResetPassword.razor` — redirection identity
- `Pages/Auth/VerifyEmail.razor` — redirection identity

### Composants
- `Components/Shared/TopBarAuthState.razor` — redirection Identity
- `Components/Shared/RedirectToLogin.razor` — redirection Identity
- `Components/Admin/Shared/Topbar.razor` — logout redirection
- `Components/Comment/CommentForm.razor` — ajout isSubmitting
- `Components/Comment/CommentItem.razor` — conflit résolu

### Pages principales
- `Pages/Home.razor` — corrections mineures
- `Pages/Blog/Blog.razor` — améliorations
- `Pages/Events/Events.razor` — améliorations
- `Pages/Notifications.razor` — redirection Identity
- `Pages/Ressources.razor` — suppression ligne

### Admin
- `Pages/Admin/Dashboard.razor` — modifications dashboard
- `Pages/Admin/Blog/*.razor` — corrections routes et navigation
- `Pages/Admin/Event/*.razor` — corrections
- `Pages/Admin/Users.razor` — modifications table

### Services API (configurables via IConfiguration)
- `Services/Api/ApiPostService.cs`
- `Services/Api/ApiEventService.cs`
- `Services/Api/ApiResourceService.cs`
- `Services/Api/ApiProjectService.cs`
- `Services/Api/ApiPartnerService.cs`
- `Services/Api/ApiNewsletterService.cs`
- `Services/Api/ApiMemberDirectoryService.cs`
- `Services/Api/ApiSearchService.cs`
- `Services/Api/ApiNotificationService.cs`
- `Services/Api/ApiContactService.cs`
- `Services/Api/ApiProfileService.cs`
- `Services/Api/ApiCommentService.cs`
- `Services/Api/ApiUserService.cs`
- `Services/Api/ApiRegistrationService.cs` (nouveau)
- `Services/Api/ApiUploadService.cs`

### Fichiers supprimés
- `parties_ui_manquantes.txt`
- `modifications-recentes.md`
- `Services/Auth/ClientIdHeaderHandler.cs`

---

**Résumé :** 89 fichiers modifiés, +2471/-1107 lignes, ~3 semaines de travail.
