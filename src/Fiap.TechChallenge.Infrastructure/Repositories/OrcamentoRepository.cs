using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public OrcamentoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Orcamento orcamento)
        {
            const string sql = @"INSERT INTO orcamento (id, id_ordem_servico, valor_total) VALUES (@Id, @IdOrdemServico, @ValorTotal)";

            await _dbConnection.ExecuteAsync(sql, orcamento, transaction: _transaction);
        }
    }
}
