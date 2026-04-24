using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public EstoqueRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Estoque estoque)
        {
            const string sql = @"INSERT INTO estoque (id, id_peca_insumo, quantidade) VALUES (@Id, @IdPecaInsumo, @Quantidade)";

            await _dbConnection.ExecuteAsync(sql, estoque, transaction: _transaction);
        }
    }
}
