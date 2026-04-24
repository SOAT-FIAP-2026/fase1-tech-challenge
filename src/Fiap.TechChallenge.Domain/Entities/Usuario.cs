using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Usuario : EntidadeAuditavel
    {
        public NomeVO Nome { get; private set; } = null!;
        public EmailVO Email { get; private set; } = null!;
        public SenhaUsuarioVO Senha { get; private set; } = null!;
        public Guid IdPermissao { get; private set; }
        public Permissao Permissao { get; private set; } = null!;

        protected Usuario() { }

        public Usuario(string nome, string email, SenhaUsuarioVO senha, Guid idPermissao) : base()
        {
            Nome = new NomeVO(nome);
            Email = new EmailVO(email);
            Senha = senha;
            IdPermissao = idPermissao;
        }

        public void AlterarNome(string novoNome)
        {
            Nome = new NomeVO(novoNome);
            AtualizarTimestamp();
        }

        public void AlterarEmail(string novoEmail)
        {
            Email = new EmailVO(novoEmail);
            AtualizarTimestamp();
        }
    }
}
