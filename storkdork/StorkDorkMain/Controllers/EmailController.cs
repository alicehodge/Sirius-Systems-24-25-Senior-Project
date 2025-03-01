using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly SendGridService _sendGridService;

    public EmailController(SendGridService sendGridService)
    {
        _sendGridService = sendGridService;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] EmailRequest emailRequest)
    {
        if (string.IsNullOrEmpty(emailRequest.ToEmail) || string.IsNullOrEmpty(emailRequest.Subject))
        {
            return BadRequest("ToEmail and Subject are required.");
        }

        await _sendGridService.SendEmailAsync(
            "storkdorkapp@gmail.com",  // Your "From" email
            "Stork Dork",              // Your "From" name
            emailRequest.ToEmail,
            emailRequest.Subject,
            emailRequest.PlainTextContent,
            emailRequest.HtmlContent
        );
        return Ok("Email sent successfully!");
    }
}

public class EmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string PlainTextContent { get; set; }
    public string HtmlContent { get; set; }
}