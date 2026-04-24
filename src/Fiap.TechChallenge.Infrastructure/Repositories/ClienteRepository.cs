using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public ClienteRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Cliente cliente)
        {
            const string sql = @"INSERT INTO cliente (id, nome, cpf_cnpj, email, celular) VALUES (@Id, @Nome, @CpfCnpj, @Email, @Celular)";

            await _dbConnection.ExecuteAsync(sql, cliente, transaction: _transaction);
        }
    }
}
