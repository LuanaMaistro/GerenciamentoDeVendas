using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
    {
        // foi aqui que você falou que eu ia te matar por ter um construtor vazio? ahuahua
        // esse construtor aqui tá certo. como você tá herdando uma classe, ele precisa passar os parametros para classe mãe :)
        public ClienteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> ObterPorDocumentoAsync(Documento documento)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Documento.Numero == documento.Numero);
        }

        public async Task<IEnumerable<Cliente>> ObterAtivosAsync()
        {
            return await _dbSet
                .Where(c => c.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cliente>> ObterInativosAsync()
        {
            return await _dbSet
                .Where(c => !c.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cliente>> BuscarPorNomeAsync(string nome)
        {
            return await _dbSet
                .Where(c => c.Nome.Contains(nome))
                .ToListAsync();
        }

        public async Task<bool> DocumentoJaCadastradoAsync(Documento documento)
        {
            return await _dbSet
                .AnyAsync(c => c.Documento.Numero == documento.Numero);
        }
    }
}
