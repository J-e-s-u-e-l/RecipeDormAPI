using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset TimeCreated { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TimeUpdated { get; set; } = DateTimeOffset.UtcNow;
    }

    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; }
    }
}
