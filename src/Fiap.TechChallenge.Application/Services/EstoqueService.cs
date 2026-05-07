using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class EstoqueService(IEstoqueRepository estoqueRepository, IPecaInsumoRepository pecaInsumoRepository) : IEstoqueService
    {
        private readonly IEstoqueRepository _estoqueRepository = estoqueRepository;
        private readonly IPecaInsumoRepository _pecaInsumoRepository = pecaInsumoRepository;

        public async Task<Guid> Criar(EstoqueRequest request)
        {
            await GarantirPecaInsumoExiste(request.IdPecaInsumo);

            var estoque = new Estoque(request.IdPecaInsumo, request.Quantidade);

            await _estoqueRepository.Adicionar(estoque);

            return estoque.Id;
        }

        public async Task<int?> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo)
        {
            return await _estoqueRepository.VerificarQuantidadePorIdPecaInsumo(idPecaInsumo);
        }

        public async Task<int?> VerificarQuantidadePorDescricaoPeca(string descricao)
        {
            return await _estoqueRepository.VerificarQuantidadePorDescricaoPeca(descricao);
        }

        public async Task AdicionarQuantidade(Guid idPecaInsumo, int quantidade)
        {
            await GarantirPecaInsumoExiste(idPecaInsumo);
            Estoque estoque = await ObterEstoquePorIdPecaInsumo(idPecaInsumo);

            estoque.AdicionarQuantidade(quantidade);

            await _estoqueRepository.Atualizar(estoque);
        }

        public async Task RemoverQuantidade(Guid idPecaInsumo, int quantidade)
        {
            await GarantirPecaInsumoExiste(idPecaInsumo);
            Estoque estoque = await ObterEstoquePorIdPecaInsumo(idPecaInsumo);

            estoque.RemoverQuantidade(quantidade);

            await _estoqueRepository.Atualizar(estoque);
        }

        public async Task Deletar(Guid idEstoque)
        {
            await _estoqueRepository.Deletar(idEstoque);
        }

        private async Task GarantirPecaInsumoExiste(Guid idPecaInsumo)
        {
            if (await _pecaInsumoRepository.ObterPorId(idPecaInsumo) == null)
                throw new PecaInsumoNaoEncontradaException(idPecaInsumo);
        }

        private async Task<Estoque> ObterEstoquePorIdPecaInsumo(Guid idPecaInsumo)
        {
            Estoque? estoque = await _estoqueRepository.ObterPorIdPecaInsumo(idPecaInsumo);

            return estoque ?? throw new EstoqueNaoEncontradoException(idPecaInsumo);
        }
    }
}
