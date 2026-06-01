# Progression

## Goal
Implémenter tous les services frontend manquants pour les fonctionnalités backend Community API, dynamiser Partners.razor, créer la page détail ressource, corriger le footer newsletter.

## Constraints & Preferences
- Tout passe par le Gateway Ocelot (`http://localhost:5000`)
- Ne pas toucher aux dossiers `bin/` et `obj/`
- Le frontend fonctionne actuellement en mode mock (`UseMockServices: true` dans `wwwroot/appsettings.json`)
- Le frontend et le backend sont sur deux dépôts GitHub distincts

## Progress
### Done
- **5 interfaces service** : `IProjectService`, `IPartnerService`, `INewsletterService`, `IMemberDirectoryService`, `ISearchService`
- **6 request DTOs** : `Create/UpdateProjectRequest`, `Subscribe/UnsubscribeRequest`, `Create/UpdatePartnerRequest`
- **5 response DTOs** : `ProjectResponse`, `PartnerResponse`, `NewsletterSubscriptionResponse`, `MemberDirectoryResponse`, `DashboardResponse`
- **5 services Mock** : `MockProjectService`, `MockPartnerService`, `MockNewsletterService`, `MockMemberDirectoryService`, `MockSearchService`
- **6 services Api** : `ApiProjectService`, `ApiPartnerService`, `ApiNewsletterService`, `ApiMemberDirectoryService`, `ApiSearchService`, `ApiNotificationService`
- **Program.cs** : tous les services enregistrés dans les deux modes (mock + API)
- **Build** : 0 erreur

### In Progress
- **Rendre `Partners.razor` dynamique** avec `IPartnerService`
- **Créer la page détail ressource** (`/ressource/{slug}`)
- **Corriger le footer** (newsletter avec `INewsletterService`)

### Blocked
- *(none)*

## Key Decisions
- `SearchResultDto` et `PaginatedDto<T>` existants réutilisés sans modification
- `INotificationService` reste mocké côté backend (pas d'API notification dédiée dans Community) ; `ApiNotificationService` créé pour simulation HTTP
- `IRegistrationService` conserve son implémentation mock uniquement (workflow de certification sans endpoint API)
- `IUserStateService` enregistré dans les deux modes (mock + API)
- Le partenaire côté frontend utilise `PartnerResponse` (DTO dédié) au lieu de `UserDto`

## Next Steps
1. Dynamiser `Pages/Partners.razor` avec injection `IPartnerService`
2. Créer la page détail ressource avec slug
3. Connecter le formulaire newsletter du footer à `INewsletterService`
4. Build + test final
