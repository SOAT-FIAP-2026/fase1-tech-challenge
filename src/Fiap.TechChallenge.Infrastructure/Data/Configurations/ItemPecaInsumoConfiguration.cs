using Fiap.TechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class ItemPecaInsumoConfiguration : IEntityTypeConfiguration<ItemPecaInsumo>
    {
        public void Configure(EntityTypeBuilder<ItemPecaInsumo> builder)
        {
            builder.ToTable("item_peca_insumo");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasColumnName("id");

            builder.Property(i => i.IdOrdemServico).HasColumnName("id_ordem_servico").IsRequired();
            builder.Property(i => i.IdPecaInsumo).HasColumnName("id_peca_insumo").IsRequired();

            builder.HasOne(i => i.OrdemServico)
                .WithMany(o => o.ItensPecaInsumo)
                .HasForeignKey(i => i.IdOrdemServico)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.PecaInsumo)
                .WithMany()
                .HasForeignKey(i => i.IdPecaInsumo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => new { i.IdOrdemServico, i.IdPecaInsumo }).IsUnique();
            builder.HasIndex(i => i.IdOrdemServico);
            builder.HasIndex(i => i.IdPecaInsumo);
        }
    }
}
