using System.Text.RegularExpressions;
using Fiap.TechChallenge.Domain.Interfaces.Security;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record SenhaUsuarioVO
    {
        public string Hash { get; }

        private static readonly Regex ValidadorDeSenhaForteRegex = new Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            RegexOptions.Compiled);

        public SenhaUsuarioVO(string hash)
        {
            Hash = hash;
        }

        public static SenhaUsuarioVO CriarNova(string senhaPlana, ICrypto cryptoService)
        {
            if (string.IsNullOrWhiteSpace(senhaPlana))
                throw new ArgumentException("A senha é obrigatória.");

            if (!ValidadorDeSenhaForteRegex.IsMatch(senhaPlana))
                throw new ArgumentException("A senha deve conter no mínimo 8 caracteres, letras maiúsculas, minúsculas, números e caracteres especiais.");

            string hash = cryptoService.CriptografarSenha(senhaPlana);

            return new SenhaUsuarioVO(hash);
        }
    }
}