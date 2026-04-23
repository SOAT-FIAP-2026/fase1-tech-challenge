using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public NomeUsuarioVO Nome { get; private set; }
        public EmailUsuarioVO Email { get; private set; }
        public SenhaUsuarioVO Senha { get; private set; }

        protected Usuario() { }

        public Usuario(string nome, string email, SenhaUsuarioVO senha)
        {
            Id = Guid.NewGuid();
            Nome = new NomeUsuarioVO(nome);
            Email = new EmailUsuarioVO(email);
            Senha = senha;
        }

        public void AlterarNome(NomeUsuarioVO novoNome)
        {
            Nome = novoNome;
        }
    }
}
