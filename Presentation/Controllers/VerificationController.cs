using Microsoft.AspNetCore.Mvc;
using Presentation.Interfaces;
using Presentation.Models;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController(IVerificationService verificationService) : ControllerBase
{
  private readonly IVerificationService _verificationService = verificationService;

  [HttpPost("send-verification-link")]
  public async Task<IActionResult> SendVerificationLinkAsync([FromBody] SendVerificationLinkRequest request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest("Invalid data.");
    }
    
    var result = await _verificationService.SendVerificationLinkAsync(request);

    return result.Succeeded
      ? Ok(result)
      : BadRequest(result);
  }
}