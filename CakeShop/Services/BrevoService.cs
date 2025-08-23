using System.Text;
using System.Text.Json;

namespace CakeShop.Services
{
    public static class BrevoService
    {
        public static async Task<bool> SendEmailAsync(string fromEmail, string fromName, string toEmail, string subject, string htmlBody, IConfiguration configuration)
        {
            try
            {
                // Lấy cấu hình từ appsettings.json
                var apiUrl = configuration["Brevo:ApiUrl"];
                var apiKey = configuration["Brevo:ApiKey"];

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                var emailData = new
                {
                    sender = new { name = fromName, email = fromEmail },
                    to = new[] { new { email = toEmail } },
                    subject = subject,
                    htmlContent = htmlBody
                };

                var json = JsonSerializer.Serialize(emailData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{apiUrl}/v3/smtp/email", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Brevo API Error: {response.StatusCode} - {errorContent}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email via Brevo: {ex.Message}");
                return false;
            }
        }

        // Phương thức đơn giản với cấu hình mặc định
        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, IConfiguration configuration)
        {
            var fromName = configuration["Brevo:FromName"];
            var fromEmail = configuration["Brevo:FromEmail"];

            return await SendEmailAsync(fromEmail, fromName, toEmail, subject, htmlBody, configuration);
        }
    }
}