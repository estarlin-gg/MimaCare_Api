using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Repositories
{
    public class RevokedTokenRepository : IRevokedTokenRepository
    {
        private readonly MimaCareContext _context;

        public RevokedTokenRepository(MimaCareContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RevokedToken token)
            => await _context.RevokedTokens.AddAsync(token);

        public async Task<bool> IsRevokedAsync(string jti)
            => await _context.RevokedTokens.AnyAsync(t => t.Jti == jti);

        public Task DeleteExpiredAsync()
        {
            var expired = _context.RevokedTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow);

            _context.RevokedTokens.RemoveRange(expired);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
