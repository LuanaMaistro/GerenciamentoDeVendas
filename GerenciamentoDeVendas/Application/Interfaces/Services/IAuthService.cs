using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<TokenDTO> LoginAsync(LoginDTO dto);
        Task<UsuarioDTO> CriarUsuarioAsync(UsuarioCreateDTO dto);
    }
}
