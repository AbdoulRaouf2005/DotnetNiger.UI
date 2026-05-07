
using System.ComponentModel.DataAnnotations;
namespace DotnetNiger.UI.Models.Requests;
public class DeleteCommentRequest
{
      [Required]
      public string Id { get; set;} = string.Empty;
      public bool DeleteAllReplies { get; set;}
}