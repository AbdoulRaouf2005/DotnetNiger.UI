
using System.ComponentModel.DataAnnotations;
namespace DotnetNiger.UI.Models.Requests;
public class DeleteCommentRequest
{
      [Required]
      public Guid Id { get; set; }
      public bool DeleteAllReplies { get; set; }
}