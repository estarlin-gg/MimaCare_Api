using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IRevokedTokenRepository
    {
        Task AddAsync(RevokedToken token);
        Task<bool> IsRevokedAsync(string jti);
        Task DeleteExpiredAsync();
        Task SaveChangesAsync();
    }
}
