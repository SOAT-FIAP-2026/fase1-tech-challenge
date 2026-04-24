using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class PermissaoConfiguration : IEntityTypeConfiguration<Permissao>
    {
        public void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.ToTable("permissao");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.Descricao)
                .HasConversion(v => v.Valor, v => new DescricaoVO(v, 50))
                .HasColumnName("descricao")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(p => p.Descricao).IsUnique();

            builder.Property(p => p.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(p => p.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(p => p.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(p => p.ApagadoEm == null);
        }
    }
}
