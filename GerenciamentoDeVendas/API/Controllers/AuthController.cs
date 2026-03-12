using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica o usuário e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }

        /// <summary>
        /// Cria um novo usuário (apenas Admins).
        /// </summary>
        [HttpPost("usuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioCreateDTO dto)
        {
            var usuario = await _authService.CriarUsuarioAsync(dto);
            return CreatedAtAction(nameof(CriarUsuario), new { id = usuario.Id }, usuario);
        }
    }
}
