using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Infrastructure.Data.Seed
{
    public static class DatabaseSeed
    {
        public static readonly Guid PermissaoAdminId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        public static readonly Guid PermissaoOperadorId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
        public static readonly Guid StatusRecebidaId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
        public static readonly Guid StatusEmDiagnosticoId = Guid.Parse("d3e4f5a6-b7c8-9012-defa-234567890123");
        public static readonly Guid StatusEmAguardandoAprovacaoId = Guid.Parse("e3f4a5b6-c7d8-9012-efab-345678901234");
        public static readonly Guid StatusFinalizadaId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");
        public static readonly Guid StatusEmExecucaoId = Guid.Parse("f4a5b6c7-d8e9-0123-efab-345678901234");
        public static readonly Guid StatusEntregueId = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234");
        public static readonly Guid StatusCanceladaId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345");
        public static readonly Guid UsuarioAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        
        // GUIDs das Peças e Insumos
        public static readonly Guid PecaOleoMotoId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid PecaPastilhaFreioId = Guid.Parse("a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c6");
        public static readonly Guid PecaAguaRadiadorId = Guid.Parse("0a1b2c3d-4e5f-6789-abcd-ef0123456789");
        public static readonly Guid PecaFiltroArId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
        public static readonly Guid PecaBateria60AhId = Guid.Parse("9c858901-8a57-4791-81fe-4c455b099bc9");
        public static readonly Guid PecaPneu205Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");
        public static readonly Guid PecaAmortecedorId = Guid.Parse("987e6543-e21b-12d3-a456-426614174001");
        public static readonly Guid PecaVelasIgnicaoId = Guid.Parse("abcdefab-cdef-1234-5678-abcdefabcdef");
        public static readonly Guid PecaCorreibaDentadaId = Guid.Parse("fedcba98-7654-3210-4321-fedcba987654");
        
        // GUIDs dos Estoques
        public static readonly Guid EstoqueOleoMotoId = Guid.Parse("111e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoquePastilhaFreioId = Guid.Parse("222e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoqueAguaRadiadorId = Guid.Parse("8e9d0c1b-2a3f-4e5d-6c7b-8a90b1c2d3e4");
        public static readonly Guid EstoqueFiltroArId = Guid.Parse("333e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoqueBateria60AhId = Guid.Parse("444e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoquePneu205Id = Guid.Parse("555e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoqueAmortecedorId = Guid.Parse("666e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoqueVelasIgnicaoId = Guid.Parse("777e8400-e29b-41d4-a716-446655440000");
        public static readonly Guid EstoqueCorreibaDentadaId = Guid.Parse("888e8400-e29b-41d4-a716-446655440000");

        public static void Apply(ApplicationDbContext context)
        {
            SeedPermissoes(context);
            SeedStatusOrdemServico(context);
            SeedUsuarioAdmin(context);
            SeedClientes(context);
            SeedServicos(context);
            SeedPecasInsumo(context);
            SeedEstoque(context);
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

            var recebida = new StatusOrdemServico("Recebida");
            context.Entry(recebida).Property(s => s.Id).CurrentValue = StatusRecebidaId;

            var emDiagnostico = new StatusOrdemServico("Em Diagnóstico");
            context.Entry(emDiagnostico).Property(s => s.Id).CurrentValue = StatusEmDiagnosticoId;

            var aguardandoAprovacao = new StatusOrdemServico("Aguardando aprovação");
            context.Entry(aguardandoAprovacao).Property(s => s.Id).CurrentValue = StatusEmAguardandoAprovacaoId;

            var emExecucao = new StatusOrdemServico("Em Execução");
            context.Entry(emExecucao).Property(s => s.Id).CurrentValue = StatusEmExecucaoId;

            var finalizada = new StatusOrdemServico("Finalizada");
            context.Entry(finalizada).Property(s => s.Id).CurrentValue = StatusFinalizadaId;

            var entregue = new StatusOrdemServico("Entregue");
            context.Entry(entregue).Property(s => s.Id).CurrentValue = StatusEntregueId;

            var cancelada = new StatusOrdemServico("Cancelada");
            context.Entry(cancelada).Property(s => s.Id).CurrentValue = StatusCanceladaId;
            

            context.StatusOrdensServico.AddRange(recebida, emDiagnostico, aguardandoAprovacao, emExecucao, finalizada, entregue, cancelada);
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

        private static void SeedClientes(ApplicationDbContext context)
        {
            if (context.Clientes.Any()) return;

            var cliente1 = new Cliente("João Silva", "282.027.830-20", "joao.silva@example.com", "11987654321");
            var cliente2 = new Cliente("Maria Oliveira", "312.408.510-81", "maria.oliveira@example.com", "11987654321");
            context.Clientes.Add(cliente1);
            context.Clientes.Add(cliente2);
            context.SaveChanges();
        }

         private static void SeedServicos(ApplicationDbContext context)
        {
            if (context.Servicos.Any()) return;

            var servico1 = new Servico("Troca de Pneus", "Descrição do serviço 1",100);
            var servico2 = new Servico("Troca de Óleo", "Descrição do serviço 2",200);
            var servico3 = new Servico("Alinhamento", "Descrição do serviço 3",150);
            var servico4 = new Servico("Balanceamento", "Descrição do serviço 4",120);
            
            context.Servicos.Add(servico1);
            context.Servicos.Add(servico2);
            context.Servicos.Add(servico3);
            context.Servicos.Add(servico4);
            context.SaveChanges();
        }

        private static void SeedPecasInsumo(ApplicationDbContext context)
        {
            if (context.PecasInsumo.Any()) return;

            var pecas = new List<(Guid Id, string Descricao, decimal Valor)>
            {
                (PecaOleoMotoId, "Óleo de Motor", 29.99m),
                (PecaPastilhaFreioId, "Pastilha de Freio", 49.99m),
                (PecaAguaRadiadorId, "Água Radiador", 24.99m),
                (PecaFiltroArId, "Filtro de Ar", 19.99m),
                (PecaBateria60AhId, "Bateria 60Ah", 389.99m),
                (PecaPneu205Id, "Pneu 205/55 R16", 299.99m),
                (PecaAmortecedorId, "Amortecedor Dianteiro", 199.99m),
                (PecaVelasIgnicaoId, "Velas de Ignição", 79.99m),
                (PecaCorreibaDentadaId, "Correia Dentada", 149.99m)
            };

            foreach (var (id, descricao, valor) in pecas)
            {
                var peca = new PecaInsumo(descricao, valor);
                context.Entry(peca).Property(p => p.Id).CurrentValue = id;
                context.PecasInsumo.Add(peca);
            }

            context.SaveChanges();
        }

        private static void SeedEstoque(ApplicationDbContext context)
        {
            if (context.Estoques.Any()) return;

            var estoques = new List<(Guid Id, Guid IdPecaInsumo, int Quantidade)>
            {
                (EstoqueOleoMotoId, PecaOleoMotoId, 100),
                (EstoquePastilhaFreioId, PecaPastilhaFreioId, 50),
                (EstoqueAguaRadiadorId, PecaAguaRadiadorId, 90),
                (EstoqueFiltroArId, PecaFiltroArId, 200),
                (EstoqueBateria60AhId, PecaBateria60AhId, 30),
                (EstoquePneu205Id, PecaPneu205Id, 80),
                (EstoqueAmortecedorId, PecaAmortecedorId, 40),
                (EstoqueVelasIgnicaoId, PecaVelasIgnicaoId, 150),
                (EstoqueCorreibaDentadaId, PecaCorreibaDentadaId, 60)
            };

            foreach (var (id, idPecaInsumo, quantidade) in estoques)
            {
                var estoque = new Estoque(idPecaInsumo, quantidade);
                context.Entry(estoque).Property(e => e.Id).CurrentValue = id;
                context.Estoques.Add(estoque);
            }

            context.SaveChanges();
        }
    }
}
