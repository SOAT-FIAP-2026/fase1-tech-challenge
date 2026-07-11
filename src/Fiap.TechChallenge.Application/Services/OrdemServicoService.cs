using System.ComponentModel;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.Interfaces.Service;
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
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IEmailService _emailService;


        public OrdemServicoService(
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IStatusOrdemServicoRepository statusRepository,
            IServicoRepository servicoRepository,
            IPecaInsumoRepository pecaInsumoRepository,
            IEmailService emailService,
            IEstoqueRepository estoqueRepository,
            IOrdemServicoRepository ordemServicoRepository)
        {
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _statusRepository = statusRepository;
            _servicoRepository = servicoRepository;
            _pecaInsumoRepository = pecaInsumoRepository;
            _estoqueRepository = estoqueRepository;
            _ordemServicoRepository = ordemServicoRepository;
            _emailService = emailService;
        }

        public async Task<(Guid Id, bool ClienteNotificado)> Criar(OrdemServicoRequest request)
        {
            await GarantirClienteExiste(request.ClienteId);
            await GarantirVeiculoExiste(request.VeiculoId);
            StatusOrdemServico statusInicial = await GarantirStatusExiste(StatusOS.Recebida);

            OrdemServico ordemServico = new(request.ClienteId, request.VeiculoId, statusInicial.Id, request.Observacao);
            ordemServico.AlterarStatus(statusInicial);

            await _ordemServicoRepository.Adicionar(ordemServico);
            bool notificado = await NotificarMudancaStatusAsync(ordemServico);

            return (ordemServico.Id, notificado);
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
            bool notificado = await NotificarMudancaStatusAsync(ordemServico);

            var response = ToResponse(ordemServico);
            response.ClienteNotificado = notificado;
            return response;
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
            bool notificado = await NotificarMudancaStatusAsync(ordemServico);

            var response = ToResponse(ordemServico);
            response.ClienteNotificado = notificado;
            return response;
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

            HashSet<Guid> pecasExistentesIds = ordemServico.ItensPecaInsumo.Select(item => item.IdPecaInsumo).ToHashSet();
            IReadOnlyCollection<Guid> pecasNovasIds = [.. request.PecasInsumosIds.Distinct().Where(idPeca => !pecasExistentesIds.Contains(idPeca))];
            IReadOnlyCollection<Guid> pecasIds = [.. pecasExistentesIds.Concat(request.PecasInsumosIds).Distinct()];
            IReadOnlyCollection<PecaInsumo> pecas = await _pecaInsumoRepository.ObterPorIds(pecasIds);
            ValidarEntidadesEncontradas(pecasIds, pecas.Select(p => p.Id), idPeca => new PecaInsumoNaoEncontradaException(idPeca));

            await BaixarEstoqueDasPecas(pecasNovasIds);

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
            ordemServico.AlterarStatus(statusEmExecucao);

            await _ordemServicoRepository.Atualizar(ordemServico);
            await NotificarMudancaStatusAsync(ordemServico);
        }

        public async Task FinalizarServico(Guid id, Guid idServico)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);
            ItemServico item = ObterItemServico(ordemServico, idServico);

            item.FinalizarServico();

            if (ordemServico.ItensServico.All(servico => servico.DataHoraFim != null))
            {
                StatusOrdemServico statusFinalizada = await GarantirStatusExiste(StatusOS.Finalizada);
                ordemServico.AlterarStatus(statusFinalizada);
                ordemServico.Concluir();
            }

            await _ordemServicoRepository.Atualizar(ordemServico);
            await NotificarMudancaStatusAsync(ordemServico);
        }

        public async Task<OrdemServicoResponse> AprovarOrcamento(Guid id, bool aprovado)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusAguardandoAprovacao = await GarantirStatusExiste(StatusOS.AguardandoAprovacao);

            if (ordemServico.IdStatus != statusAguardandoAprovacao.Id)
                throw new InvalidOperationException("A ordem de serviço precisa estar no status Aguardando aprovação para ser aprovada.");

            if (aprovado)
            {
                StatusOrdemServico statusOrcamentoAprovado = await GarantirStatusExiste(StatusOS.OrcamentoAprovado);
                ordemServico.AlterarStatus(statusOrcamentoAprovado);
            }
            else
            {
                StatusOrdemServico statusOrcamentoReprovado = await GarantirStatusExiste(StatusOS.OrcamentoReprovado);
                ordemServico.AlterarStatus(statusOrcamentoReprovado);
                ordemServico.Concluir();
            }

            await _ordemServicoRepository.Atualizar(ordemServico);
            bool notificado = await NotificarMudancaStatusAsync(ordemServico);

            var response = ToResponse(ordemServico);
            response.ClienteNotificado = notificado;
            return response;
        }

        public async Task<OrdemServicoResponse> ConfirmarEntrega(Guid id)
        {
            OrdemServico ordemServico = await ObterEntidadePorId(id);

            StatusOrdemServico statusEntregue = await GarantirStatusExiste(StatusOS.Entregue);

            if (ordemServico.IdStatus == statusEntregue.Id)
                throw new InvalidOperationException("A ordem de serviço já foi entregue.");

            StatusOrdemServico statusFinalizada = await GarantirStatusExiste(StatusOS.Finalizada);
            if (ordemServico.IdStatus != statusFinalizada.Id)
                throw new InvalidOperationException("A ordem de serviço não pode ser entregue, pois não foi finalizada.");

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

        private async Task<bool> NotificarMudancaStatusAsync(OrdemServico ordemServico)
        {
            try
            {
                var cliente = await _clienteRepository.ObterPorId(ordemServico.IdCliente);
                if (cliente != null && ordemServico.Status != null)
                {
                    string assunto = $"Status da sua Ordem de Serviço mudou para {ordemServico.Status.Descricao.Valor}";
                    string corpoHtml = $$"""
                        <!DOCTYPE html>
                        <html lang="pt-BR">
                        <head>
                          <meta charset="UTF-8">
                          <style>
                            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0; }
                            .container { max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
                            .header { background-color: #0056b3; padding: 20px; text-align: center; color: #ffffff; }
                            .header h1 { margin: 0; font-size: 24px; font-weight: 600; letter-spacing: 0.5px; }
                            .content { padding: 30px; color: #333333; line-height: 1.6; font-size: 16px; }
                            .status-box { background-color: #e9f2fb; border-left: 4px solid #0056b3; padding: 20px; margin: 25px 0; border-radius: 0 4px 4px 0; text-align: center; }
                            .status-label { font-size: 14px; color: #555555; text-transform: uppercase; letter-spacing: 1px; }
                            .status-text { display: block; font-size: 22px; font-weight: bold; color: #0056b3; margin-top: 5px; text-transform: uppercase; }
                            .footer { background-color: #f9f9f9; padding: 20px; text-align: center; font-size: 13px; color: #777777; border-top: 1px solid #eeeeee; }
                          </style>
                        </head>
                        <body>
                          <div class="container">
                            <div class="header">
                              <h1>Atualização de Serviço</h1>
                            </div>
                            <div class="content">
                              <p>Olá, <strong>{{cliente.Nome.Valor}}</strong>!</p>
                              <p>Passando para informar que temos uma atualização sobre o andamento da manutenção do seu veículo.</p>
                              <div class="status-box">
                                <span class="status-label">Status atual da sua ordem de serviço</span>
                                <span class="status-text">{{ordemServico.Status.Descricao.Valor}}</span>
                              </div>
                              <p>Agradecemos a confiança em nossos serviços. Em caso de dúvidas, não hesite em entrar em contato com a nossa equipe.</p>
                            </div>
                            <div class="footer">
                              Esta é uma mensagem automática, por favor não responda a este e-mail.<br/>
                              Tech Challenge Auto Center &copy; {{DateTime.Now.Year}}
                            </div>
                          </div>
                        </body>
                        </html>
                        """;
                    return await _emailService.EnviarEmailAsync(cliente.Email.Endereco, assunto, corpoHtml);
                }
            }
            catch (Exception)
            {
                // Ignora falhas no envio de e-mail para não quebrar a transação ou retornar erro 500
            }

            return false;
            
        }
        
        private async Task BaixarEstoqueDasPecas(IReadOnlyCollection<Guid> pecasIds)
        {
            foreach (Guid idPecaInsumo in pecasIds)
            {
                Estoque? estoque = await _estoqueRepository.ObterPorIdPecaInsumo(idPecaInsumo);

                if (estoque == null)
                    throw new EstoqueNaoEncontradoException(idPecaInsumo);

                estoque.RemoverQuantidade(1);
            }
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
