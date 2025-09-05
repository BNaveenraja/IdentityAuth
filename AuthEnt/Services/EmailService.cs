using AuthEnt.Services;

namespace AuthEnt.Services
{
    public class EmailServiceStub : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string html)
        {
            // For development/testing only
            Console.WriteLine($"[Email Stub] To:{to}, Subject:{subject}, Body:{html}");
            return Task.CompletedTask;
        }
    }
}
