# DotnetNiger.UI -- API Integration Guide

All frontend HTTP requests are sent to the **Ocelot API Gateway** at the URL configured in `ApiBaseUrl` (default: `http://localhost:5000`). The Gateway routes requests to the appropriate backend microservice (Identity or Community API).

---

## Endpoint Reference

### Posts

| Method   | Endpoint                        | Service Method                        | Description              |
|----------|---------------------------------|----------------------------------------|--------------------------|
| `GET`    | `api/v1/posts`                  | `GetAllPostsAsync()`                   | List all posts           |
| `GET`    | `api/v1/posts?published=true`   | `GetPublishedPostsAsync()`             | Published posts only     |
| `GET`    | `api/v1/posts?category={slug}`  | `GetPostsByCategoryAsync(slug)`        | Filter by category slug  |
| `GET`    | `api/v1/posts?tag={slug}`       | `GetPostsByTagAsync(slug)`             | Filter by tag slug       |
| `GET`    | `api/v1/posts/{id}`             | `GetPostByIdAsync(id)`                 | Single post by GUID      |
| `GET`    | `api/v1/posts/{slug}`           | `GetPostBySlugAsync(slug)`             | Single post by slug      |
| `GET`    | `api/v1/posts?query=q&page=1&pageSize=100` | `SearchPostsAsync(q)`   | Search posts             |
| `POST`   | `api/v1/posts`                  | `CreatePostAsync(request, userId)`      | Create a post            |
| `PUT`    | `api/v1/posts/{id}`             | `UpdatePostAsync(id, request)`          | Update a post            |
| `DELETE` | `api/v1/posts/{id}`             | `DeletePostAsync(id)`                   | Delete a post            |
| `POST`   | `api/v1/posts/{id}/views`       | `IncrementViewCountAsync(id)`           | Increment view counter   |

### Events

| Method   | Endpoint                                              | Service Method                           | Description                 |
|----------|-------------------------------------------------------|-------------------------------------------|-----------------------------|
| `GET`    | `api/v1/events`                                       | `GetAllEventsAsync()`                     | List all events             |
| `GET`    | `api/v1/events?published=true`                        | `GetPublishedEventsAsync()`               | Published events only       |
| `GET`    | `api/v1/events?past=true`                             | `GetPastEventsAsync()`                    | Past events                 |
| `GET`    | `api/v1/events/upcoming?page=1&pageSize=10`           | `GetUpcomingEventsAsync()`                | Upcoming events             |
| `GET`    | `api/v1/events?eventType={type}`                      | `GetEventsByTypeAsync(type)`              | Filter by event type        |
| `GET`    | `api/v1/events?query=q&page=1&pageSize=100`           | `SearchEventsAsync(q)`                    | Search events               |
| `GET`    | `api/v1/events/{id}`                                  | `GetEventByIdAsync(id)`                   | Single event by GUID        |
| `GET`    | `api/v1/events/{slug}`                                | `GetEventBySlugAsync(slug)`               | Single event by slug        |
| `GET`    | `api/v1/events?submitterId={userId}`                  | `GetEventsBySubmitterAsync(userId)`       | Events by submitter         |
| `GET`    | `api/v1/events/pending`                               | `GetPendingEventsAsync()`                 | Events pending approval     |
| `POST`   | `api/v1/events`                                       | `CreateEventAsync(request)`               | Create an event             |
| `PUT`    | `api/v1/events/{id}`                                  | `UpdateEventAsync(id, request)`           | Update an event             |
| `DELETE` | `api/v1/events/{id}`                                  | `DeleteEventAsync(id)`                    | Delete an event             |
| `PATCH`  | `api/v1/events/{id}/publish`                          | `TogglePublishAsync(id)`                  | Publish event               |
| `PATCH`  | `api/v1/events/{id}/unpublish`                        | `TogglePublishAsync(id)`                  | Unpublish event             |
| `PATCH`  | `api/v1/events/{id}/approve`                          | `ApproveEventAsync(id, comment?)`         | Approve event               |
| `PATCH`  | `api/v1/events/{id}/reject?reason={text}`             | `RejectEventAsync(id, reason)`            | Reject event                |
| `POST`   | `api/v1/events/registrations`                         | `RegisterToEventAsync(request, ...)`      | Register for an event       |
| `DELETE` | `api/v1/events/{eventId}/registrations`               | `CancelRegistrationAsync(eventId, ...)`   | Cancel registration         |
| `GET`    | `api/v1/events/{eventId}/registrations`               | `GetRegistrationsByEventAsync(eventId)`   | List event registrations    |

### Comments

