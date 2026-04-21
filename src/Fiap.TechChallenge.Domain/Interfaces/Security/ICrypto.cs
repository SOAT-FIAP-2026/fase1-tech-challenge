namespace Fiap.TechChallenge.Domain.Interfaces.Security
{
    public interface ICrypto
    {
        string CriptografarSenha(string senha);
        bool VerificarSenha(string senha, string hash);
    }
}