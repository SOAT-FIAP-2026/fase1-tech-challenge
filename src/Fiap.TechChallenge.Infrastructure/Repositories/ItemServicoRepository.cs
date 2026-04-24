using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ItemServicoRepository : IItemServicoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public ItemServicoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(ItemServico itemServico)
        {
            const string sql = @"INSERT INTO item_servico (id, id_ordem_servico, id_servico, data_hora_inicio, data_hora_fim) VALUES (@Id, @IdOrdemServico, @IdServico, @DataHoraInicio, @DataHoraFim)";

            await _dbConnection.ExecuteAsync(sql, itemServico, transaction: _transaction);
        }
    }
}
