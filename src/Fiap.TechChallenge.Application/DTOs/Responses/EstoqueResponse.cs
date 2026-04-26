namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class EstoqueResponse
    {
        public Guid Id { get; set; }
        public Guid IdPecaInsumo { get; set; }
        public int Quantidade { get; set; }

        public EstoqueResponse(Guid id, Guid idPecaInsumo, int quantidade)
        {
            Id = id;
            IdPecaInsumo = idPecaInsumo;
            Quantidade = quantidade;
        }
    }
}
