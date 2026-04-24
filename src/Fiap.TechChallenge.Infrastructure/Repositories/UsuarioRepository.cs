using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorLogin(string login)
        {
            return await _context.Usuarios
                .Include(u => u.Permissao)
                .FirstOrDefaultAsync(u => EF.Property<string>(u, "email") == login.Trim().ToLower());
        }

        public async Task<bool> ExisteEmail(string email)
        {
            string emailNormalizado = email.Trim().ToLower();
            return await _context.Usuarios
                .AnyAsync(u => EF.Property<string>(u, "email") == emailNormalizado);
        }

        public async Task Adicionar(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
