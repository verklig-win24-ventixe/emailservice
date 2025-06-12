using Presentation.Models;

namespace Presentation.Interfaces;

public interface IVerificationService
{
  Task<VerificationServiceResult> SendVerificationLinkAsync(SendVerificationLinkRequest request);
}