using System.ComponentModel;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
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
            StatusOrdemServico statusInicial = await GarantirStatusExiste(StatusOS.Recebida);

            OrdemServico ordemServico = new(request.ClienteId, request.VeiculoId, statusInicial.Id, request.Observacao);

            await _ordemServicoRepository.Adicionar(ordemServico);

            return ordemServico.Id;
        }

        public async Task<OrdemServicoResponse> IniciarDiagnostico(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusRecebida = await GarantirStatusExiste(StatusOS.Recebida);

            if (ordemServico.IdStatus != statusRecebida.Id)
                throw new InvalidOperationException("A ordem de serviço precisa estar no status Recebida para iniciar o diagnóstico.");

            StatusOrdemServico statusEmDiagnostico = await GarantirStatusExiste(StatusOS.EmDiagnostico);

            ordemServico.AlterarStatus(statusEmDiagnostico);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }

        public async Task<OrdemServicoResponse> FinalizarDiagnostico(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusEmDiagnostico = await GarantirStatusExiste(StatusOS.EmDiagnostico);

            if (ordemServico.IdStatus != statusEmDiagnostico.Id)
                throw new InvalidOperationException("A ordem de serviço precisa estar no status Em Diagnóstico para finalizar o diagnóstico.");

            StatusOrdemServico statusAguardandoAprovacao = await GarantirStatusExiste(StatusOS.AguardandoAprovacao);

            ordemServico.AlterarStatus(statusAguardandoAprovacao);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }

        public async Task<OrdemServicoResponse> IncluirServico(Guid id, OrdemServicoServicosRequest request)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            IReadOnlyCollection<Guid> servicosIds = [.. ordemServico.ItensServico.Select(item => item.IdServico).Concat(request.ServicosIds).Distinct()];

            IReadOnlyCollection<Servico> servicos = await _servicoRepository.ObterPorIds(servicosIds);
            ValidarEntidadesEncontradas(servicosIds, servicos.Select(s => s.Id), idServico => new ServicoNaoEncontradoException(idServico));

            ordemServico.SincronizarItens(servicos, [.. ordemServico.ItensPecaInsumo.Select(item => item.PecaInsumo).Where(p => p != null).Select(p => p!)]);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);

        }

        public async Task<OrdemServicoResponse> IncluirPecaInsumo(Guid id, OrdemServicoPecaInsumoRequest request)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            IReadOnlyCollection<Guid> pecasIds = [.. ordemServico.ItensPecaInsumo.Select(item => item.IdPecaInsumo).Concat(request.PecasInsumosIds).Distinct()];
            IReadOnlyCollection<PecaInsumo> pecas = await _pecaInsumoRepository.ObterPorIds(pecasIds);
            ValidarEntidadesEncontradas(pecasIds, pecas.Select(p => p.Id), idPeca => new PecaInsumoNaoEncontradaException(idPeca));

            ordemServico.SincronizarItens([.. ordemServico.ItensServico.Select(item => item.Servico).Where(s => s != null).Select(s => s!)], pecas);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }

        public async Task RemoverItemServico(Guid id, Guid idServico)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            if (!ordemServico.RemoverItemServico(idServico))
                throw new ItemServicoNaoEncontradoException(idServico);

            await RecalcularOrcamentoDaOrdem(ordemServico);
            await _ordemServicoRepository.Atualizar(ordemServico);
        }

        public async Task RemoverItemPecaInsumo(Guid id, Guid idPecaInsumo)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            if (!ordemServico.RemoverItemPecaInsumo(idPecaInsumo))
                throw new ItemPecaInsumoNaoEncontradoException(idPecaInsumo);

            await RecalcularOrcamentoDaOrdem(ordemServico);
            await _ordemServicoRepository.Atualizar(ordemServico);
        }

        public async Task IniciarServico(Guid id, Guid idServico)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            ItemServico item = ObterItemServico(ordemServico, idServico);
            StatusOrdemServico statusEmExecucao = await GarantirStatusExiste(StatusOS.EmExecucao);

            item.IniciarServico();
            ordemServico.AlterarStatus(statusEmExecucao.Id);

            await _ordemServicoRepository.Atualizar(ordemServico);
        }

        public async Task FinalizarServico(Guid id, Guid idServico)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            ItemServico item = ObterItemServico(ordemServico, idServico);

            item.FinalizarServico();

            if (ordemServico.ItensServico.All(servico => servico.DataHoraFim != null))
            {
                StatusOrdemServico statusFinalizada = await GarantirStatusExiste(StatusOS.Finalizada);
                ordemServico.AlterarStatus(statusFinalizada.Id);
                ordemServico.Concluir();
            }

            await _ordemServicoRepository.Atualizar(ordemServico);
        }

        public async Task<OrdemServicoResponse> AprovarOrdemServico(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusAguardandoAprovacao = await GarantirStatusExiste(StatusOS.AguardandoAprovacao);

            if (ordemServico.IdStatus != statusAguardandoAprovacao.Id)
                throw new InvalidOperationException("A ordem de serviço precisa estar no status Aguardando aprovação para ser aprovada.");

            StatusOrdemServico statusEmExecucao = await GarantirStatusExiste(StatusOS.EmExecucao);

            ordemServico.AlterarStatus(statusEmExecucao);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }

        public async Task<OrdemServicoResponse> ConfirmarEntrega(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusEntregue = await GarantirStatusExiste(StatusOS.Entregue);

            if (ordemServico.IdStatus == statusEntregue.Id)
                throw new InvalidOperationException("A ordem de serviço já foi entregue.");

            ordemServico.AlterarStatus(statusEntregue.Id);

            await _ordemServicoRepository.Atualizar(ordemServico);

            return ToResponse(ordemServico);
        }


        public async Task<OrdemServicoResponse> ObterPorId(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            return ToResponse(ordemServico);
        }

        public async Task<OrdemServicoProgressoResponse> ObterProgresso(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            return ToProgressoResponse(ordemServico);
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

        private async Task<StatusOrdemServico> GarantirStatusExiste(StatusOS statusOS)
        {
            StatusOrdemServico? status = await _statusRepository.ObterPorCodigo(new CodigoVO(statusOS.Codigo));

            if (status == null)
                throw new StatusOrdemServicoNaoEncontradoException(statusOS.Codigo);

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
            OrdemServico? ordemServico = await _ordemServicoRepository.ObterPorId(id) ?? throw new OrdemServicoNaoEncontradaException(id);
            return ordemServico;
        }

        private static ItemServico ObterItemServico(OrdemServico ordemServico, Guid idServico)
        {
            ItemServico? item = ordemServico.ItensServico.FirstOrDefault(item => item.IdServico == idServico);

            if (item == null)
                throw new ItemServicoNaoEncontradoException(idServico);

            return item;
        }

        private async Task RecalcularOrcamentoDaOrdem(OrdemServico ordemServico)
        {
            IReadOnlyCollection<Guid> servicosIds = [.. ordemServico.ItensServico.Select(item => item.IdServico)];
            IReadOnlyCollection<Guid> pecasIds = [.. ordemServico.ItensPecaInsumo.Select(item => item.IdPecaInsumo)];

            IReadOnlyCollection<Servico> servicos = servicosIds.Count == 0
                ? []
                : await _servicoRepository.ObterPorIds(servicosIds);

            ValidarEntidadesEncontradas(servicosIds, servicos.Select(s => s.Id), idServico => new ServicoNaoEncontradoException(idServico));

            IReadOnlyCollection<PecaInsumo> pecas = pecasIds.Count == 0
                ? []
                : await _pecaInsumoRepository.ObterPorIds(pecasIds);

            ValidarEntidadesEncontradas(pecasIds, pecas.Select(p => p.Id), idPeca => new PecaInsumoNaoEncontradaException(idPeca));

            ordemServico.RecalcularOrcamento(servicos, pecas);
        }

        private static OrdemServicoResponse ToResponse(OrdemServico ordemServico)
        {
            return new OrdemServicoResponse(
                ordemServico.Id,
                ordemServico.IdCliente,
                ordemServico.IdVeiculo,
                ordemServico.IdStatus,
                ordemServico.Cliente?.Nome?.Valor ?? string.Empty,
                ordemServico.Status?.Descricao.Valor ?? string.Empty,
                ordemServico.Observacao,
                ordemServico.DataAbertura,
                ordemServico.DataConclusao,
                ordemServico.Orcamento?.ValorTotal.Valor,
                [.. ordemServico.ItensServico.Select(item => new OrdemServicoItemServicoResponse(
                    item.IdServico,
                    item.DataHoraInicio,
                    item.DataHoraFim,
                    item.Servico?.Nome?.Valor ?? string.Empty,
                    item.Servico?.ValorUnitario.Valor ?? 0m))],
                [.. ordemServico.ItensPecaInsumo.Select(item => new OrdemServicoItemPecaInsumoResponse(
                    item.IdPecaInsumo,
                    item.PecaInsumo?.Descricao?.Valor ?? string.Empty,
                    item.PecaInsumo?.ValorUnitario.Valor ?? 0m))]
            );
        }

        private static OrdemServicoProgressoResponse ToProgressoResponse(OrdemServico ordemServico)
        {
            int tempoTotalEstimado = ordemServico.ItensServico.Sum(item => item.Servico?.TempoEstimadoMinutos ?? 0);
            int progressoEmMinutos = ordemServico.ItensServico.Sum(CalcularProgressoItemEmMinutos);
            int percentualConcluido = tempoTotalEstimado == 0
                ? 100
                : Math.Clamp((int)Math.Round(progressoEmMinutos * 100m / tempoTotalEstimado), 0, 100);

            DateTime? previsaoConclusao = tempoTotalEstimado == 0
                ? null
                : ordemServico.DataAbertura.AddMinutes(tempoTotalEstimado);

            return new OrdemServicoProgressoResponse(
                ordemServico.Id,
                ordemServico.Status?.Descricao.Valor ?? string.Empty,
                percentualConcluido,
                ordemServico.DataAbertura,
                previsaoConclusao,
                [.. ordemServico.ItensServico.Select(item => new OrdemServicoProgressoServicoResponse(
                    item.IdServico,
                    item.Servico?.Nome?.Valor ?? string.Empty,
                    ObterStatusItemServico(item),
                    item.Servico?.TempoEstimadoMinutos ?? 0,
                    item.ObterTempoExecutadoMinutos(),
                    item.DataHoraInicio,
                    item.DataHoraFim))]
            );
        }

        private static int CalcularProgressoItemEmMinutos(ItemServico item)
        {
            int tempoEstimado = item.Servico?.TempoEstimadoMinutos ?? 0;

            if (item.DataHoraFim != null)
                return tempoEstimado;

            if (item.DataHoraInicio == null)
                return 0;

            int tempoExecutado = item.ObterTempoExecutadoMinutos() ?? 0;
            return Math.Min(tempoExecutado, tempoEstimado);
        }

        private static string ObterStatusItemServico(ItemServico item)
        {
            if (item.DataHoraFim != null)
                return "Finalizado";

            if (item.DataHoraInicio != null)
                return "Em execução";

            return "Pendente";
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
