namespace Fiap.TechChallenge.Domain.Entities
{
    public class OrdemServico
    {
        public Guid Id { get; private set; }
        public Guid IdCliente { get; private set; }
        public Guid IdVeiculo { get; private set; }
        public Guid IdStatus { get; private set; }
        public string? Observacao { get; private set; }

        protected OrdemServico() { }

        public OrdemServico(Guid idCliente, Guid idVeiculo, Guid idStatus, string? observacao)
        {
            Id = Guid.NewGuid();
            IdCliente = idCliente;
            IdVeiculo = idVeiculo;
            IdStatus = idStatus;
            Observacao = observacao;
        }
    }
}
