using Fiap.TechChallenge.Application.DTOs.Common;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/veiculos")]
    [Produces("application/json")]
    public class VeiculoController(IVeiculoService veiculoService) : ControllerBase
    {
        private readonly IVeiculoService _veiculoService = veiculoService;

        /// <summary>
        /// Cria um novo veículo
        /// </summary>
        /// <param name="request">Dados do veículo</param>
        /// <returns>ID do veículo criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Criar([FromBody] VeiculoRequest request)
        {
            Guid id = await _veiculoService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }

        /// <summary>
        /// Obtém um veículo pelo ID
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Dados do veículo</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var veiculo = await _veiculoService.ObterPorId(id);

            return Ok(veiculo);
        }

        /// <summary>
        /// Lista veículos com paginação
        /// </summary>
        /// <param name="request">Parâmetros de paginação</param>
        /// <returns>Lista paginada de veículos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<VeiculoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Listar([FromQuery] PagedRequest request)
        {
            var veiculos = await _veiculoService.ListarPaginado(request);

            return Ok(veiculos);
        }

        /// <summary>
        /// Atualiza parcialmente um veículo
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="request">Campos a serem atualizados</param>
        /// <returns>Dados atualizados do veículo</returns>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] VeiculoPatchRequest request)
        {
            var veiculo = await _veiculoService.Atualizar(id, request);

            return Ok(veiculo);
        }

        /// <summary>
        /// Remove um veículo (soft delete)
        /// </summary>
        /// <param name="id">ID do veículo</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar([FromRoute] Guid id)
        {
            await _veiculoService.Deletar(id);

            return NoContent();
        }
    }
}