| Method   | Endpoint                                     | Service Method                        | Description                     |
|----------|----------------------------------------------|----------------------------------------|---------------------------------|
| `GET`    | `api/v1/comments/post/{postId}`              | `GetCommentsByPostIdAsync(postId)`     | Comments for a post             |
| `GET`    | `api/v1/comments/event/{eventId}`            | `GetCommentsByEventIdAsync(eventId)`   | Comments for an event           |
| `GET`    | `api/v1/comments/{id}`                       | `GetCommentByIdAsync(id)`             | Single comment                  |
| `POST`   | `api/v1/comments`                            | `CreateCommentAsync(request)`         | Create a comment                |
| `PUT`    | `api/v1/comments/{id}`                       | `UpdateCommentAsync(request)`         | Update comment content          |
| `DELETE` | `api/v1/comments/{id}`                       | `DeleteCommentAsync(request)`         | Delete comment                  |
| `DELETE` | `api/v1/comments/{id}?deleteAllReplies=true` | `DeleteCommentAsync(request)`         | Delete comment + all replies    |

### Resources

| Method   | Endpoint                                     | Service Method                         | Description                  |
|----------|----------------------------------------------|----------------------------------------|------------------------------|
| `GET`    | `api/v1/resources`                           | `GetAllResourcesAsync()`               | List all resources           |
| `GET`    | `api/v1/resources?resourceType={type}`        | `GetResourcesByTypeAsync(type)`        | Filter by resource type      |
| `GET`    | `api/v1/resources?level={level}`             | `GetResourcesByLevelAsync(level)`      | Filter by difficulty level   |
| `GET`    | `api/v1/resources?query=q&page=1&pageSize=100` | `SearchResourcesAsync(q)`           | Search resources             |
| `GET`    | `api/v1/resources/{id}`                      | `GetResourceByIdAsync(id)`             | Single resource by GUID      |
| `GET`    | `api/v1/resources/{slug}`                    | `GetResourceBySlugAsync(slug)`         | Single resource by slug      |
| `GET`    | `api/v1/resources/types`                     | `GetResourceTypesAsync()`              | List resource types          |
| `GET`    | `api/v1/resources/levels`                    | `GetLevelsAsync()`                     | List difficulty levels       |
| `POST`   | `api/v1/resources`                           | `CreateResourceAsync(request)`         | Create a resource            |
| `PUT`    | `api/v1/resources/{id}`                      | `UpdateResourceAsync(id, request)`     | Update a resource            |
| `DELETE` | `api/v1/resources/{id}`                      | `DeleteResourceAsync(id)`              | Delete a resource            |
| `POST`   | `api/v1/resources/{id}/views`                | `IncrementViewCountAsync(id)`          | Increment view counter       |

### Search

| Method   | Endpoint                                          | Service Method              | Description                  |
|----------|---------------------------------------------------|------------------------------|------------------------------|
| `GET`    | `api/v1/search?query=q&type=t&page=1&pageSize=10` | `SearchAsync(request)`       | Full-text search across types |

### Member Directory

| Method   | Endpoint                                                       | Service Method                       | Description                  |
|----------|----------------------------------------------------------------|---------------------------------------|------------------------------|
| `GET`    | `api/v1/members?page=1&pageSize=10&query=q&country=c`         | `GetAllAsync(query, country, ...)`    | Paginated member list       |
| `GET`    | `api/v1/members/{id}`                                          | `GetByIdAsync(id)`                    | Single member profile       |

### Projects

| Method   | Endpoint                                                       | Service Method                       | Description                  |
|----------|----------------------------------------------------------------|---------------------------------------|------------------------------|
| `GET`    | `api/v1/projects?page=1&pageSize=10&status=s&query=q`         | `GetAllAsync(status, query, ...)`     | Paginated project list      |
| `GET`    | `api/v1/projects/featured`                                     | `GetFeaturedAsync()`                  | Featured projects           |
| `GET`    | `api/v1/projects/{id}`                                         | `GetByIdAsync(id)`                    | Single project              |
| `POST`   | `api/v1/projects`                                              | `CreateAsync(request)`                | Create a project            |
| `PUT`    | `api/v1/projects/{id}`                                         | `UpdateAsync(id, request)`            | Update a project            |
| `DELETE` | `api/v1/projects/{id}`                                         | `DeleteAsync(id)`                     | Delete a project            |

### Partners

| Method   | Endpoint                                   | Service Method                     | Description                  |
|----------|--------------------------------------------|-------------------------------------|------------------------------|
| `GET`    | `api/v1/partners`                          | `GetAllActiveAsync(partnerType?)`   | Active partners (optional filter) |
| `GET`    | `api/v1/partners/{id}`                     | `GetByIdAsync(id)`                  | Single partner               |

### Newsletter

| Method   | Endpoint                             | Service Method                  | Description                  |
|----------|--------------------------------------|----------------------------------|------------------------------|
| `POST`   | `api/v1/newsletter/subscribe`        | `SubscribeAsync(request)`        | Subscribe email              |
| `POST`   | `api/v1/newsletter/unsubscribe`      | `UnsubscribeAsync(request)`      | Unsubscribe email            |

