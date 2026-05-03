using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("veiculo");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).HasColumnName("id");

            builder.Property(v => v.Placa)
                .HasConversion(p => p.Valor, p => new PlacaVeiculoVO(p))
                .HasColumnName("placa")
                .HasMaxLength(10)
                .IsRequired();

            builder.HasIndex(v => v.Placa).IsUnique();

            builder.Property(v => v.Marca)
                .HasConversion(m => m.Valor, m => new NomeVO(m))
                .HasColumnName("marca")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Modelo)
                .HasConversion(m => m.Valor, m => new NomeVO(m))
                .HasColumnName("modelo")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Ano)
                .HasConversion(a => a.Valor, a => new AnoVeiculoVO(a))
                .HasColumnName("ano")
                .IsRequired();

            builder.Property(v => v.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(v => v.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(v => v.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(v => v.ApagadoEm == null);
        }
    }
}
