# Audit Frontend DotnetNiger — Bugs Design/UI/UX uniquement

> Généré le 2026-06-15
> Bugs techniques et fonctionnels corrigés — ne restent que les problèmes de design, UI et UX

## Résumé

| Métrique | Valeur |
|---|---|
| Pages totales | 26 |
| Composants | 11 (dont 6 Cards) |
| Services (Contracts) | 15 |
| Services (Impl API) | 10 |
| Services (Mock) | 6 |
| Bugs UI/UX restants | 10 |

---

## 🟡 HAUTS

### H2 — `Pages/Auth/VerifyEmail.razor:36-38`
**Problème :** `ResendVerification` ne désactive pas le bouton après clic
**Impact :** UX — envois multiples possibles

### H12 — `Components/Shared/ImageUploader.razor`
**Problème :** Pas d'indicateur de progression upload, pas de gestion erreur fichier trop volumineux
**Impact :** Mauvaise UX upload

---

## 🟡 MOYENS

### M3 — `Pages/Profile/Index.razor:142-144`
**Problème :** `Navigation.NavigateTo("/profile/informations")` redirige vers formulaire — pas de modal d'édition in-situ
**Impact :** UX fragmentée

### M4 — `Pages/Admin/Profile/Index.razor:160-165`
**Problème :** Même pattern — bouton "Modifier l'avatar" redirige vers page séparée
**Impact :** UX fragmentée

### M5 — `Pages/Admin/Ressource/Ressource.razor:120-150`
**Problème :** Le drawer CRUD submit ne ferme pas le drawer automatiquement après succès
**Impact :** UX — utilisateur doit fermer manuellement

### M6 — `Pages/Admin/Event/Event.razor:200-210`
**Problème :** `TogglePublishedAsync` / `ArchiveAsync` pas d'optimistic UI : bouton pas désactivé pendant appel API
**Impact :** Clics multiples possibles

### M7 — `Pages/Notifications.razor:100-108`
**Problème :** Pas de pagination — si >50 notifications, toutes chargées en mémoire
**Impact :** Performance dégradée avec volume

---

## 🟢 BAS

### B2 — `Pages/About.razor`
**Problème :** Photos membres en `<img>` sans fallback si URL invalide
**Impact :** Image brisée affichée

### B3 — `Pages/Contact.razor:90-110`
**Problème :** Formulaire de contact : pas de feedback visuel "message envoyé" avant redirection
**Impact :** UX — utilisateur incertain

### B5 — `Pages/Partners.razor:20-40`
**Problème :** Aucun filtrage entre partenaires "locaux", "technologiques" et "institutionnels" — tous fusionnés
**Impact :** Présentation confuse

---

## Bugs techniques corrigés (hors audit)

### 🔴 Critiques (6)
- C1 — `Login.razor` : fallback ApiBaseUrl retiré, utilise `appsettings.json`
- C2 — `ExternalCallback.razor` : redirection auto vers `/login` si échec
- C4 — `EventCreate.razor` / `CreateEventRequest` : gestion OrganizerName
- C5 — `ApiProfileService.cs` : null-check sur `GetProfileAsync()`
- C6 — `CustomAuthStateProvider.cs` : fallback claims name/email
- C7 — `EventEdit.razor` : DTO complet envoyé (IsPublished, TagNames)

### 🟡 Hauts (8)
- H3 — `Blog.razor` : filtres connectés à l'API
- H4 — `BlogDetail.razor` : sanitization XSS via `HtmlSanitizer`
- H5 — `Events.razor` : selects Lieu/Mois bindés
- H6 — `Dashboard.razor` : isolation erreurs par service, types extraits dans `DashboardTypes.cs`
- H9 — `Users.razor` : champ password en `InputPassword`
- H10 — `ProjectEdit.razor` : copie Status/ProjectUrl
- H11 — `CommentItem.razor` : timezone géré
- H1/Register — `Register.razor` : fallback ApiBaseUrl retiré, utilisation `IdentityEndpoints:Register`

### 🟡 Moyens (2)
- M9 — `Mock/EventService.cs` : fallback OrganizerName
- M10 — `Components/Admin/Cards/` : `@key` ajouté sur les foreach

### 🟢 Bas (3)
- B1 — `Home.razor` : lazy loading images
- B4 — `Community.razor` : Take paramétrable
- B6 — `Ressources.razor` : `isListView` inutilisé supprimé

### 🔧 Configuration & URLs (Session 2)
- **`appsettings.json`** : ajout section `IdentityEndpoints` (Login, Register, ForgotPassword, ResetPassword)
- **6 fichiers `.razor`** : retrait des fallbacks `http://localhost:5000` codés en dur, utilisation `IdentityEndpoints:Login`
  - `Pages/Auth/ResetPassword.razor`
  - `Pages/Auth/VerifyEmail.razor`
  - `Pages/Notifications.razor`
  - `Components/Admin/Shared/Topbar.razor`
  - `Components/Shared/TopBarAuthState.razor`
  - `Components/Shared/RedirectToLogin.razor`
- **Gateway URL unique** : `ApiBaseUrl` dans `appsettings.json` = point d'entrée unique configurable
- **Tous les redirects auth** passent par le gateway via `IdentityEndpoints`

---

## Fichiers analysés (40)

| Catégorie | Quantité |
|---|---|
| Pages | 26 |
| Composants | 11 |
| Services | 15 contrats + 10 API + 6 Mock |
