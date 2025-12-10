using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public enum TipoContato
    {
        Telefone,
        Celular,
        Email
    }

    public class Contato : IEquatable<Contato>
    {
        public TipoContato Tipo { get; }
        public string Valor { get; }
        public bool Principal { get; }

        public Contato(TipoContato tipo, string valor, bool principal = false)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Valor do contato é obrigatório", nameof(valor));

            var valorLimpo = LimparValor(valor, tipo);

            if (!ValidarContato(valorLimpo, tipo))
                throw new ArgumentException($"{tipo} inválido", nameof(valor));

            Tipo = tipo;
            Valor = valorLimpo;
            Principal = principal;
        }

        private static string LimparValor(string valor, TipoContato tipo)
        {
            if (tipo == TipoContato.Email)
                return valor.Trim().ToLower();

            return Regex.Replace(valor, @"[^\d]", string.Empty);
        }

        private static bool ValidarContato(string valor, TipoContato tipo)
        {
            return tipo switch
            {
                TipoContato.Telefone => ValidarTelefone(valor),
                TipoContato.Celular => ValidarCelular(valor),
                TipoContato.Email => ValidarEmail(valor),
                _ => false
            };
        }

        private static bool ValidarTelefone(string telefone)
        {
            // Telefone fixo: 10 dígitos (DDD + 8 dígitos)
            return telefone.Length == 10 && telefone.All(char.IsDigit);
        }

        private static bool ValidarCelular(string celular)
        {
            // Celular: 11 dígitos (DDD + 9 + 8 dígitos)
            return celular.Length == 11 && celular.All(char.IsDigit) && celular[2] == '9';
        }

        private static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public string GetFormatado()
        {
            return Tipo switch
            {
                TipoContato.Telefone => FormatarTelefone(),
                TipoContato.Celular => FormatarCelular(),
                TipoContato.Email => Valor,
                _ => Valor
            };
        }

        private string FormatarTelefone()
        {
            // (XX) XXXX-XXXX
            return Regex.Replace(Valor, @"^(\d{2})(\d{4})(\d{4})$", "($1) $2-$3");
        }

        private string FormatarCelular()
        {
            // (XX) 9XXXX-XXXX
            return Regex.Replace(Valor, @"^(\d{2})(\d{5})(\d{4})$", "($1) $2-$3");
        }

        public bool Equals(Contato? other)
        {
            if (other is null) return false;
            return Tipo == other.Tipo && Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Contato);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tipo, Valor);
        }

        public override string ToString()
        {
            var formatado = GetFormatado();
            var tipoPrincipal = Principal ? " (Principal)" : "";
            return $"{Tipo}: {formatado}{tipoPrincipal}";
        }

        public static bool operator ==(Contato? left, Contato? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Contato? left, Contato? right)
        {
            return !(left == right);
        }
    }
}
