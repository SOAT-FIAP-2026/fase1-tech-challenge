using Fiap.TechChallenge.Application.DTOs.Common;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class VeiculoService(IVeiculoRepository veiculoRepository) : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;

        public async Task<Guid> Criar(VeiculoRequest request)
        {
            if (await _veiculoRepository.ExistePlaca(request.Placa))
                throw new VeiculoPlacaJaExisteException(request.Placa);

            Veiculo veiculo = new(request.Placa, request.Marca, request.Modelo, request.Ano);

            await _veiculoRepository.Adicionar(veiculo);

            return veiculo.Id;
        }

        public async Task<VeiculoResponse> ObterPorId(Guid id)
        {
            Veiculo veiculo = await ObterEntidadePorId(id);

            return ToResponse(veiculo);
        }

        public async Task<PagedResult<VeiculoResponse>> ListarPaginado(PagedRequest request)
        {
            var (items, totalCount) = await _veiculoRepository.ListarPaginado(request.Skip, request.PageSize);

            var responses = items.Select(ToResponse).ToList().AsReadOnly();

            return new PagedResult<VeiculoResponse>(responses, request.Page, request.PageSize, totalCount);
        }

        public async Task<VeiculoResponse> Atualizar(Guid id, VeiculoPatchRequest request)
        {
            Veiculo veiculo = await ObterEntidadePorId(id);

            if (request.Placa is not null)
            {
                if (await _veiculoRepository.ExistePlaca(request.Placa, id))
                    throw new VeiculoPlacaJaExisteException(request.Placa);

                veiculo.AlterarPlaca(request.Placa);
            }

            if (request.Marca is not null)
                veiculo.AlterarMarca(request.Marca);

            if (request.Modelo is not null)
                veiculo.AlterarModelo(request.Modelo);

            if (request.Ano.HasValue)
                veiculo.AlterarAno(request.Ano.Value);

            await _veiculoRepository.Atualizar(veiculo);

            return ToResponse(veiculo);
        }

        public async Task Deletar(Guid id)
        {
            Veiculo veiculo = await ObterEntidadePorId(id);

            await _veiculoRepository.Deletar(veiculo);
        }

        private async Task<Veiculo> ObterEntidadePorId(Guid id)
        {
            Veiculo? veiculo = await _veiculoRepository.ObterPorId(id);

            return veiculo ?? throw new VeiculoNaoEncontradoException(id);
        }

        private static VeiculoResponse ToResponse(Veiculo veiculo)
        {
            return new VeiculoResponse(
                veiculo.Id,
                veiculo.Placa.Valor,
                veiculo.Marca.Valor,
                veiculo.Modelo.Valor,
                veiculo.Ano.Valor
            );
        }
    }
}
