using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class RevokedToken
    {
        public Guid Id { get; set; }

        public string Jti { get; set; }
            = default!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
