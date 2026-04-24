namespace Fiap.TechChallenge.Domain.Entities
{
    public class Permissao
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; } = string.Empty;

        protected Permissao() { }

        public Permissao(string descricao)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
        }
    }
}
