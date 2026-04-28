namespace Fiap.TechChallenge.Domain.Entities
{
    public class OrdemServico : EntidadeAuditavel
    {
        public Guid IdCliente { get; private set; }
        public Guid IdVeiculo { get; private set; }
        public Guid IdStatus { get; private set; }
        public string? Observacao { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DataConclusao { get; private set; }

        public Cliente Cliente { get; private set; } = null!;
        public Veiculo Veiculo { get; private set; } = null!;
        public StatusOrdemServico Status { get; private set; } = null!;
        public Orcamento? Orcamento { get; private set; }

        private readonly List<ItemServico> _itensServico = new();
        public IReadOnlyCollection<ItemServico> ItensServico => _itensServico.AsReadOnly();

        private readonly List<ItemPecaInsumo> _itensPecaInsumo = new();
        public IReadOnlyCollection<ItemPecaInsumo> ItensPecaInsumo => _itensPecaInsumo.AsReadOnly();

        protected OrdemServico() { }

        public OrdemServico(Guid idCliente, Guid idVeiculo, Guid idStatus, string? observacao = null) : base()
        {
            IdCliente = idCliente;
            IdVeiculo = idVeiculo;
            IdStatus = idStatus;
            Observacao = observacao;
            DataAbertura = DateTime.UtcNow;
        }

        public void AlterarStatus(Guid idStatus)
        {
            IdStatus = idStatus;
            AtualizarTimestamp();
        }

        public void Concluir()
        {
            DataConclusao = DateTime.UtcNow;
            AtualizarTimestamp();
        }

        public void AdicionarItemServico(ItemServico item)
        {
            _itensServico.Add(item);
            AtualizarTimestamp();
        }

        public void AdicionarItemPecaInsumo(ItemPecaInsumo item)
        {
            _itensPecaInsumo.Add(item);
            AtualizarTimestamp();
        }

        public void DefinirOrcamento(Orcamento orcamento)
        {
            Orcamento = orcamento;
            AtualizarTimestamp();
        }
    }
}
