using Fiap.TechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.ToTable("estoque");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.IdPecaInsumo).HasColumnName("id_peca_insumo").IsRequired();
            builder.Property(e => e.Quantidade).HasColumnName("quantidade").IsRequired();

            builder.HasOne(e => e.PecaInsumo)
                .WithOne()
                .HasForeignKey<Estoque>(e => e.IdPecaInsumo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.IdPecaInsumo).IsUnique();

            builder.Property(e => e.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(e => e.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(e => e.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(e => e.ApagadoEm == null);
        }
    }
}
