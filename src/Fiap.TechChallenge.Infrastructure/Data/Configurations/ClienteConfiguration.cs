using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("cliente");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");

            builder.Property(c => c.Nome)
                .HasConversion(v => v.Valor, v => new NomeVO(v))
                .HasColumnName("nome")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.CpfCnpj)
                .HasConversion(v => v.Valor, v => new CpfCnpjVO(v))
                .HasColumnName("cpf_cnpj")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(c => c.CpfCnpj).IsUnique();

            builder.Property(c => c.Email)
                .HasConversion(v => v.Endereco, v => new EmailVO(v))
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.Celular)
                .HasConversion(v => v.Numero, v => new CelularVO(v))
                .HasColumnName("celular")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(c => c.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(c => c.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(c => c.ApagadoEm == null);
        }
    }
}
