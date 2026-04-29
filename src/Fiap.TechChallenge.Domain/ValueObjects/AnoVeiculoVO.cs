namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record AnoVeiculoVO
    {
        public int Valor { get; }

        protected AnoVeiculoVO() { }

        public AnoVeiculoVO(int ano)
        {
            int anoMaximo = DateTime.UtcNow.Year + 1;

            if (ano < 1900)
                throw new ArgumentException("O ano do veículo não pode ser anterior a 1900.");

            if (ano > anoMaximo)
                throw new ArgumentException($"O ano do veículo não pode ser posterior a {anoMaximo}.");

            Valor = ano;
        }
    }
}
