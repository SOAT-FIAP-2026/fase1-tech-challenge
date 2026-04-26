using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Cliente : EntidadeAuditavel
    {
        public NomeVO Nome { get; private set; } = null!;
        public CpfCnpjVO CpfCnpj { get; private set; } = null!;
        public EmailVO Email { get; private set; } = null!;
        public CelularVO Celular { get; private set; } = null!;

        protected Cliente() { }

        public Cliente(string nome, string cpfCnpj, string email, string celular) : base()
        {
            Nome = new NomeVO(nome);
            CpfCnpj = new CpfCnpjVO(cpfCnpj);
            Email = new EmailVO(email);
            Celular = new CelularVO(celular);
        }

        public void AlterarNome(string nome)
        {
            Nome = new NomeVO(nome);
            AtualizarTimestamp();
        }

        public void AlterarEmail(string email)
        {
            Email = new EmailVO(email);
            AtualizarTimestamp();
        }

        public void AlterarCelular(string celular)
        {
            Celular = new CelularVO(celular);
            AtualizarTimestamp();
        }

        public void Atualizar(string nome, string cpfCnpj, string email, string celular)
        {
            Nome = new NomeVO(nome);
            CpfCnpj = new CpfCnpjVO(cpfCnpj);
            Email = new EmailVO(email);
            Celular = new CelularVO(celular);
            AtualizarTimestamp();
        }
    }
}
