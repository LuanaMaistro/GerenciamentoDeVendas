using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Contato : IEquatable<Contato>
    {
        public string? Telefone { get; private set; }
        public string? Celular { get; private set; }
        public string? Email { get; private set; }

        // Construtor para EF Core
        private Contato()
        {
        }

        public Contato(string? telefone, string? celular, string? email)
        {
            if (string.IsNullOrWhiteSpace(telefone) && string.IsNullOrWhiteSpace(celular) && string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Pelo menos um meio de contato deve ser informado (telefone, celular ou email)");

            if (!string.IsNullOrWhiteSpace(telefone))
            {
                var telefoneLimpo = LimparNumero(telefone);
                if (!ValidarTelefone(telefoneLimpo))
                    throw new ArgumentException("Telefone inválido", nameof(telefone));
                Telefone = telefoneLimpo;
            }

            if (!string.IsNullOrWhiteSpace(celular))
            {
                var celularLimpo = LimparNumero(celular);
                if (!ValidarCelular(celularLimpo))
                    throw new ArgumentException("Celular inválido", nameof(celular));
                Celular = celularLimpo;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailLimpo = email.Trim().ToLower();
                if (!ValidarEmail(emailLimpo))
                    throw new ArgumentException("Email inválido", nameof(email));
                Email = emailLimpo;
            }
        }

        private static string LimparNumero(string valor)
        {
            return Regex.Replace(valor, @"[^\d]", string.Empty);
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

        public string GetTelefoneFormatado()
        {
            if (string.IsNullOrWhiteSpace(Telefone)) return string.Empty;
            return Regex.Replace(Telefone, @"^(\d{2})(\d{4})(\d{4})$", "($1) $2-$3");
        }

        public string GetCelularFormatado()
        {
            if (string.IsNullOrWhiteSpace(Celular)) return string.Empty;
            return Regex.Replace(Celular, @"^(\d{2})(\d{5})(\d{4})$", "($1) $2-$3");
        }

        public bool Equals(Contato? other)
        {
            if (other is null) return false;
            return Telefone == other.Telefone && Celular == other.Celular && Email == other.Email;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Contato);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Telefone, Celular, Email);
        }

        public override string ToString()
        {
            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(Telefone)) partes.Add($"Tel: {GetTelefoneFormatado()}");
            if (!string.IsNullOrWhiteSpace(Celular)) partes.Add($"Cel: {GetCelularFormatado()}");
            if (!string.IsNullOrWhiteSpace(Email)) partes.Add($"Email: {Email}");
            return string.Join(" | ", partes);
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
