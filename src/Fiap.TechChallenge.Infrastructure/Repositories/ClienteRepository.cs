using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ClienteRepository(ApplicationDbContext context) : IClienteRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Cliente?> ObterPorId(Guid id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> ObterPorCpfCnpj(string cpfCnpj)
        {
            CpfCnpjVO cpfCnpjVO = new(cpfCnpj);

            return await _context.Clientes.FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpjVO);
        }

        public async Task<IReadOnlyCollection<Cliente>> ObterTodos()
        {
            return await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<bool> ExisteCpfCnpj(string cpfCnpj, Guid? ignorarId = null)
        {
            CpfCnpjVO cpfCnpjVO = new(cpfCnpj);

            return await _context.Clientes.AnyAsync(c =>
                c.CpfCnpj == cpfCnpjVO &&
                (!ignorarId.HasValue || c.Id != ignorarId.Value)
            );
        }

        public async Task Adicionar(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Cliente cliente)
        {
            cliente.MarcarComoApagado();
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
