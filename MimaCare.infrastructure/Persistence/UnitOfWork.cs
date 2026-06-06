using Microsoft.EntityFrameworkCore.Storage;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MimaCareContext _context;

        private IDbContextTransaction?
            _transaction;

        public UnitOfWork(
            MimaCareContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction =
                await _context.Database
                    .BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context
                    .SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction
                    .CommitAsync();

                await _transaction.DisposeAsync();
            }
            
        }

        public async Task RollbackAsync()
        {
            if (_transaction is not null)
            {
                await _transaction
                    .RollbackAsync();
            }
        }
    }
}
