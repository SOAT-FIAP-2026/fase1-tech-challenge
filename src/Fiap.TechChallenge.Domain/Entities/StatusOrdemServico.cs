namespace Fiap.TechChallenge.Domain.Entities
{
    public class StatusOrdemServico
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; } = string.Empty;

        protected StatusOrdemServico() { }

        public StatusOrdemServico(string descricao)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
        }
    }
}
