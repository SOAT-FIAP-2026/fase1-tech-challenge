using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/ordens-servico")]
    public class OrdemServicoController(IOrdemServicoService ordemServicoService) : ControllerBase
    {
        private readonly IOrdemServicoService _ordemServicoService = ordemServicoService;

        [HttpPost]
        public async Task<IActionResult> Criar(OrdemServicoRequest request)
        {
            Guid id = await _ordemServicoService.Criar(request);

            return StatusCode(201, new
            {
                id
            });
        }

        [HttpPost("{id:guid}/itens")]
        public async Task<IActionResult> IncluirItens(Guid id, OrdemServicoItensRequest request)
        {
            var ordemServico = await _ordemServicoService.IncluirItens(id, request);

            return Ok(ordemServico);
        }

        [HttpPost("{id:guid}/iniciar-diagnostico")]
        public async Task<IActionResult> IniciarDiagnostico(Guid id)
        {
            var ordemServico = await _ordemServicoService.IniciarDiagnostico(id);

            return Ok(ordemServico);
        }

        [HttpPost("{id:guid}/finalizar-diagnostico")]
        public async Task<IActionResult> FinalizarDiagnostico(Guid id)
        {
            var ordemServico = await _ordemServicoService.FinalizarDiagnostico(id);

            return Ok(ordemServico);
        }

        [HttpDelete("{id:guid}/itens/servicos/{idServico:guid}")]
        public async Task<IActionResult> RemoverItemServico(Guid id, Guid idServico)
        {
            await _ordemServicoService.RemoverItemServico(id, idServico);

            return NoContent();
        }

        [HttpDelete("{id:guid}/itens/pecas-insumos/{idPecaInsumo:guid}")]
        public async Task<IActionResult> RemoverItemPecaInsumo(Guid id, Guid idPecaInsumo)
        {
            await _ordemServicoService.RemoverItemPecaInsumo(id, idPecaInsumo);

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var ordemServico = await _ordemServicoService.ObterPorId(id);

            return Ok(ordemServico);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var ordensServico = await _ordemServicoService.ObterTodos();

            return Ok(ordensServico);
        }
    }
}
