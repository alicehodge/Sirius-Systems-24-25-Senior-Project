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

    public async Task SendEmailAsync(string fromEmail, string fromName, string toEmail, string templateId, string verificationLink, string user)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail.Trim());

        var msg = new SendGridMessage();
        msg.SetFrom(from);
        msg.AddTo(to);
        msg.SetTemplateId(templateId);

        var dynamicTemplateData = new
        {
            username = user,
            verification_link = verificationLink
        };
        msg.SetTemplateData(dynamicTemplateData);

        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        // You can handle the response here if you need
        // Example: logging the response status code

        Console.WriteLine($"Sent From:{fromEmail}");
        Console.WriteLine($"Email sent with status code: {response.StatusCode}");
        var responseBody = await response.Body.ReadAsStringAsync();
        Console.WriteLine($"Response body: {responseBody}");
    }

    public async Task SendEmailAsync(string fromEmail, string fromName, string toEmail, string templateId, string resetLink)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail.Trim());

        var msg = new SendGridMessage();
        msg.SetFrom(from);
        msg.AddTo(to);
        msg.SetTemplateId(templateId);

        var dynamicTemplateData = new
        {
            reset_link = resetLink
        };
        msg.SetTemplateData(dynamicTemplateData);

        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        // You can handle the response here if you need
        // Example: logging the response status code

        Console.WriteLine($"Sent From:{fromEmail}");
        Console.WriteLine($"Email sent with status code: {response.StatusCode}");
        var responseBody = await response.Body.ReadAsStringAsync();
        Console.WriteLine($"Response body: {responseBody}");
    }
}