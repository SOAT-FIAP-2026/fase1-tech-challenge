namespace Fiap.TechChallenge.Domain.Entities
{
    public class ItemServico
    {
        public Guid Id { get; private set; }
        public Guid IdOrdemServico { get; private set; }
        public Guid IdServico { get; private set; }
        public DateTime? DataHoraInicio { get; private set; }
        public DateTime? DataHoraFim { get; private set; }

        protected ItemServico() { }

        public ItemServico(Guid idOrdemServico, Guid idServico, DateTime? dataHoraInicio, DateTime? dataHoraFim)
        {
            Id = Guid.NewGuid();
            IdOrdemServico = idOrdemServico;
            IdServico = idServico;
            DataHoraInicio = dataHoraInicio;
            DataHoraFim = dataHoraFim;
        }
    }
}
