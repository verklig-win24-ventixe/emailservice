using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SendVerificationLinkRequest
{
  [Required]
  public string Email { get; set; } = null!;

  [Required]
  public string UserId { get; set; } = null!;

  [Required]
  public string EmailToken { get; set; } = null!;
}