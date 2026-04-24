using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public OrdemServicoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(OrdemServico ordemServico)
        {
            const string sql = @"INSERT INTO ordem_servico (id, id_cliente, id_veiculo, id_status, observacao) VALUES (@Id, @IdCliente, @IdVeiculo, @IdStatus, @Observacao)";

            await _dbConnection.ExecuteAsync(sql, ordemServico, transaction: _transaction);
        }
    }
}
