using Auth.Core.Interface;

using System.ComponentModel.DataAnnotations;

namespace Auth.Core.Entity
{
    public class BaseEntity<T> : BaseEntity
    {
        [Key]
        public required T Id { get; set; }
    }

    public class BaseEntity : IEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
