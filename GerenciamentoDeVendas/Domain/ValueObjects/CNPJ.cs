using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class CNPJ : IEquatable<CNPJ>
    {
        private readonly string _value;

        public CNPJ(string cnpj)
        {
            var cleaned = Clean(cnpj);

            if (!IsValid(cleaned))
                throw new ArgumentException("CNPJ inválido", nameof(cnpj));

            _value = cleaned;
        }

        private static string Clean(string cnpj)
        {
            return Regex.Replace(cnpj ?? string.Empty, @"[^\d]", string.Empty);
        }

        private static bool IsValid(string cnpj)
        {
            if (cnpj.Length != 14) return false;
            if (cnpj.All(c => c == cnpj[0])) return false;

            var sum = 0;
            var pos = 5;

            for (var i = 0; i < 12; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * pos;
                pos = pos == 2 ? 9 : pos - 1;
            }

            var digit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (digit != int.Parse(cnpj[12].ToString())) return false;

            sum = 0;
            pos = 6;

            for (var i = 0; i < 13; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * pos;
                pos = pos == 2 ? 9 : pos - 1;
            }

            digit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            return digit == int.Parse(cnpj[13].ToString());
        }

        public string Value => _value;

        public string GetFormatted()
        {
            return Regex.Replace(_value, @"^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$",
                "$1.$2.$3/$4-$5");
        }

        public bool Equals(CNPJ other)
        {
            if (other is null) return false;
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CNPJ);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return GetFormatted();
        }

        public static bool operator ==(CNPJ left, CNPJ right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(CNPJ left, CNPJ right)
        {
            return !(left == right);
        }

    }
}
