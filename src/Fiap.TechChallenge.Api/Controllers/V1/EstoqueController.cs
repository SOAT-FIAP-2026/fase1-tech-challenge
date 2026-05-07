using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/estoques")]
    public class EstoqueController(IEstoqueService estoqueService) : ControllerBase
    {
        private readonly IEstoqueService _estoqueService = estoqueService;

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Criar(EstoqueRequest request)
        {
            Guid id = await _estoqueService.Criar(request);

            return Created("Criar", id);
        }

        [HttpGet("quantidade/peca/{idPecaInsumo:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo)
        {
            int? quantidade = await _estoqueService.VerificarQuantidadePorIdPecaInsumo(idPecaInsumo);

            if (quantidade == null)
                return NotFound("Estoque não encontrado para esta peça/insumo.");

            return Ok(quantidade);
        }

        [HttpGet("quantidade/descricao")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> VerificarQuantidadePorDescricaoPeca([FromQuery] string descricao)
        {
            return await VerificarQuantidadePorDescricao(descricao);
        }

        [HttpGet("quantidade/descricao/{descricao}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> VerificarQuantidadePorDescricaoPecaRota([FromRoute] string descricao)
        {
            return await VerificarQuantidadePorDescricao(descricao);
        }

        private async Task<IActionResult> VerificarQuantidadePorDescricao(string descricao)
        {
            int? quantidade = await _estoqueService.VerificarQuantidadePorDescricaoPeca(descricao);

            if (quantidade == null)
                return NotFound($"Estoque não encontrado para peça/insumo com descrição contendo '{descricao}'.");

            return Ok(quantidade);
        }

        [HttpPut("{idPecaInsumo:guid}/adicionar-quantidade")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AdicionarQuantidade(Guid idPecaInsumo, [FromBody] int quantidade)
        {
            await _estoqueService.AdicionarQuantidade(idPecaInsumo, quantidade);

            return Ok("Quantidade adicionada com sucesso.");
        }

        [HttpPut("{idPecaInsumo:guid}/remover-quantidade")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RemoverQuantidade(Guid idPecaInsumo, [FromBody] int quantidade)
        {
            await _estoqueService.RemoverQuantidade(idPecaInsumo, quantidade);

            return Ok("Quantidade removida com sucesso.");
        }

        [HttpDelete("{idEstoque:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deletar(Guid idEstoque)
        {
            await _estoqueService.Deletar(idEstoque);

            return NoContent();
        }
    }
}
