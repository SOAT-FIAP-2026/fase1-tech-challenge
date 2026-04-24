using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
    {
        public void Configure(EntityTypeBuilder<Orcamento> builder)
        {
            builder.ToTable("orcamento");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");

            builder.Property(o => o.IdOrdemServico).HasColumnName("id_ordem_servico").IsRequired();

            builder.Property(o => o.ValorTotal)
                .HasConversion(v => v.Valor, v => new ValorMonetarioVO(v))
                .HasColumnName("valor_total")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(o => o.OrdemServico)
                .WithOne(os => os.Orcamento)
                .HasForeignKey<Orcamento>(o => o.IdOrdemServico)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(o => o.IdOrdemServico).IsUnique();

            builder.Property(o => o.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(o => o.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(o => o.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(o => o.ApagadoEm == null);
        }
    }
}
