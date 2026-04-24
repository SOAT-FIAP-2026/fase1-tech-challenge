using Fiap.TechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class ItemServicoConfiguration : IEntityTypeConfiguration<ItemServico>
    {
        public void Configure(EntityTypeBuilder<ItemServico> builder)
        {
            builder.ToTable("item_servico");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasColumnName("id");

            builder.Property(i => i.IdOrdemServico).HasColumnName("id_ordem_servico").IsRequired();
            builder.Property(i => i.IdServico).HasColumnName("id_servico").IsRequired();
            builder.Property(i => i.DataHoraInicio).HasColumnName("data_hora_inicio");
            builder.Property(i => i.DataHoraFim).HasColumnName("data_hora_fim");

            builder.HasOne(i => i.OrdemServico)
                .WithMany(o => o.ItensServico)
                .HasForeignKey(i => i.IdOrdemServico)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Servico)
                .WithMany()
                .HasForeignKey(i => i.IdServico)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => new { i.IdOrdemServico, i.IdServico }).IsUnique();
            builder.HasIndex(i => i.IdOrdemServico);
            builder.HasIndex(i => i.IdServico);
        }
    }
}
