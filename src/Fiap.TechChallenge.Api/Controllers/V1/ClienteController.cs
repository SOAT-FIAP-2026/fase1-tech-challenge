using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/clientes")]
    public class ClienteController(IClienteService clienteService) : ControllerBase
    {
        private readonly IClienteService _clienteService = clienteService;

        [HttpPost]
        public async Task<IActionResult> Criar(ClienteRequest request)
        {
            Guid id = await _clienteService.Criar(request);

            return StatusCode(201, new
            {
                id
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cliente = await _clienteService.ObterPorId(id);

            return Ok(cliente);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _clienteService.ObterTodos();

            return Ok(clientes);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ClienteRequest request)
        {
            var cliente = await _clienteService.Atualizar(id, request);

            return Ok(cliente);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _clienteService.Deletar(id);

            return NoContent();
        }
    }
}
