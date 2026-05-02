using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/pecas-insumos")]
    public class PecaInsumoController(IPecaInsumoService pecaInsumoService) : ControllerBase
    {
        private readonly IPecaInsumoService _pecaInsumoService = pecaInsumoService;

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Criar(PecaInsumoRequest request)
        {
            Guid id = await _pecaInsumoService.Criar(request);

            return Created("Criar", id);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var pecaInsumo = await _pecaInsumoService.ObterPorId(id);

            return Ok(pecaInsumo);
        }

        [HttpGet("buscar-por-descricao")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterPorDescricao([FromQuery] string descricao)
        {
            var pecaInsumo = await _pecaInsumoService.ObterPorDescricao(descricao);

            return Ok(pecaInsumo);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObterTodos()
        {
            var pecasInsumos = await _pecaInsumoService.ObterTodos();

            return Ok(pecasInsumos);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(Guid id, PecaInsumoRequest request)
        {
            var pecaInsumo = await _pecaInsumoService.Atualizar(id, request);

            return Ok(pecaInsumo);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _pecaInsumoService.Deletar(id);

            return NoContent();
        }
    }
}
