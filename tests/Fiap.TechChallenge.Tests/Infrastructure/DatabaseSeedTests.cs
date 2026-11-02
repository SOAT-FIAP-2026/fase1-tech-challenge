using Fiap.TechChallenge.Infrastructure.Data;
using Fiap.TechChallenge.Infrastructure.Data.Seed;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Tests.Infrastructure
{
    public class DatabaseSeedTests
    {
        [Fact]
        public void Constantes_Mecanico_DevemEstarDefinidas()
        {
            DatabaseSeed.PermissaoMecanicoId.Should().NotBeEmpty();
            DatabaseSeed.UsuarioMecanicoId.Should().NotBeEmpty();
            DatabaseSeed.PermissaoMecanicoId.Should().NotBe(DatabaseSeed.PermissaoAdminId);
            DatabaseSeed.PermissaoMecanicoId.Should().NotBe(DatabaseSeed.PermissaoOperadorId);
            DatabaseSeed.UsuarioMecanicoId.Should().NotBe(DatabaseSeed.UsuarioAdminId);
        }

        [Fact]
        public void Apply_DeveSemearPermissaoEUsuarioMecanico_DeFormaIdempotente()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"SeedTestDb_{Guid.NewGuid()}")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                DatabaseSeed.Apply(context);

                var permissaoMecanico = context.Permissoes.FirstOrDefault(p => p.Id == DatabaseSeed.PermissaoMecanicoId);
                permissaoMecanico.Should().NotBeNull();
                permissaoMecanico!.Descricao.Valor.Should().Be("Mecanico");

                var usuarioMecanico = context.Usuarios.Include(u => u.Permissao).FirstOrDefault(u => u.Id == DatabaseSeed.UsuarioMecanicoId);
                usuarioMecanico.Should().NotBeNull();
                usuarioMecanico!.Email.Endereco.Should().Be("mecanico@techchallenge.com");
                usuarioMecanico.IdPermissao.Should().Be(DatabaseSeed.PermissaoMecanicoId);
                usuarioMecanico.Nome.Valor.Should().Be("Mecanico");

                BCrypt.Net.BCrypt.Verify("Mecanico@123", usuarioMecanico.Senha.Hash).Should().BeTrue();
            }

            // Segunda execução: deve ser 100% idempotente sem duplicar ou lançar exceção
            using (var context = new ApplicationDbContext(options))
            {
                var action = () => DatabaseSeed.Apply(context);
                action.Should().NotThrow();

                context.Permissoes.Count(p => p.Id == DatabaseSeed.PermissaoMecanicoId).Should().Be(1);
                context.Usuarios.Count(u => u.Id == DatabaseSeed.UsuarioMecanicoId).Should().Be(1);
            }
        }
    }
}
