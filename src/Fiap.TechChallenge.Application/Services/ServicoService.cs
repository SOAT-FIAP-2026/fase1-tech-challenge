using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;

namespace Fiap.TechChallenge.Application.Services
{
    public class ServicoService: IServicoService
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicoService(
            IServicoRepository servicoRepository
        ) 
        { 
            _servicoRepository = servicoRepository;
        }

        public Task<Guid> Criar(ServicoRequest request)
        {

        Servico servico = new(request.Nome, request.Descricao, request.Preco);

        _servicoRepository.Adicionar(servico);

            throw new NotImplementedException();
        }
    }
}
