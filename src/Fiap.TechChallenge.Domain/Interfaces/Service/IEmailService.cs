namespace Fiap.TechChallenge.Domain.Interfaces.Service
{
    public interface IEmailService
    {
        Task<bool> EnviarEmailAsync(string para, string assunto, string corpoHtml);
    }
}
