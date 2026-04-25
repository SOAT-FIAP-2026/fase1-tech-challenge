using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class UsuarioRepository(ApplicationDbContext context) : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Usuario?> ObterPorLogin(EmailVO email)
        {
        
            return await _context.Usuarios
                .Include(u => u.Permissao)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteEmail(EmailVO email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email);
        }

        public async Task Adicionar(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
