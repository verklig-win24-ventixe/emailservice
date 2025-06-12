using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController(IConfiguration config) : ControllerBase
{
  [HttpGet("acs-sender")]
  public IActionResult GetSender()
  {
    return Ok(new { SenderAddress = config["ACSSenderAddress"] ?? "null" });
  }
}
