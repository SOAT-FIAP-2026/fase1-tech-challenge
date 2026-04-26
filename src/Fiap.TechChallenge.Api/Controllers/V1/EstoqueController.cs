using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/estoques")]
    public class EstoqueController(IEstoqueService estoqueService) : ControllerBase
    {
        private readonly IEstoqueService _estoqueService = estoqueService;

        [HttpPost]
        public async Task<IActionResult> Criar(EstoqueRequest request)
        {
            Guid id = await _estoqueService.Criar(request);

            return StatusCode(201, new
            {
                id
            });
        }

        [HttpGet("quantidade/peca/{idPecaInsumo:guid}")]
        public async Task<IActionResult> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo)
        {
            int? quantidade = await _estoqueService.VerificarQuantidadePorIdPecaInsumo(idPecaInsumo);

            if (quantidade == null)
                return NotFound(new { mensagem = "Estoque não encontrado para esta peça/insumo." });

            return Ok(new { quantidade });
        }

        [HttpGet("quantidade/descricao")]
        public async Task<IActionResult> VerificarQuantidadePorDescricaoPeca([FromQuery] string descricao)
        {
            int? quantidade = await _estoqueService.VerificarQuantidadePorDescricaoPeca(descricao);

            if (quantidade == null)
                return NotFound(new { mensagem = $"Estoque não encontrado para peça/insumo com descrição contendo '{descricao}'." });

            return Ok(new { quantidade });
        }

        [HttpPut("{idPecaInsumo:guid}/adicionar-quantidade")]
        public async Task<IActionResult> AdicionarQuantidade(Guid idPecaInsumo, [FromBody] int quantidade)
        {
            await _estoqueService.AdicionarQuantidade(idPecaInsumo, quantidade);

            return Ok(new { mensagem = "Quantidade adicionada com sucesso." });
        }

        [HttpPut("{idPecaInsumo:guid}/remover-quantidade")]
        public async Task<IActionResult> RemoverQuantidade(Guid idPecaInsumo, [FromBody] int quantidade)
        {
            await _estoqueService.RemoverQuantidade(idPecaInsumo, quantidade);

            return Ok(new { mensagem = "Quantidade removida com sucesso." });
        }

        [HttpDelete("{idEstoque:guid}")]
        public async Task<IActionResult> Deletar(Guid idEstoque)
        {
            await _estoqueService.Deletar(idEstoque);

            return NoContent();
        }
    }
}
