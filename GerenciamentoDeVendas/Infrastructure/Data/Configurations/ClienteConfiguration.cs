using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Ativo)
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .IsRequired();


            builder.OwnsOne(c => c.CPF, doc =>
            {
                doc.Property(d => d.Value)
                    .HasColumnName("CPF")
                    .HasMaxLength(11);
            });

            builder.OwnsOne(c => c.CNPJ, doc =>
            {
                doc.Property(d => d.Value)
                    .HasColumnName("CNPJ")
                    .HasMaxLength(14);
            });
            // Configuração do Value Object Documento como Owned Type
            builder.OwnsOne(c => c.Documento, doc =>
            {
                doc.Property(d => d.Numero)
                    .HasColumnName("DocumentoNumero")
                    .IsRequired()
                    .HasMaxLength(14);

                doc.Property(d => d.Tipo)
                    .HasColumnName("DocumentoTipo")
                    .IsRequired()
                    .HasConversion<string>();

                doc.Ignore(d => d.IsCPF);
                doc.Ignore(d => d.IsCNPJ);
            });

            // Configuração do Value Object Endereco Principal como Owned Type
            builder.OwnsOne(c => c.EnderecoPrincipal, end =>
            {
                end.Property(e => e.CEP)
                    .HasColumnName("EnderecoCEP")
                    .HasMaxLength(8);

                end.Property(e => e.Logradouro)
                    .HasColumnName("EnderecoLogradouro")
                    .HasMaxLength(200);

                end.Property(e => e.Numero)
                    .HasColumnName("EnderecoNumero")
                    .HasMaxLength(20);

                end.Property(e => e.Complemento)
                    .HasColumnName("EnderecoComplemento")
                    .HasMaxLength(100);

                end.Property(e => e.Bairro)
                    .HasColumnName("EnderecoBairro")
                    .HasMaxLength(100);

                end.Property(e => e.Cidade)
                    .HasColumnName("EnderecoCidade")
                    .HasMaxLength(100);

                end.Property(e => e.UF)
                    .HasColumnName("EnderecoUF")
                    .HasMaxLength(2);
            });

            // Ignora coleções de Value Objects (serão tabelas separadas se necessário)
            builder.Ignore(c => c.EnderecosSecundarios);
            builder.Ignore(c => c.Contatos);
        }
    }
}
