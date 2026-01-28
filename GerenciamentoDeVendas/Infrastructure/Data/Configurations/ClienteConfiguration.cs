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

            // Configuração do Contato Principal como Owned Type (inline na tabela Clientes)
            builder.OwnsOne(c => c.ContatoPrincipal, ct =>
            {
                ct.Property(c => c.Telefone)
                    .HasColumnName("ContatoTelefone")
                    .HasMaxLength(11);

                ct.Property(c => c.Celular)
                    .HasColumnName("ContatoCelular")
                    .HasMaxLength(11);

                ct.Property(c => c.Email)
                    .HasColumnName("ContatoEmail")
                    .HasMaxLength(200);
            });

            // Configuração dos Contatos Secundários como Owned Type (tabela separada)
            builder.OwnsMany(c => c.ContatosSecundarios, ct =>
            {
                ct.ToTable("ClienteContatos");

                ct.WithOwner().HasForeignKey("ClienteId");

                ct.Property<int>("Id");
                ct.HasKey("Id");

                ct.Property(c => c.Telefone)
                    .HasMaxLength(11);

                ct.Property(c => c.Celular)
                    .HasMaxLength(11);

                ct.Property(c => c.Email)
                    .HasMaxLength(200);
            });


            builder.Navigation(c => c.ContatosSecundarios)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Configuração do Endereço Principal como Owned Type (inline na tabela Clientes)
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

            // Configuração dos Endereços Secundários como Owned Type (tabela separada)
            builder.OwnsMany(c => c.EnderecosSecundarios, end =>
            {
                end.ToTable("ClienteEnderecosSecundarios");

                end.WithOwner().HasForeignKey("ClienteId");

                end.Property<int>("Id");
                end.HasKey("Id");

                end.UsePropertyAccessMode(PropertyAccessMode.Field);

                end.Property(e => e.CEP)
                    .IsRequired()
                    .HasMaxLength(8);

                end.Property(e => e.Logradouro)
                    .IsRequired()
                    .HasMaxLength(200);

                end.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(20);

                end.Property(e => e.Complemento)
                    .HasMaxLength(100);

                end.Property(e => e.Bairro)
                    .IsRequired()
                    .HasMaxLength(100);

                end.Property(e => e.Cidade)
                    .IsRequired()
                    .HasMaxLength(100);

                end.Property(e => e.UF)
                    .IsRequired()
                    .HasMaxLength(2);
            });
        }
     }
}
