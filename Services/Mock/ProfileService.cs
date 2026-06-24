using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class ProfileService : IProfileService
{
    private readonly IUserStateService _userStateService;
    private readonly IUserService _userService;

    public ProfileService(IUserStateService userStateService, IUserService userService)
    {
        _userStateService = userStateService;
        _userService = userService;
    }

    private UserDto ResolveUser()
        => _userStateService.CurrentUser ?? new UserDto
        {
            Id = Guid.NewGuid(),
            SocialLinks = new List<SocialLinkDto>()
        };

    public async Task<UserDto> GetProfileAsync()
        => await Task.FromResult(ResolveUser());

    public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = ResolveUser();

        if (request.FullName is not null)    user.FullName    = request.FullName;
        if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
        if (request.Bio is not null)         user.Bio         = request.Bio;
        if (request.AvatarUrl is not null)   user.AvatarUrl   = request.AvatarUrl;
        if (request.Country is not null)     user.Country     = request.Country;
        if (request.City is not null)        user.City        = request.City;

        await _userStateService.UpdateUserAsync(user);
        await _userService.UpdateUserAsync(user);

        return await Task.FromResult(user);
    }

    public async Task<List<SocialLinkDto>> GetSocialLinksAsync()
        => await Task.FromResult(ResolveUser().SocialLinks);

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
        await _userStateService.UpdateUserAsync(user);

        return await Task.FromResult(link);
    }

    public async Task<bool> DeleteSocialLinkAsync(Guid id)
    {
        var user = ResolveUser();
        var removed = user.SocialLinks.RemoveAll(link => link.Id == id) > 0;

        if (removed)
            await _userStateService.UpdateUserAsync(user);

        return await Task.FromResult(removed);
    }
}
