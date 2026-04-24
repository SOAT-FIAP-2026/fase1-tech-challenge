using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Infrastructure.Data.Seed
{
    public static class DatabaseSeed
    {
        public static readonly Guid PermissaoAdminId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        public static readonly Guid PermissaoOperadorId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

        public static readonly Guid StatusAbertaId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
        public static readonly Guid StatusEmAndamentoId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");
        public static readonly Guid StatusConcluidaId = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234");
        public static readonly Guid StatusCanceladaId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345");

        public static readonly Guid UsuarioAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static void Apply(ApplicationDbContext context)
        {
            SeedPermissoes(context);
            SeedStatusOrdemServico(context);
            SeedUsuarioAdmin(context);
        }

        private static void SeedPermissoes(ApplicationDbContext context)
        {
            if (context.Permissoes.Any()) return;

            var admin = new Permissao("Administrador");
            context.Entry(admin).Property(p => p.Id).CurrentValue = PermissaoAdminId;

            var operador = new Permissao("Operador");
            context.Entry(operador).Property(p => p.Id).CurrentValue = PermissaoOperadorId;

            context.Permissoes.AddRange(admin, operador);
            context.SaveChanges();
        }

        private static void SeedStatusOrdemServico(ApplicationDbContext context)
        {
            if (context.StatusOrdensServico.Any()) return;

            var aberta = new StatusOrdemServico("Aberta");
            context.Entry(aberta).Property(s => s.Id).CurrentValue = StatusAbertaId;

            var emAndamento = new StatusOrdemServico("Em Andamento");
            context.Entry(emAndamento).Property(s => s.Id).CurrentValue = StatusEmAndamentoId;

            var concluida = new StatusOrdemServico("Concluída");
            context.Entry(concluida).Property(s => s.Id).CurrentValue = StatusConcluidaId;

            var cancelada = new StatusOrdemServico("Cancelada");
            context.Entry(cancelada).Property(s => s.Id).CurrentValue = StatusCanceladaId;

            context.StatusOrdensServico.AddRange(aberta, emAndamento, concluida, cancelada);
            context.SaveChanges();
        }

        private static void SeedUsuarioAdmin(ApplicationDbContext context)
        {
            if (context.Usuarios.Any()) return;

            // Pre-computed BCrypt hash for "Admin@123"
            var senhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var senha = new SenhaUsuarioVO(senhaHash);
            var admin = new Usuario("Administrador", "admin@techchallenge.com", senha, PermissaoAdminId);
            context.Entry(admin).Property(u => u.Id).CurrentValue = UsuarioAdminId;

            context.Usuarios.Add(admin);
            context.SaveChanges();
        }
    }
}
