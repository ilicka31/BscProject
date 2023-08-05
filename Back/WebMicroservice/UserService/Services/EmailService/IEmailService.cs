namespace UserService.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmail(Message message);
    }
}