### Contact

| Method   | Endpoint                  | Service Method          | Description                  |
|----------|---------------------------|--------------------------|------------------------------|
| `POST`   | `api/v1/contact`          | `SendAsync(request)`     | Submit contact form          |

### Upload

| Method   | Endpoint                | Service Method                        | Description                  |
|----------|-------------------------|----------------------------------------|------------------------------|
| `POST`   | `/api/upload`           | `UploadImageAsync(file, type)`          | Upload image file (multipart)|
| `POST`   | `/api/upload/base64`    | `UploadImageBase64Async(content, ...)`  | Upload base64 image          |
| `DELETE` | `/api/upload?path={url}` | `DeleteImageAsync(imageUrl)`            | Delete uploaded image        |

### Notifications

| Method   | Endpoint                                           | Service Method                     | Description                  |
|----------|----------------------------------------------------|-------------------------------------|------------------------------|
| `GET`    | `api/v1/notifications/{userId}`                    | `GetNotificationsAsync(userId)`     | List user notifications      |
| `GET`    | `api/v1/notifications/{userId}/unread-count`       | `GetUnreadCountAsync(userId)`       | Unread notification count    |
| `POST`   | `api/v1/notifications/{userId}`                    | `SendNotificationAsync(userId, msg)`| Send a notification          |
| `PATCH`  | `api/v1/notifications/{userId}/{notifId}/read`     | `MarkAsReadAsync(userId, notifId)`  | Mark single as read          |
| `PATCH`  | `api/v1/notifications/{userId}/read-all`           | `MarkAllAsReadAsync(userId)`        | Mark all as read             |

### Authentication

| Method   | Endpoint                          | Service Method                         | Description                       |
|----------|-----------------------------------|-----------------------------------------|-----------------------------------|
| `POST`   | `connect/token`                   | `LoginAsync(request)`                   | OIDC password grant (form-encoded)|
| `POST`   | `connect/token`                   | `CompleteExternalLoginAsync(ticket)`    | External login (form-encoded)     |
| `POST`   | `api/auth/register`               | `RegisterAsync(request)`                | User registration                 |
| `POST`   | `api/auth/forgot-password`        | `ForgotPasswordAsync(request)`          | Request password reset            |
| `POST`   | `api/auth/reset-password`         | `ResetPasswordAsync(request)`           | Reset password with token         |
| `POST`   | `api/auth/request-email-verification` | `RequestEmailVerificationAsync(req)` | Request email verification link   |
| `POST`   | `api/auth/verify-email`           | `VerifyEmailAsync(request)`             | Verify email with token           |
| `GET`    | `api/auth/session`                | `CustomAuthStateProvider` (internal)    | Restore session from cookies      |
| `POST`   | `api/auth/tokens`                 | `StoreTokensAsync()` (internal)         | Store access/refresh cookies      |
| `DELETE` | `api/auth/tokens`                 | `LogoutAsync()`                         | Clear auth cookies                |
| `POST`   | `api/auth/refresh`                | `RefreshTokenAsync()`                   | Refresh access token              |

### Profile

| Method   | Endpoint                       | Service Method                        | Description                  |
|----------|--------------------------------|----------------------------------------|------------------------------|
| `GET`    | `api/v1/me`                    | `GetProfileAsync()`                    | Get current user profile     |
| `PUT`    | `api/v1/me`                    | `UpdateProfileAsync(request)`          | Update profile               |
| `GET`    | `api/v1/social-links`          | `GetSocialLinksAsync()`                | List social links            |
| `POST`   | `api/v1/social-links`          | `AddSocialLinkAsync(request)`          | Add social link              |
| `DELETE` | `api/v1/social-links/{id}`     | `DeleteSocialLinkAsync(id)`            | Delete social link           |

### Admin -- Users

| Method   | Endpoint                                   | Service Method                        | Description                  |
|----------|--------------------------------------------|----------------------------------------|------------------------------|
| `GET`    | `api/v1/admin/users`                       | `GetUsersAsync()`                      | List all users               |
| `GET`    | `api/v1/admin/users/{id}`                  | `GetUserByIdAsync(id)`                 | Single user                  |
| `PATCH`  | `api/v1/admin/users/{id}/status`           | `UpdateUserAsync(user)`                | Activate/deactivate user     |
| `POST`   | `api/v1/admin/users/{id}/roles`            | `UpdateUserAsync(user)`                | Add role to user             |
| `PATCH`  | `api/v1/admin/users/{id}/status`           | `ApproveUserAsync(id)`                 | Approve/activate user        |
| `PATCH`  | `api/v1/admin/users/{id}/status`           | `RejectUserAsync(id)`                  | Reject/deactivate user       |

