using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class VendaConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.ClienteId)
                .IsRequired();

            builder.Property(v => v.DataVenda)
                .IsRequired();

            builder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(v => v.FormaPagamento)
                .HasConversion<string>();

            builder.Property(v => v.Observacao)
                .HasMaxLength(500);

            // ValorTotal é calculado, não precisa mapear
            builder.Ignore(v => v.ValorTotal);

            // Relacionamento com Cliente
            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Itens
            builder.HasMany(v => v.Itens)
                .WithOne()
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
