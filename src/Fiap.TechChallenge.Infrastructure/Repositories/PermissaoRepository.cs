using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class PermissaoRepository : IPermissaoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public PermissaoRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task Adicionar(Permissao permissao)
        {
            const string sql = @"INSERT INTO permissao (id, descricao) VALUES (@Id, @Descricao)";

            await _dbConnection.ExecuteAsync(sql, permissao, transaction: _transaction);
        }
    }
}
