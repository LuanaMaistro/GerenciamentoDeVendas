using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.ValueObjects
{
    public enum TipoDocumento
    {
        CPF,
        CNPJ
    }

    public class Documento : IEquatable<Documento>
    {
        // Construtor para EF Core
        private Documento()
        {
            Numero = string.Empty;
        }

        public TipoDocumento Tipo { get; private set; }
        public string Numero { get; private set; }

        private CPF? _cpf;
        private CNPJ? _cnpj;

        public Documento(string numero)
        {
            var numeroLimpo = Regex.Replace(numero ?? string.Empty, @"[^\d]", string.Empty);

            if (numeroLimpo.Length == 11)
            {
                _cpf = new CPF(numero);
                Tipo = TipoDocumento.CPF;
                Numero = _cpf.Value;
            }
            else if (numeroLimpo.Length == 14)
            {
                _cnpj = new CNPJ(numero);
                Tipo = TipoDocumento.CNPJ;
                Numero = _cnpj.Value;
            }
            else
            {
                throw new ArgumentException("Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)", nameof(numero));
            }
        }

        public bool IsCPF => Tipo == TipoDocumento.CPF;
        public bool IsCNPJ => Tipo == TipoDocumento.CNPJ;

        public CPF? GetCPF() => _cpf;
        public CNPJ? GetCNPJ() => _cnpj;

        public string GetFormatado()
        {

            return Tipo switch
            {
                TipoDocumento.CPF => CPF.FormataCPF(Numero),
                TipoDocumento.CNPJ => CNPJ.FormataCNPJ(Numero),
                _ => Numero
            };
        }

        public bool Equals(Documento? other)
        {
            if (other is null) return false;
            return Tipo == other.Tipo && Numero == other.Numero;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Documento);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tipo, Numero);
        }

        public override string ToString()
        {
            return $"{Tipo}: {GetFormatado()}";
        }

        public static bool operator ==(Documento? left, Documento? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Documento? left, Documento? right)
        {
            return !(left == right);
        }

        public static Documento FromCPF(string cpf)
        {
            return new Documento(cpf);
        }

        public static Documento FromCNPJ(string cnpj)
        {
            return new Documento(cnpj);
        }
    }
}
