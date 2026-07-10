using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public AutenticacaoController(
            IAutenticacaoService autenticacaoService
        )
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpPost("Cadastrar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Cadastrar(CadastrarRequest request)
        {
            Guid idUsuario = await _autenticacaoService.Cadastrar(request);

            return Created("Cadastrar", idUsuario);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            LoginResponse login = await _autenticacaoService.Login(request);

            return Ok(login);
        }
    }
}
