using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/servico")]
    public class ServicoController(IServicoService servicoService) : ControllerBase
    {
        private readonly IServicoService _servicoService = servicoService;

        [HttpPost]
        public async Task<IActionResult> Criar(ServicoRequest request)
        {
            Guid id = await _servicoService.Criar(request);

            return StatusCode(201, new
            {
                id
            });

        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var servico = await _servicoService.ObterPorId(id);

            return Ok(servico);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var servicos = await _servicoService.ObterTodos();

            return Ok(servicos);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ServicoRequest request)
        {
            var servico = await _servicoService.Atualizar(id, request);

            return Ok(servico);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _servicoService.Deletar(id);

            return NoContent();
        }
    }
}
