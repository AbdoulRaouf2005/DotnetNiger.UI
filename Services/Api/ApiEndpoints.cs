namespace DotnetNiger.UI.Services.Api;

public static class ApiEndpoints
{
    public const string Events = "api/events";
    public const string AdminEvents = "api/community/admin/events";
    public const string Posts = "api/posts";
    public const string Resources = "api/resources";
    public const string Projects = "api/projects";
    public const string Partners = "api/partners";
    public const string Members = "api/members";
    public const string Search = "api/search";
    public const string Contact = "api/contact";
    public const string Notifications = "api/notifications";
    public const string Newsletters = "api/newsletters";
    public const string Upload = "/api/upload";
    public const string UploadBase64 = "/api/upload/base64";
    public const string Profile = "api/me";
    public const string SocialLinks = "api/social-links";
    public const string Certificates = "api/profile/certificates";
    public const string AdminUsers = "api/identity/admin/users";
    public const string Comments = "api/comments";

    public static class Auth
    {
        public const string Token = "connect/token";
        public const string Register = "api/auth/register";
        public const string Logout = "api/auth/logout";
        public const string ForgotPassword = "api/auth/forgot-password";
        public const string ResetPassword = "api/auth/reset-password";
        public const string RequestEmailVerification = "api/auth/request-email-verification";
        public const string VerifyEmail = "api/auth/verify-email";
    }
}
