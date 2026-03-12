using System;

namespace Domain.Entities
{
    public class Usuario
    {
        // Construtor para EF Core
        private Usuario()
        {
            Nome = string.Empty;
            Email = string.Empty;
            SenhaHash = string.Empty;
            Role = string.Empty;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public string Role { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public Usuario(string nome, string email, string senhaHash, string role = "Operador")
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório", nameof(email));

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha é obrigatória", nameof(senhaHash));

            Id = Guid.NewGuid();
            Nome = nome.Trim();
            Email = email.Trim().ToLowerInvariant();
            SenhaHash = senhaHash;
            Role = role;
            DataCadastro = DateTime.Now;
        }

        public void AtualizarSenha(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
                throw new ArgumentException("Senha é obrigatória", nameof(novaSenhaHash));

            SenhaHash = novaSenhaHash;
        }
    }
}
