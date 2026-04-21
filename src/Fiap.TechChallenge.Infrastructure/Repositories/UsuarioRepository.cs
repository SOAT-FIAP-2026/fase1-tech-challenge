using Dapper;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using System.Data;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;

        public UsuarioRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.Connection;
            _transaction = unitOfWork.Transaction;
        }

        public async Task<Usuario> ObterPorLogin(string login)
        {
            const string sql = "SELECT * FROM usuario WHERE Login = @login";

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.AddDynamicParams(new { Login = login });

            Usuario result = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(
                sql, dynamicParameters, transaction: _transaction);

            return result;
        }

        public async Task Adicionar(Usuario usuario)
        {
            const string sql = 
                @"INSERT INTO usuario (Id, Nome, Email, Login, Senha) 
                  VALUES (@Id, @Nome, @Email, @Login, @Senha)";

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.AddDynamicParams(new { Id = usuario.Id });
            dynamicParameters.AddDynamicParams(new { Nome = usuario.Nome });
            dynamicParameters.AddDynamicParams(new { Email = usuario.Email });
            dynamicParameters.AddDynamicParams(new { Login = usuario.Login.Login });
            dynamicParameters.AddDynamicParams(new { Senha = usuario.Login.Senha });

            await _dbConnection.ExecuteAsync(sql, dynamicParameters, transaction: _transaction);
        }

        public async Task<bool> ExisteEmail(string email)
        {
            const string sql =
                @"SELECT count(1) > 0 FROM usuario WHERE Email = @Email";

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.AddDynamicParams(new { Email = email });

            bool result = await _dbConnection.QueryFirstOrDefaultAsync<bool>(
                sql, dynamicParameters, transaction: _transaction);

            return result;
        }
    }
}