using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Endereco : IEquatable<Endereco>
    {
        public string CEP { get; }
        public string Logradouro { get; }
        public string Numero { get; }
        public string? Complemento { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string UF { get; }

        private static readonly string[] UFsValidas =
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        public Endereco(string cep, string logradouro, string numero, string? complemento,
            string bairro, string cidade, string uf)
        {
            var cepLimpo = LimparCEP(cep);

            if (!CEPValido(cepLimpo))
                throw new ArgumentException("CEP inválido", nameof(cep));

            if (string.IsNullOrWhiteSpace(logradouro))
                throw new ArgumentException("Logradouro é obrigatório", nameof(logradouro));

            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("Número é obrigatório", nameof(numero));

            if (string.IsNullOrWhiteSpace(bairro))
                throw new ArgumentException("Bairro é obrigatório", nameof(bairro));

            if (string.IsNullOrWhiteSpace(cidade))
                throw new ArgumentException("Cidade é obrigatória", nameof(cidade));

            var ufUpper = uf?.ToUpper().Trim() ?? string.Empty;
            if (!UFValida(ufUpper))
                throw new ArgumentException("UF inválida", nameof(uf));

            CEP = cepLimpo;
            Logradouro = logradouro.Trim();
            Numero = numero.Trim();
            Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
            Bairro = bairro.Trim();
            Cidade = cidade.Trim();
            UF = ufUpper;
        }

        private static string LimparCEP(string cep)
        {
            return Regex.Replace(cep ?? string.Empty, @"[^\d]", string.Empty);
        }

        private static bool CEPValido(string cep)
        {
            return cep.Length == 8 && cep.All(char.IsDigit);
        }

        private static bool UFValida(string uf)
        {
            return UFsValidas.Contains(uf);
        }

        public string GetCEPFormatado()
        {
            return Regex.Replace(CEP, @"^(\d{5})(\d{3})$", "$1-$2");
        }

        public string GetEnderecoCompleto()
        {
            var endereco = $"{Logradouro}, {Numero}";

            if (!string.IsNullOrWhiteSpace(Complemento))
                endereco += $" - {Complemento}";

            endereco += $", {Bairro}, {Cidade} - {UF}, {GetCEPFormatado()}";

            return endereco;
        }

        public bool Equals(Endereco? other)
        {
            if (other is null) return false;
            return CEP == other.CEP &&
                   Logradouro == other.Logradouro &&
                   Numero == other.Numero &&
                   Complemento == other.Complemento &&
                   Bairro == other.Bairro &&
                   Cidade == other.Cidade &&
                   UF == other.UF;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Endereco);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CEP, Logradouro, Numero, Complemento, Bairro, Cidade, UF);
        }

        public override string ToString()
        {
            return GetEnderecoCompleto();
        }

        public static bool operator ==(Endereco? left, Endereco? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Endereco? left, Endereco? right)
        {
            return !(left == right);
        }
    }
}
