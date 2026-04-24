using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public ServicoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Servico servico)
        {
            const string sql = @"INSERT INTO servico (id, descricao, valor_unitario) VALUES (@Id, @Descricao, @ValorUnitario)";

            await _dbConnection.ExecuteAsync(sql, servico, transaction: _transaction);
        }
    }
}
