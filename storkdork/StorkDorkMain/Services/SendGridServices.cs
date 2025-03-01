using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class SendGridService
{
    private readonly string _apiKey;

    public SendGridService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task SendEmailAsync(string fromEmail, string fromName, string toEmail, string subject, string plainTextContent, string htmlContent)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail.Trim());
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        // You can handle the response here if you need
        // Example: logging the response status code

        Console.WriteLine($"Sent From:{fromEmail}");
        Console.WriteLine($"Email sent with status code: {response.StatusCode}");
        var responseBody = await response.Body.ReadAsStringAsync();
        Console.WriteLine($"Response body: {responseBody}");
    }
}