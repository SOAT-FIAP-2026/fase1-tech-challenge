using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v{version}/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly ILogger<AutenticacaoController> _logger;
        private readonly IAutenticacaoService _autenticacaoService;

        public AutenticacaoController(
            ILogger<AutenticacaoController> logger,
            IAutenticacaoService autenticacaoService
        )
        {
            _logger = logger;
            _autenticacaoService = autenticacaoService;
        }

        [HttpPost("Cadastrar")]
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
