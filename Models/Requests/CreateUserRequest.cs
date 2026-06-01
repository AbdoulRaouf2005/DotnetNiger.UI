namespace DotnetNiger.UI.Models.Requests;

public class CreateUserRequest
{
      public string FullName { get; set; } = string.Empty;
      public string Email {get; set;} = string.Empty;
      public string Password {set; get;} = string.Empty;
}