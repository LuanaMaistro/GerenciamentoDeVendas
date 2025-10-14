using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Cliente
    {
        public string Nome { get; set; }
        public Documento Documento { get; set; }
        public bool Inativo { get; set; }
        public Contato[] Contatos { get; set; }

        public Endereco EnderecoPrincipal { get; set; }
        public Endereco[] EnderecoSecundarios { get; set; }

        public void Ativa()
        {
            Inativo = false;
        }
        public void Inativa()
        {
            Inativo = true;
        }

        public void SetEnderecoPrincipal(Endereco endereco)
        {
            EnderecoPrincipal = endereco;
        }

    }
}
