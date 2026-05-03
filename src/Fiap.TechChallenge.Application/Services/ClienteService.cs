using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Guid> Criar(ClienteRequest request)
        {
            if (await _clienteRepository.ExisteCpfCnpj(request.CpfCnpj))
                throw new ClienteCpfCnpjJaExisteException(request.CpfCnpj);

            Cliente cliente = new(request.Nome, request.CpfCnpj, request.Email, request.Celular);

            await _clienteRepository.Adicionar(cliente);

            return cliente.Id;
        }

        public async Task<ClienteResponse> ObterPorId(Guid id)
        {
            Cliente cliente = await ObterEntidadePorId(id);

            return ToResponse(cliente);
        }

        public async Task<ClienteResponse?> ObterPorCpfCnpj(string cpfCnpj)
        {
            Cliente? cliente = await _clienteRepository.ObterPorCpfCnpj(cpfCnpj);

            return cliente is null ? null : ToResponse(cliente);
        }

        public async Task<IReadOnlyCollection<ClienteResponse>> ObterTodos()
        {
            IReadOnlyCollection<Cliente> clientes = await _clienteRepository.ObterTodos();

            return [.. clientes.Select(ToResponse)];
        }

        public async Task<ClienteResponse> Atualizar(Guid id, ClienteRequest request)
        {
            Cliente cliente = await ObterEntidadePorId(id);

            if (await _clienteRepository.ExisteCpfCnpj(request.CpfCnpj, id))
                throw new ClienteCpfCnpjJaExisteException(request.CpfCnpj);

            cliente.Atualizar(request.Nome, request.CpfCnpj, request.Email, request.Celular);

            await _clienteRepository.Atualizar(cliente);

            return ToResponse(cliente);
        }

        public async Task Deletar(Guid id)
        {
            Cliente cliente = await ObterEntidadePorId(id);

            await _clienteRepository.Deletar(cliente);
        }

        private async Task<Cliente> ObterEntidadePorId(Guid id)
        {
            Cliente? cliente = await _clienteRepository.ObterPorId(id);

            if (cliente == null)
                throw new ClienteNaoEncontradoException(id);

            return cliente;
        }

        private static ClienteResponse ToResponse(Cliente cliente)
        {
            return new ClienteResponse(
                cliente.Id,
                cliente.Nome.Valor,
                cliente.CpfCnpj.Valor,
                cliente.Email.Endereco,
                cliente.Celular.Numero
            );
        }
    }
}
