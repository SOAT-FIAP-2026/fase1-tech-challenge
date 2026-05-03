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

        private readonly List<ItemServico> _itensServico = [];
        public IReadOnlyCollection<ItemServico> ItensServico => _itensServico.AsReadOnly();

        private readonly List<ItemPecaInsumo> _itensPecaInsumo = [];
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

        public void AlterarStatus(StatusOrdemServico status)
        {
            IdStatus = status.Id;
            Status = status;
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

        public bool RemoverItemServico(Guid idServico)
        {
            int itensRemovidos = _itensServico.RemoveAll(item => item.IdServico == idServico);

            if (itensRemovidos > 0)
                AtualizarTimestamp();

            return itensRemovidos > 0;
        }

        public bool RemoverItemPecaInsumo(Guid idPecaInsumo)
        {
            int itensRemovidos = _itensPecaInsumo.RemoveAll(item => item.IdPecaInsumo == idPecaInsumo);

            if (itensRemovidos > 0)
                AtualizarTimestamp();

            return itensRemovidos > 0;
        }

        public void SincronizarItens(IReadOnlyCollection<Servico> servicos, IReadOnlyCollection<PecaInsumo> pecasInsumo)
        {
            HashSet<Guid> servicosExistentes = _itensServico.Select(item => item.IdServico).ToHashSet();
            HashSet<Guid> pecasExistentes = _itensPecaInsumo.Select(item => item.IdPecaInsumo).ToHashSet();

            foreach (Servico servico in servicos)
            {
                if (servicosExistentes.Add(servico.Id))
                    _itensServico.Add(new ItemServico(Id, servico.Id));
            }

            foreach (PecaInsumo pecaInsumo in pecasInsumo)
            {
                if (pecasExistentes.Add(pecaInsumo.Id))
                    _itensPecaInsumo.Add(new ItemPecaInsumo(Id, pecaInsumo.Id));
            }

            RecalcularOrcamento(servicos, pecasInsumo);
        }

        public void RecalcularOrcamento(IReadOnlyCollection<Servico> servicos, IReadOnlyCollection<PecaInsumo> pecasInsumo)
        {
            decimal valorTotal = servicos.Sum(servico => servico.ValorUnitario.Valor) + pecasInsumo.Sum(peca => peca.ValorUnitario.Valor);

            if (Orcamento == null)
            {
                DefinirOrcamento(new Orcamento(Id, valorTotal));
                return;
            }

            Orcamento.AlterarValorTotal(valorTotal);
            AtualizarTimestamp();
        }

        public void DefinirOrcamento(Orcamento orcamento)
        {
            Orcamento = orcamento;
            AtualizarTimestamp();
        }
    }
}
