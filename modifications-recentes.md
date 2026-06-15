# Modifications récentes — Détail événement & Intervenants

## 1. Correction du mock `RegisterToEventAsync`

**Problème :** Le mock ne renseignait pas `AvatarUrl` dans le `EventRegistrationDto` retourné, les avatars dynamiques affichaient toujours les initiales.

**Modifications :**
| Fichier | Changement |
|---------|------------|
| `Models/Requests/RegisterEventRequest.cs` | Ajout de `string AvatarUrl` |
| `Pages/Events/Details.razor` | Passage de `UserStateService.CurrentUser?.AvatarUrl` dans la requête |
| `Services/Mock/EventService.cs` | Stockage de `request.AvatarUrl` dans le DTO créé |

---

## 2. Refonte complète de `Pages/Events/Details.razor`

### 2.1 Indentation
Le fichier entier a été réécrit avec une indentation cohérente (2 espaces par niveau de nesting).

### 2.2 Partage mobile — `IsShared` / `Closed`
Fusion des deux variables redondantes en une seule `IsShared`. La méthode `ToggleSharePanel()` ne fait plus que `IsShared = !IsShared`.

### 2.3 Lien "Voir plus de commentaires"
Supprimé (ne faisait rien — `<a href="#">` sans handler).

### 2.4 Bouton Meetup (Online / Hybrid)
Nouveau bouton affiché quand l'événement est de type Online ou Hybrid avec un `MeetupLink` renseigné :

```razor
@if (!string.IsNullOrWhiteSpace(CurrentEvent.MeetupLink) && CurrentEvent.EventType is "Online" or "Hybrid")
{
    <a href="@CurrentEvent.MeetupLink" target="_blank" rel="noopener noreferrer"
       class="inline-flex items-center gap-2 rounded-lg bg-secondary px-4 py-2 ...">
        <i class="fa-solid fa-video"></i>
        Rejoindre en @(CurrentEvent.EventType == "Online" ? "ligne" : "visioconférence")
    </a>
}
```

### 2.5 Inscription pour événements passés

| Situation | Affichage |
|-----------|-----------|
| Événement passé, non inscrit | Badge "Événement passé" + avatars + compteur |
| Événement passé, inscrit | Badge "Événement passé" + "Vous étiez inscrit" + avatars |
| Événement futur, non inscrit | Bouton "S'inscrire" + avatars |
| Événement futur, inscrit | Bouton rouge "Se désinscrire" + avatars |

### 2.6 Annulation d'inscription
Nouvelle méthode `HandleCancelRegistration()` avec état `IsCancelling`. Appelle `EventService.CancelRegistrationAsync()` et met à jour l'UI.

### 2.7 Galerie photos
Le cas `else if (_isPastEvent)` (événement passé sans galerie) affiche désormais un message explicite : **"Aucune photo disponible"**.

---

## 3. Intervenants — Sélection parmi les membres

### SpeakerDto enrichi
| Champ | Type | Description |
|-------|------|-------------|
| `UserId` | `Guid` | Référence vers l'utilisateur membre |
| `Name` | `string` | Nom complet (pré-rempli depuis le membre) |
| `Role` | `string` | Rôle dans l'événement (ex: "Intervenant", "Paneliste") |
| `AvatarUrl` | `string` | URL de l'avatar (pré-rempli depuis le membre) |

### Nouvelle UI dans EventCreate.razor et EventEdit.razor

```
┌─────────────────────────────────────────────┐
│  Intervenants                               │
│                                             │
│  ┌───┐  Nom complet ────────────  [🗑]     │
│  │🖼│  [ Rôle: Intervenant _______ ]       │
│  └───┘                                      │
│                                             │
│  [ 🔍 Rechercher un membre...        ]     │
│  ┌─────────────────────────────────────┐    │
│  │ 👤 Ahmed Moussa   @ahmed.moussa    │    │
│  │ 👤 Awa Issa       @awa.issa        │    │
│  │ 👤 Boubacar Adamou @boubacar.a     │    │
│  └─────────────────────────────────────┘    │
│                                             │
│  ➕ Ajouter un intervenant                  │
└─────────────────────────────────────────────┘
```

**Fonctionnement :**
1. Cliquer sur **"Ajouter un intervenant"** → un champ de recherche apparaît
2. Taper le nom ou l'username → filtrage en temps réel des membres
3. Cliquer sur un membre → ajouté à la liste avec nom + avatar pré-remplis
4. Le rôle est modifiable (champ texte)
5. Les membres déjà ajoutés sont exclus des résultats de recherche
6. Bouton poubelle pour retirer un intervenant

### Backend — `Services/Mock/EventService.cs`
Copie des nouveaux champs (`UserId`, `AvatarUrl`) dans `CreateEventAsync` et `UpdateEventAsync`.

### Page publique — `Details.razor`
Affichage de l'avatar photo (ou initiale) pour chaque intervenant :
```razor
@if (!string.IsNullOrWhiteSpace(speaker.AvatarUrl))
{
    <img src="@speaker.AvatarUrl" alt="@speaker.Name" class="..." />
}
else
{
    <span>@speaker.Name[..1].ToUpper()</span>
}
```

---

## Fichiers modifiés (résumé)

```
Models/Requests/RegisterEventRequest.cs         + AvatarUrl
Models/Requests/CreateEventRequest.cs           + Speakers + using
Models/Responses/EventDto.cs                    + Speakers
Models/Responses/SpeakerDto.cs                  NOUVEAU (UserId, Name, Role, AvatarUrl)
Pages/Events/Details.razor                      Refonte complète + speakers dynamiques
Pages/Admin/Event/EventCreate.razor             + Section intervenants avec sélecteur membres
Pages/Admin/Event/EventEdit.razor               + Section intervenants avec sélecteur membres
Services/Mock/EventService.cs                   + AvatarUrl + SpeakerDto champs
```
