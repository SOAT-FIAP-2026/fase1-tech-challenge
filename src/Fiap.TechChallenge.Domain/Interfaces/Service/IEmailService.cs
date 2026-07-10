namespace Fiap.TechChallenge.Domain.Interfaces.Service
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string para, string assunto, string corpoHtml);
    }
}
