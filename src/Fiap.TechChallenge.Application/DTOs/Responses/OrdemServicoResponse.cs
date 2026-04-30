namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoResponse(
        Guid id,
        Guid idCliente,
        Guid idVeiculo,
        Guid idStatus,
        string clienteNome,
        string statusDescricao,
        string? observacao,
        DateTime dataAbertura,
        DateTime? dataConclusao,
        decimal? valorTotal,
        IReadOnlyCollection<OrdemServicoItemServicoResponse> itensServico,
        IReadOnlyCollection<OrdemServicoItemPecaInsumoResponse> itensPecaInsumo)
    {
        public Guid Id { get; set; } = id;
        public Guid IdCliente { get; set; } = idCliente;
        public Guid IdVeiculo { get; set; } = idVeiculo;
        public string ClienteNome { get; set; } = clienteNome;
        public Guid IdStatus { get; set; } = idStatus;
        public string StatusDescricao { get; set; } = statusDescricao;
        public string? Observacao { get; set; } = observacao;
        public DateTime DataAbertura { get; set; } = dataAbertura;
        public DateTime? DataConclusao { get; set; } = dataConclusao;
        public decimal? ValorTotal { get; set; } = valorTotal;
        public IReadOnlyCollection<OrdemServicoItemServicoResponse> ItensServico { get; set; } = itensServico;
        public IReadOnlyCollection<OrdemServicoItemPecaInsumoResponse> ItensPecaInsumo { get; set; } = itensPecaInsumo;
    }
}