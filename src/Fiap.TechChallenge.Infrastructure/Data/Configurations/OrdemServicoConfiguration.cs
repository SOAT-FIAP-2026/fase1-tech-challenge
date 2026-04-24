using Fiap.TechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.TechChallenge.Infrastructure.Data.Configurations
{
    public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServico> builder)
        {
            builder.ToTable("ordem_servico");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");

            builder.Property(o => o.IdCliente).HasColumnName("id_cliente").IsRequired();
            builder.Property(o => o.IdVeiculo).HasColumnName("id_veiculo").IsRequired();
            builder.Property(o => o.IdStatus).HasColumnName("id_status").IsRequired();
            builder.Property(o => o.Observacao).HasColumnName("observacao").HasColumnType("text");
            builder.Property(o => o.DataAbertura).HasColumnName("data_abertura").IsRequired();
            builder.Property(o => o.DataConclusao).HasColumnName("data_conclusao");

            builder.HasOne(o => o.Cliente)
                .WithMany()
                .HasForeignKey(o => o.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Veiculo)
                .WithMany()
                .HasForeignKey(o => o.IdVeiculo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.IdStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(o => o.IdCliente);
            builder.HasIndex(o => o.IdVeiculo);
            builder.HasIndex(o => o.IdStatus);

            builder.Property(o => o.CriadoEm).HasColumnName("criado_em").IsRequired();
            builder.Property(o => o.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            builder.Property(o => o.ApagadoEm).HasColumnName("apagado_em");
            builder.HasQueryFilter(o => o.ApagadoEm == null);
        }
    }
}
