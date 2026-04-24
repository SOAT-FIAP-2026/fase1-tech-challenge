namespace Fiap.TechChallenge.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string CpfCnpj { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Celular { get; private set; } = string.Empty;

        protected Cliente() { }

        public Cliente(string nome, string cpfCnpj, string email, string celular)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            CpfCnpj = cpfCnpj;
            Email = email;
            Celular = celular;
        }
    }
}
