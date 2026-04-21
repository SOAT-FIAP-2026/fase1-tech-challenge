using Fiap.TechChallenge.Domain.Interfaces.Security;

namespace Fiap.TechChallenge.Infrastructure.Security
{
    public class Crypto : ICrypto
    {
        public string CriptografarSenha(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);
        public bool VerificarSenha(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}