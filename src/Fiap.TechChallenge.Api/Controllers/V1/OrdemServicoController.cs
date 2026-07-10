using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/ordens-servico")]
    public class OrdemServicoController(IOrdemServicoService ordemServicoService) : ControllerBase
    {
        private readonly IOrdemServicoService _ordemServicoService = ordemServicoService;

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Criar(OrdemServicoRequest request)
        {
            Guid id = await _ordemServicoService.Criar(request);

            return StatusCode(201, new
            {
                id
            });
        }

        [HttpPost("{id:guid}/pecas-insumos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> IncluirPecaInsumo(Guid id, OrdemServicoPecaInsumoRequest request)
        {
            var ordemServico = await _ordemServicoService.IncluirPecaInsumo(id, request);

            return Ok(ordemServico);
        }



        [HttpPost("{id:guid}/servicos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> IncluirServico(Guid id, OrdemServicoServicosRequest request)
        {
            var ordemServico = await _ordemServicoService.IncluirServico(id, request);

            return Ok(ordemServico);
        }

        [HttpPatch("{id:guid}/iniciar-diagnostico")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> IniciarDiagnostico(Guid id)
        {
            var ordemServico = await _ordemServicoService.IniciarDiagnostico(id);

            return Ok(ordemServico);
        }

        [HttpPatch("{id:guid}/finalizar-diagnostico")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> FinalizarDiagnostico(Guid id)
        {
            var ordemServico = await _ordemServicoService.FinalizarDiagnostico(id);

            return Ok(ordemServico);
        }

        [HttpPatch("{id:guid}/aprovar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Aprovar(Guid id)
        {
            var ordemServico = await _ordemServicoService.AprovarOrdemServico(id);

            return Ok(ordemServico);
        }

        [HttpDelete("{id:guid}/servicos/{idServico:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RemoverItemServico(Guid id, Guid idServico)
        {
            await _ordemServicoService.RemoverItemServico(id, idServico);

            return NoContent();
        }

        [HttpDelete("{id:guid}/pecas-insumos/{idPecaInsumo:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RemoverItemPecaInsumo(Guid id, Guid idPecaInsumo)
        {
            await _ordemServicoService.RemoverItemPecaInsumo(id, idPecaInsumo);

            return NoContent();
        }

        [HttpPatch("{id:guid}/servicos/{idServico:guid}/iniciar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> IniciarServico(Guid id, Guid idServico)
        {
            await _ordemServicoService.IniciarServico(id, idServico);

            return NoContent();
        }

        [HttpPatch("{id:guid}/servicos/{idServico:guid}/finalizar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> FinalizarServico(Guid id, Guid idServico)
        {
            await _ordemServicoService.FinalizarServico(id, idServico);

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var ordemServico = await _ordemServicoService.ObterPorId(id);

            return Ok(ordemServico);
        }

        [HttpGet("{id:guid}/progresso")]
        public async Task<IActionResult> ObterProgresso(Guid id)
        {
            var progresso = await _ordemServicoService.ObterProgresso(id);

            return Ok(progresso);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterTodos()
        {
            var ordensServico = await _ordemServicoService.ObterTodos();

            return Ok(ordensServico);
        }
    }
}
