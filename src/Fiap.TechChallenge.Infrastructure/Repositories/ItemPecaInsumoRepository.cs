using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ItemPecaInsumoRepository : IItemPecaInsumoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public ItemPecaInsumoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(ItemPecaInsumo itemPecaInsumo)
        {
            const string sql = @"INSERT INTO item_peca_insumo (id, id_ordem_servico, id_peca_insumo) VALUES (@Id, @IdOrdemServico, @IdPecaInsumo)";

            await _dbConnection.ExecuteAsync(sql, itemPecaInsumo, transaction: _transaction);
        }
    }
}
