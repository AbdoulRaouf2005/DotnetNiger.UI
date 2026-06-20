using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock
{
    /// <summary>
    /// Service mock de profil synchronisé avec IUserStateService.
    /// Toute modification du profil met à jour l'état global de l'utilisateur.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IUserStateService _userStateService;

        private UserDto _fallbackUser = new()
        {
            Id = Guid.NewGuid(),
            Username = "Abdoul Raouf",
            Email = "AbdoulRaoufHarouna@gmail.com",
            FullName = "Abdoul Raouf Harouna Moussa",
            PhoneNumber = "+227 96 00 00 00",
            Bio = "je suis un developpeur passionné par la tech",
            AvatarUrl = "./Images/upload/user.png",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 15),
            LastLoginAt = DateTime.Now,
            Roles = new List<string> { "Admin", "SuperAdmin" },
            SocialLinks = new List<SocialLinkDto>
            {
                new() { Id = Guid.NewGuid(), Platform = "LinkedIn", Url = "https://linkedin.com/in/jeandupont" },
                new() { Id = Guid.NewGuid(), Platform = "GitHub",   Url = "https://github.com/jeandupont" },
                new() { Id = Guid.NewGuid(), Platform = "Twitter",  Url = "https://twitter.com/jeandupont" }
            }
        };

        public ProfileService(IUserStateService userStateService)
        {
            _userStateService = userStateService;
        }

        // Résout l'utilisateur courant : UserStateService en priorité, sinon le fallback interne.
        private UserDto ResolveUser()
            => _userStateService.CurrentUser ?? _fallbackUser;

        public async Task<UserDto> GetProfileAsync()
        {
            await Task.Delay(800);
            return ResolveUser();
        }

        public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
        {
            var user = ResolveUser();

            if (request.FullName is not null)    user.FullName    = request.FullName;
            if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
            if (request.Bio is not null)         user.Bio         = request.Bio;
            if (request.AvatarUrl is not null)   user.AvatarUrl   = request.AvatarUrl;
            if (request.Country is not null)     user.Country     = request.Country;
            if (request.City is not null)        user.City        = request.City;

            // Synchronisation : on propage les changements dans UserStateService.
            await _userStateService.UpdateUserAsync(user);

            return await Task.FromResult(user);
        }

        public async Task<List<SocialLinkDto>> GetSocialLinksAsync()
        {
            await Task.Delay(800);
            return ResolveUser().SocialLinks;
        }

        public async Task<SocialLinkDto?> AddSocialLinkAsync(AddSocialLinkRequest request)
        {
            var user = ResolveUser();
            var link = new SocialLinkDto
            {
                Id       = Guid.NewGuid(),
                Platform = request.Platform,
                Url      = request.Url
            };

            user.SocialLinks.Add(link);

            // Synchronisation après ajout d'un lien social.
            await _userStateService.UpdateUserAsync(user);

            return await Task.FromResult(link);
        }

        public async Task<bool> DeleteSocialLinkAsync(Guid id)
        {
            var user = ResolveUser();
            var removed = user.SocialLinks.RemoveAll(link => link.Id == id) > 0;

            if (removed)
            {
                // Synchronisation après suppression d'un lien social.
                await _userStateService.UpdateUserAsync(user);
            }

            return await Task.FromResult(removed);
        }
    }
}
