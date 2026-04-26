using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/pecas-insumos")]
    public class PecaInsumoController(IPecaInsumoService pecaInsumoService) : ControllerBase
    {
        private readonly IPecaInsumoService _pecaInsumoService = pecaInsumoService;

        [HttpPost]
        public async Task<IActionResult> Criar(PecaInsumoRequest request)
        {
            Guid id = await _pecaInsumoService.Criar(request);

            return StatusCode(201, new
            {
                id
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var pecaInsumo = await _pecaInsumoService.ObterPorId(id);

            return Ok(pecaInsumo);
        }

        [HttpGet("buscar-por-descricao")]
        public async Task<IActionResult> ObterPorDescricao([FromQuery] string descricao)
        {
            var pecaInsumo = await _pecaInsumoService.ObterPorDescricao(descricao);

            return Ok(pecaInsumo);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var pecasInsumos = await _pecaInsumoService.ObterTodos();

            return Ok(pecasInsumos);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, PecaInsumoRequest request)
        {
            var pecaInsumo = await _pecaInsumoService.Atualizar(id, request);

            return Ok(pecaInsumo);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _pecaInsumoService.Deletar(id);

            return NoContent();
        }
    }
}
