namespace Fiap.TechChallenge.Domain.Entities
{
    public abstract class EntidadeBase
    {
        public Guid Id { get; protected set; }

        protected EntidadeBase()
        {
            Id = Guid.NewGuid();
        }
    }

    public abstract class EntidadeAuditavel : EntidadeBase
    {
        public DateTime CriadoEm { get; private set; }
        public DateTime AtualizadoEm { get; private set; }
        public DateTime? ApagadoEm { get; private set; }

        protected EntidadeAuditavel() : base()
        {
            CriadoEm = DateTime.UtcNow;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void MarcarComoApagado()
        {
            ApagadoEm = DateTime.UtcNow;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void AtualizarTimestamp()
        {
            AtualizadoEm = DateTime.UtcNow;
        }
    }
}
