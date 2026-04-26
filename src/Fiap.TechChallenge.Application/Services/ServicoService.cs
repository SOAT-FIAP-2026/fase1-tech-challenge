using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class ServicoService( IServicoRepository servicoRepository) : IServicoService
    {
        private readonly IServicoRepository _servicoRepository = servicoRepository;

        public async Task<Guid> Criar(ServicoRequest request)
        {
            if (await _servicoRepository.ExisteNome(request.Nome))
                throw new ServicoNomeJaExisteException(request.Nome);

            Servico servico = new(request.Nome, request.Descricao, request.ValorUnitario);

            await _servicoRepository.Adicionar(servico);

            return servico.Id;
        }

        public async Task<ServicoResponse> ObterPorId(Guid id)
        {
            Servico servico = await ObterEntidadePorId(id);

            return ToResponse(servico);
        }

        public async Task<IReadOnlyCollection<ServicoResponse>> ObterTodos()
        {
            IReadOnlyCollection<Servico> servicos = await _servicoRepository.ObterTodos();

            return [.. servicos.Select(ToResponse)];
        }

        public async Task<ServicoResponse> Atualizar(Guid id, ServicoRequest request)
        {
            Servico servico = await ObterEntidadePorId(id);

            if (await _servicoRepository.ExisteNome(request.Nome, id))
                throw new ServicoNomeJaExisteException(request.Nome);


            servico.Atualizar(request.Nome, request.Descricao, request.ValorUnitario);

            await _servicoRepository.Atualizar(servico);

            return ToResponse(servico);
        }

        public async Task Deletar(Guid id)
        {
            Servico servico = await ObterEntidadePorId(id);

            await _servicoRepository.Deletar(servico);
        }

        private async Task<Servico> ObterEntidadePorId(Guid id)
        {
            Servico? servico = await _servicoRepository.ObterPorId(id);

            if (servico == null)
                throw new ServicoNaoEncontradoException(id);

            return servico;
        }

        private static ServicoResponse ToResponse(Servico servico)
        {
            return new ServicoResponse(
                servico.Id,
                servico.Nome.Valor,
                servico.Descricao.Valor,
                servico.ValorUnitario.Valor
            );
        }
    }
}
