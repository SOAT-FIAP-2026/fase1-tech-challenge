using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Servico(string nome, string descricao, decimal valorUnitario) : EntidadeAuditavel()
    {
        public NomeVO Nome { get; private set; } = new NomeVO(nome);
        public DescricaoVO Descricao { get; private set; } = new DescricaoVO(descricao, 255);
        public ValorMonetarioVO ValorUnitario { get; private set; } = new ValorMonetarioVO(valorUnitario);

        public void Atualizar(string nome, string descricao, decimal valorUnitario)
        {
            Nome = new NomeVO(nome);
            Descricao = new DescricaoVO(descricao, 255);
            ValorUnitario = new ValorMonetarioVO(valorUnitario);
            AtualizarTimestamp();
        }
    }
}
