using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class StatusOrdemServicoRepository : IStatusOrdemServicoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public StatusOrdemServicoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(StatusOrdemServico statusOrdemServico)
        {
            const string sql = @"INSERT INTO status_ordem_servico (id, descricao) VALUES (@Id, @Descricao)";

            await _dbConnection.ExecuteAsync(sql, statusOrdemServico, transaction: _transaction);
        }
    }
}