### Registration (Certification)

| Method   | Endpoint                          | Service Method                        | Description                  |
|----------|-----------------------------------|----------------------------------------|------------------------------|
| `POST`   | `api/v1/profile/certificates`     | `SubmitStep2Async(request)`            | Submit certification docs   |

---

## Service to Endpoint Mapping

| Service Contract     | API Implementation   | Base Path(s)                             |
|----------------------|----------------------|------------------------------------------|
| `IPostService`       | `ApiPostService`     | `api/v1/posts`                           |
| `IEventService`      | `ApiEventService`    | `api/v1/events`                          |
| `ICommentService`    | `ApiCommentService`  | `api/v1/comments`                        |
| `IResourceService`   | `ApiResourceService` | `api/v1/resources`                       |
| `ISearchService`     | `ApiSearchService`   | `api/v1/search`                          |
| `IMemberDirectoryService` | `ApiMemberDirectoryService` | `api/v1/members`                |
| `IProjectService`    | `ApiProjectService`  | `api/v1/projects`                        |
| `IPartnerService`    | `ApiPartnerService`  | `api/v1/partners`                        |
| `INewsletterService` | `ApiNewsletterService` | `api/v1/newsletter`                    |
| `IContactService`    | `ApiContactService`  | `api/v1/contact`                         |
| `IUploadService`     | `ApiUploadService`   | `/api/upload`                            |
| `INotificationService` | `ApiNotificationService` | `api/v1/notifications`              |
| `IProfileService`    | `ApiProfileService`  | `api/v1/me`, `api/v1/social-links`       |
| `IUserService`       | `ApiUserService`     | `api/v1/admin/users`                     |
| `IAuthService`       | `AuthService`        | `connect/token`, `api/auth/*`            |
| `IRegistrationService` | `ApiRegistrationService` | `api/v1/profile/certificates`       |

---

## Authentication

### Token Acquisition

The frontend uses the **OIDC resource owner password grant**:

1. `POST` username/password to `connect/token` with `grant_type=password`.
2. The Gateway proxies this to Identity/OpenIddict which returns JWT tokens.
3. Tokens are stored in **httpOnly cookies** via `POST /api/auth/tokens`.
4. Subsequent requests include the cookie, and `CustomAuthStateProvider` calls `GET /api/auth/session` to restore the authentication state.

### Token Refresh

Automatic refresh via `POST /api/auth/refresh` (called by `AuthService.RefreshTokenAsync()`). A semaphore prevents concurrent refresh calls.

### Session Restoration

On application load, `CustomAuthStateProvider` calls `GET /api/auth/session`. The Gateway reads the httpOnly cookie and returns the user's claims. The state is cached in memory for 5 minutes.

### Auth Headers

The `ClientIdHeaderHandler` automatically adds a `X-Client-Id` header to all outgoing HTTP requests. Authorization headers are not set manually -- the Gateway handles token verification from cookies.

---

## Response Format

All API responses are parsed by `ApiResponseReader` (`Services/Api/ApiResponseReader.cs`). The reader handles these shapes:

### Success -- Single Object

```json
{
  "success": true,
  "data": { ... },
  "message": "optional"
}
```

### Success -- Collection

```json
{
  "success": true,
  "data": [ ... ]
}
```

### Success -- Paginated

```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 10
  }
}
```

### Error

```json
{
  "success": false,
  "message": "Human-readable error description",
  "data": null
}
```

### Direct Responses

Some endpoints return JSON directly without wrapping (e.g., `connect/token` returns standard OIDC fields; upload endpoints return `UploadResponse` directly). The `ApiResponseReader` attempts wrapped deserialization first and falls back to direct deserialization.

---

## Error Handling

### Service Layer

Each API service method follows the same pattern:

1. If the HTTP response is unsuccessful (`!response.IsSuccessStatusCode`), return a default/empty value (`null`, empty list, or `false`).
2. If the response body is empty, return a default value.
3. Successful responses are deserialized through `ApiResponseReader`.

### Auth Errors

`AuthService` provides detailed error messages from the OIDC error response (`error_description`, `error` fields) or HTTP status codes.

### Upload Errors

`ApiUploadService` returns an `UploadResponse` with `Success = false` and a human-readable `Message` (client-side validation for file type/size is also performed before sending).

### Network Errors

`HttpRequestException` and `TaskCanceledException` (timeout) are caught in `AuthService` and `ApiNotificationService`, returning graceful fallback values.

### UI Layer

Pages and components typically check for null/empty results and show appropriate states:
- **Loading**: Spinner or skeleton while data is being fetched.
- **Empty**: "No items found" message when the result list is empty.
- **Error**: Error message (components do not generally surface HTTP errors to the user; they fall back gracefully).
