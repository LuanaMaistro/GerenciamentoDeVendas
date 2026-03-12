using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<TokenDTO> LoginAsync(LoginDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var usuario = await _unitOfWork.Usuarios.ObterPorEmailAsync(email)
                ?? throw new InvalidOperationException("Email ou senha inválidos");

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha);
            if (resultado == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Email ou senha inválidos");

            var (token, expiracao) = GerarToken(usuario);
            return new TokenDTO(token, expiracao, usuario.Nome, usuario.Email, usuario.Role);
        }

        public async Task<UsuarioDTO> CriarUsuarioAsync(UsuarioCreateDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _unitOfWork.Usuarios.EmailJaCadastradoAsync(email))
                throw new InvalidOperationException("Email já cadastrado");

            // Cria instância temporária para passar ao hasher (conforme contrato IPasswordHasher<T>)
            var usuarioTemp = new Usuario(dto.Nome, email, "placeholder", dto.Role);
            var senhaHash = _passwordHasher.HashPassword(usuarioTemp, dto.Senha);

            var usuario = new Usuario(dto.Nome, email, senhaHash, dto.Role);

            await _unitOfWork.Usuarios.AdicionarAsync(usuario);
            await _unitOfWork.CommitAsync();

            return MapToDTO(usuario);
        }

        private (string token, DateTime expiracao) GerarToken(Usuario usuario)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddMinutes(int.Parse(jwtConfig["ExpiracaoMinutos"] ?? "480"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
        }

        private static UsuarioDTO MapToDTO(Usuario u) =>
            new(u.Id, u.Nome, u.Email, u.Role, u.DataCadastro);
    }
}
