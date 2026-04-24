namespace Fiap.TechChallenge.Domain.Entities
{
    public class ItemServico : EntidadeBase
    {
        public Guid IdOrdemServico { get; private set; }
        public Guid IdServico { get; private set; }
        public DateTime? DataHoraInicio { get; private set; }
        public DateTime? DataHoraFim { get; private set; }

        public OrdemServico OrdemServico { get; private set; } = null!;
        public Servico Servico { get; private set; } = null!;

        protected ItemServico() { }

        public ItemServico(Guid idOrdemServico, Guid idServico) : base()
        {
            IdOrdemServico = idOrdemServico;
            IdServico = idServico;
        }

        public void IniciarServico()
        {
            DataHoraInicio = DateTime.UtcNow;
        }

        public void FinalizarServico()
        {
            if (DataHoraInicio == null)
                throw new InvalidOperationException("O serviço precisa ser iniciado antes de ser finalizado.");

            DataHoraFim = DateTime.UtcNow;
        }
    }
}
