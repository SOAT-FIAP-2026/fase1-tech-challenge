using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public VeiculoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Veiculo veiculo)
        {
            const string sql = @"INSERT INTO veiculo (id, placa, marca, modelo, ano) VALUES (@Id, @Placa, @Marca, @Modelo, @Ano)";

            await _dbConnection.ExecuteAsync(sql, veiculo, transaction: _transaction);
        }
    }
}
