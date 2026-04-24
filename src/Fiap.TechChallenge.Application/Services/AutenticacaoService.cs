using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Domain.Interfaces.Service;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Application.Services
{
    public class AutenticacaoService: IAutenticacaoService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly ICrypto _cryptoService;

        public AutenticacaoService(
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            ICrypto cryptoService
        ) 
        { 
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _cryptoService = cryptoService;
        }

        public async Task<Guid> Cadastrar(CadastrarRequest request)
        {
            if (await _usuarioRepository.ExisteEmail(request.Email))
                throw new Exception("E-mail já cadastrado.");

            SenhaUsuarioVO hashSenha = SenhaUsuarioVO.CriarNova(request.Senha, _cryptoService);

            var usuario = new Usuario(request.Nome, request.Email, hashSenha, request.IdPermissao);

            await _usuarioRepository.Adicionar(usuario);

            return usuario.Id;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorLogin(request.Login);

            if (usuario == null || 
                !_cryptoService.VerificarSenha(request.Senha, usuario.Senha.Hash))
                throw new Exception("Usuário ou senha inválidos.");

            var token = _tokenService.GerarToken(usuario);
            return new LoginResponse(token, usuario.Nome.Valor);
        }
    }
}
