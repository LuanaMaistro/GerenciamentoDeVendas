using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class CPF : IEquatable<CPF>
    {
        private readonly string _value;

        public CPF()
        {

        }
        public CPF(string cpf)
        {
            var cleaned = Clean(cpf);

            if (!IsValid(cleaned))
                throw new ArgumentException("CPF inválido", nameof(cpf));

            _value = cleaned;
        }

        private static string Clean(string cpf)
        {
            return Regex.Replace(cpf ?? string.Empty, @"[^\d]", string.Empty);
        }

        private static bool IsValid(string cpf)
        {
            if (cpf.Length != 11) return false;
            if (cpf.All(c => c == cpf[0])) return false;

            // Cálculo do primeiro dígito verificador
            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }

            var firstDigit = (sum * 10) % 11;
            if (firstDigit == 10) firstDigit = 0;

            if (firstDigit != int.Parse(cpf[9].ToString())) return false;

            // Cálculo do segundo dígito verificador
            sum = 0;
            for (var i = 0; i < 10; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (11 - i);
            }

            var secondDigit = (sum * 10) % 11;
            if (secondDigit == 10) secondDigit = 0;

            return secondDigit == int.Parse(cpf[10].ToString());
        }

        public string Value => _value;

        public string GetFormatted()
        {
            return Regex.Replace(_value, @"^(\d{3})(\d{3})(\d{3})(\d{2})$",
                "$1.$2.$3-$4");
        }

        public bool Equals(CPF? other)
        {
            if (other is null) return false;
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CPF);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return GetFormatted();
        }

        public static bool operator ==(CPF? left, CPF? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(CPF? left, CPF? right)
        {
            return !(left == right);
        }

        public static string FormataCPF(string CPF)
        {
            return Regex.Replace(CPF, @"^(\d{3})(\d{3})(\d{3})(\d{2})$",
                 "$1.$2.$3-$4");
        }
    }
}
