using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class PecaInsumoRepository : IPecaInsumoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public PecaInsumoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(PecaInsumo pecaInsumo)
        {
            const string sql = @"INSERT INTO peca_insumo (id, descricao, valor_unitario) VALUES (@Id, @Descricao, @ValorUnitario)";

            await _dbConnection.ExecuteAsync(sql, pecaInsumo, transaction: _transaction);
        }
    }
}
