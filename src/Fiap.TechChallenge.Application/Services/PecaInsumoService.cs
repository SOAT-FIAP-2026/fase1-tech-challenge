using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class PecaInsumoService(IPecaInsumoRepository pecaInsumoRepository) : IPecaInsumoService
    {
        private readonly IPecaInsumoRepository _pecaInsumoRepository = pecaInsumoRepository;

        public async Task<Guid> Criar(PecaInsumoRequest request)
        {
            PecaInsumo pecaInsumo = new(request.Descricao, request.ValorUnitario);

            await _pecaInsumoRepository.Adicionar(pecaInsumo);

            return pecaInsumo.Id;
        }

        public async Task<PecaInsumoResponse> ObterPorId(Guid id)
        {
            PecaInsumo pecaInsumo = await ObterEntidadePorId(id);

            return ToResponse(pecaInsumo);
        }

        public async Task<PecaInsumoResponse> ObterPorDescricao(string descricao)
        {
            PecaInsumo? pecaInsumo = await _pecaInsumoRepository.ObterPorDescricao(descricao);

            // if (pecaInsumo == null)
            //     throw new DomainException($"Peça/Insumo com descrição contendo '{descricao}' não encontrada.");

            return ToResponse(pecaInsumo);
        }

        public async Task<IReadOnlyCollection<PecaInsumoResponse>> ObterTodos()
        {
            IReadOnlyCollection<PecaInsumo> pecasInsumo = await _pecaInsumoRepository.ListarTodos();

            return [.. pecasInsumo.Select(ToResponse)];
        }

        public async Task<PecaInsumoResponse> Atualizar(Guid id, PecaInsumoRequest request)
        {
            PecaInsumo pecaInsumo = await ObterEntidadePorId(id);

            pecaInsumo.AlterarDescricao(request.Descricao);
            pecaInsumo.AlterarValorUnitario(request.ValorUnitario);

            await _pecaInsumoRepository.Atualizar(pecaInsumo);

            return ToResponse(pecaInsumo);
        }

        public async Task Deletar(Guid id)
        {
            PecaInsumo pecaInsumo = await ObterEntidadePorId(id);

            await _pecaInsumoRepository.Deletar(pecaInsumo);
        }

        private async Task<PecaInsumo> ObterEntidadePorId(Guid id)
        {
            PecaInsumo? pecaInsumo = await _pecaInsumoRepository.ObterPorId(id);

            // if (pecaInsumo == null)
            //     throw new DomainException($"Peça/Insumo com ID {id} não encontrada.");

            return pecaInsumo;
        }

        private static PecaInsumoResponse ToResponse(PecaInsumo pecaInsumo)
        {
            return new PecaInsumoResponse(
                pecaInsumo.Id,
                pecaInsumo.Descricao.Valor,
                pecaInsumo.ValorUnitario.Valor
            );
        }
    }
}
