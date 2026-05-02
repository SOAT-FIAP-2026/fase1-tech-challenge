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

        [HttpPost("{id:guid}/itens")]
        [Authorize(Roles = "Administrador")]

        public async Task<IActionResult> IncluirItens(Guid id, OrdemServicoItensRequest request)
        {
            var ordemServico = await _ordemServicoService.IncluirItens(id, request);

            return Ok(ordemServico);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var ordemServico = await _ordemServicoService.ObterPorId(id);

            return Ok(ordemServico);
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
