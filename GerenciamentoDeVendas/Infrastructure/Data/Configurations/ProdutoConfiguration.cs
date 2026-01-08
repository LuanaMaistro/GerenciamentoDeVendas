using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.Codigo)
                .IsUnique();

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descricao)
                .HasMaxLength(500);

            builder.Property(p => p.PrecoUnitario)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Categoria)
                .HasMaxLength(100);

            builder.Property(p => p.Ativo)
                .IsRequired();

            builder.Property(p => p.DataCadastro)
                .IsRequired();
        }
    }
}
