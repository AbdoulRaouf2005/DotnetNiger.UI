
using System.ComponentModel.DataAnnotations;

public class DeleteCommentRequest
{
      [Required]
      public string Id { get; set;} = string.Empty;
      public bool DeleteAllReplies { get; set;}
}