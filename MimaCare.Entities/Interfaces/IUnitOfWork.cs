using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}
