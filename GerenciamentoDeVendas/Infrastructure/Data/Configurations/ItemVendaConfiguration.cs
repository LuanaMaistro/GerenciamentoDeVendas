using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
    {
        public void Configure(EntityTypeBuilder<ItemVenda> builder)
        {
            builder.ToTable("ItensVenda");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.VendaId)
                .IsRequired();

            builder.Property(i => i.ProdutoId)
                .IsRequired();

            builder.Property(i => i.ProdutoNome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Quantidade)
                .IsRequired();

            builder.Property(i => i.PrecoUnitario)
                .IsRequired()
                .HasPrecision(18, 2);

            // Subtotal é calculado, não precisa mapear
            builder.Ignore(i => i.Subtotal);

            // Relacionamento com Produto
            builder.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
