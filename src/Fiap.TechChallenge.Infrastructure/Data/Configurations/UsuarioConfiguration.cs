using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("id");

            builder.Property(u => u.Nome)
                .HasConversion(v => v.Valor, v => new NomeVO(v))
                .HasColumnName("nome")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasConversion(v => v.Endereco, v => new EmailVO(v))
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Senha)
                .HasConversion(v => v.Hash, v => new SenhaUsuarioVO(v))
                .HasColumnName("senha_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.IdPermissao).HasColumnName("id_permissao").IsRequired();

            builder.HasOne(u => u.Permissao)
                .WithMany()
                .HasForeignKey(u => u.IdPermissao)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(u => u.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(u => u.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(u => u.ApagadoEm == null);
        }
    }
}
