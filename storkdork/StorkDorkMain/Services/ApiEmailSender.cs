using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;

    public ApiEmailSender(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailRequest = new
        {
            ToEmail = email,
            Subject = subject,
            PlainTextContent = "Please use an HTML-capable email client.",  // Fallback text
            HtmlContent = htmlMessage
        };

        var content = new StringContent(JsonSerializer.Serialize(emailRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5208/api/Email", content);  // localhost for development, switch to https://storkdork.azurewebsites.net/api/Email
        response.EnsureSuccessStatusCode();
    }
}
