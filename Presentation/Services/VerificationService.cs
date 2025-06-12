using Azure;
using Azure.Communication.Email;
using Presentation.Interfaces;
using Presentation.Models;

namespace Presentation.Services;

public class VerificationService(EmailClient emailClient, IConfiguration config) : IVerificationService
{
  private readonly EmailClient _emailClient = emailClient;
  private readonly IConfiguration _config = config;

  public async Task<VerificationServiceResult> SendVerificationLinkAsync(SendVerificationLinkRequest request)
  {
    if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.EmailToken))
    {
      return new VerificationServiceResult { Succeeded = false, Error = "Email, UserId, and Token are required." };
    }

    var confirmationUrl = $"https://verklig-ventixe-authservice-c3aza8fbe7gqh8e4.swedencentral-01.azurewebsites.net/api/auth/confirm-email?userId={Uri.EscapeDataString(request.UserId)}&emailToken={Uri.EscapeDataString(request.EmailToken)}";

    var subject = "Confirm your email address";
    var htmlBody = $@"
      <p>Click the link below to verify your email:</p>
      <p><a href='{confirmationUrl}'>Verify Email</a></p>";

    try
    {
      var message = new EmailMessage(
        _config["ACSSenderAddress"],
        request.Email,
        new EmailContent(subject)
        {
          Html = htmlBody
        });

      await _emailClient.SendAsync(WaitUntil.Completed, message);

      return new VerificationServiceResult { Succeeded = true, Message = "Verification link sent." };
    }
    catch (Exception ex)
    {
      return new VerificationServiceResult { Succeeded = false, Error = $"Failed to send email: {ex.ToString()}" };
    }
  }
}