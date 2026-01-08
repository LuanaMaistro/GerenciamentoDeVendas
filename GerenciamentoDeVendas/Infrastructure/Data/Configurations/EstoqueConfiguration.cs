using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.ToTable("Estoques");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ProdutoId)
                .IsRequired();

            builder.HasIndex(e => e.ProdutoId)
                .IsUnique();

            builder.Property(e => e.Quantidade)
                .IsRequired();

            builder.Property(e => e.QuantidadeMinima)
                .IsRequired();

            builder.Property(e => e.Localizacao)
                .HasMaxLength(100);

            builder.Property(e => e.DataUltimaAtualizacao)
                .IsRequired();

            // Relacionamento com Produto
            builder.HasOne<Produto>()
                .WithOne()
                .HasForeignKey<Estoque>(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
