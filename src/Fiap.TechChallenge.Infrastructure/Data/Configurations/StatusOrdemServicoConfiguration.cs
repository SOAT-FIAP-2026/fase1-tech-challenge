using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class StatusOrdemServicoConfiguration : IEntityTypeConfiguration<StatusOrdemServico>
    {
        public void Configure(EntityTypeBuilder<StatusOrdemServico> builder)
        {
            builder.ToTable("status_ordem_servico");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.Codigo)
                .HasConversion(v => v.Valor, v => new CodigoVO(v))
                .HasColumnName("codigo")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(s => s.Descricao)
                .HasConversion(v => v.Valor, v => new DescricaoVO(v, 100))
                .HasColumnName("descricao")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(s => s.Descricao).IsUnique();
        }
    }
}
