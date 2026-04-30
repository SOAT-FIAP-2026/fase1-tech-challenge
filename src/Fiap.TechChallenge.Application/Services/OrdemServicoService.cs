using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private const string StatusInicialDescricao = "Recebida";

        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IStatusOrdemServicoRepository _statusRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IPecaInsumoRepository _pecaInsumoRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;

        public OrdemServicoService(
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IStatusOrdemServicoRepository statusRepository,
            IServicoRepository servicoRepository,
            IPecaInsumoRepository pecaInsumoRepository,
            IOrdemServicoRepository ordemServicoRepository)
        {
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _statusRepository = statusRepository;
            _servicoRepository = servicoRepository;
            _pecaInsumoRepository = pecaInsumoRepository;
            _ordemServicoRepository = ordemServicoRepository;
        }

        public async Task<Guid> Criar(OrdemServicoRequest request)
        {
            await GarantirClienteExiste(request.ClienteId);
            await GarantirVeiculoExiste(request.VeiculoId);
            StatusOrdemServico statusInicial = await GarantirStatusInicialExiste();

            IReadOnlyCollection<Servico> servicos = await ObterServicos(request.ServicosIds);
            IReadOnlyCollection<PecaInsumo> pecas = await ObterPecas(request.PecasInsumoIds);

            OrdemServico ordemServico = new(request.ClienteId, request.VeiculoId, statusInicial.Id, request.Observacao);

            ordemServico.SincronizarItens(servicos, pecas);

            await _ordemServicoRepository.Adicionar(ordemServico);

            return ordemServico.Id;
        }

        public async Task<OrdemServicoResponse> IncluirItens(Guid id, OrdemServicoItensRequest request)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            IReadOnlyCollection<Guid> servicosIds = [.. ordemServico.ItensServico.Select(item => item.IdServico).Concat(request.ServicosIds).Distinct()];
            IReadOnlyCollection<Guid> pecasIds = [.. ordemServico.ItensPecaInsumo.Select(item => item.IdPecaInsumo).Concat(request.PecasInsumoIds).Distinct()];

            IReadOnlyCollection<Servico> servicos = await _servicoRepository.ObterPorIds(servicosIds);
            ValidarEntidadesEncontradas(servicosIds, servicos.Select(s => s.Id), idServico => new ServicoNaoEncontradoException(idServico));

            IReadOnlyCollection<PecaInsumo> pecas = await _pecaInsumoRepository.ObterPorIds(pecasIds);
            ValidarEntidadesEncontradas(pecasIds, pecas.Select(p => p.Id), idPeca => new PecaInsumoNaoEncontradaException(idPeca));

            ordemServico.SincronizarItens(servicos, pecas);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }

        public async Task<OrdemServicoResponse> ObterPorId(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            return ToResponse(ordemServico);
        }

        public async Task<IReadOnlyCollection<OrdemServicoResponse>> ObterTodos()
        {
            IReadOnlyCollection<OrdemServico> ordensServico = await _ordemServicoRepository.ObterTodos();
            return [.. ordensServico.Select(ToResponse)];
        }

        private async Task GarantirClienteExiste(Guid clienteId)
        {
            if (await _clienteRepository.ObterPorId(clienteId) == null)
                throw new ClienteNaoEncontradoException(clienteId);
        }

        private async Task GarantirVeiculoExiste(Guid veiculoId)
        {
            if (await _veiculoRepository.ObterPorId(veiculoId) == null)
                throw new VeiculoNaoEncontradoException(veiculoId);
        }

        private async Task<StatusOrdemServico> GarantirStatusInicialExiste()
        {
            StatusOrdemServico? status = await _statusRepository.ObterPorDescricao(StatusInicialDescricao);

            if (status == null)
                throw new StatusOrdemServicoNaoEncontradoException(StatusInicialDescricao);

            return status;
        }

        private async Task<IReadOnlyCollection<Servico>> ObterServicos(IReadOnlyCollection<Guid> servicosIds)
        {
            IReadOnlyCollection<Servico> servicos = await _servicoRepository.ObterPorIds(servicosIds);
            ValidarEntidadesEncontradas(servicosIds, servicos.Select(s => s.Id), id => new ServicoNaoEncontradoException(id));
            return servicos;
        }

        private async Task<IReadOnlyCollection<PecaInsumo>> ObterPecas(IReadOnlyCollection<Guid> pecasIds)
        {
            IReadOnlyCollection<PecaInsumo> pecas = await _pecaInsumoRepository.ObterPorIds(pecasIds);
            ValidarEntidadesEncontradas(pecasIds, pecas.Select(p => p.Id), id => new PecaInsumoNaoEncontradaException(id));
            return pecas;
        }

        private async Task<OrdemServico> ObterEntidadePorId(Guid id)
        {
            OrdemServico? ordemServico = await _ordemServicoRepository.ObterPorId(id);

            if (ordemServico == null)
                throw new OrdemServicoNaoEncontradaException(id);

            return ordemServico;
        }

        private static OrdemServicoResponse ToResponse(OrdemServico ordemServico)
        {
            return new OrdemServicoResponse(
                ordemServico.Id,
                ordemServico.IdCliente,
                ordemServico.IdVeiculo,
                ordemServico.Cliente.Nome.Valor,
                ordemServico.IdStatus,
                ordemServico.Status?.Descricao.Valor ?? string.Empty,
                ordemServico.Observacao,
                ordemServico.DataAbertura,
                ordemServico.DataConclusao,
                ordemServico.Orcamento?.ValorTotal.Valor,
                [.. ordemServico.ItensServico.Select(item => new OrdemServicoItemServicoResponse(item.Id, item.IdServico, item.DataHoraInicio, item.DataHoraFim))],            
                [.. ordemServico.ItensPecaInsumo.Select(item => new OrdemServicoItemPecaInsumoResponse(item.Id, item.IdPecaInsumo))]
            );
        }

        private static void ValidarEntidadesEncontradas(
            IReadOnlyCollection<Guid> idsSolicitados,
            IEnumerable<Guid> idsEncontrados,
            Func<Guid, Exception> exceptionFactory)
        {
            HashSet<Guid> idsEncontradosSet = idsEncontrados.ToHashSet();

            foreach (Guid id in idsSolicitados.Distinct())
            {
                if (!idsEncontradosSet.Contains(id))
                    throw exceptionFactory(id);
            }
        }
    }
}
