using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.ToTable("servico");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");

            builder.Property(s => s.Nome)
                .HasConversion(v => v.Valor, v => new NomeVO(v))
                .HasColumnName("nome")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(s => s.Descricao)
                .HasConversion(v => v.Valor, v => new DescricaoVO(v, 255))
                .HasColumnName("descricao")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(s => s.Nome).IsUnique();

            builder.Property(s => s.ValorUnitario)
                .HasConversion(v => v.Valor, v => new ValorMonetarioVO(v))
                .HasColumnName("valor_unitario")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(s => s.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(s => s.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(s => s.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(s => s.ApagadoEm == null);
        }
    }
}
