using Auth.Core.Interface;

using System.ComponentModel.DataAnnotations;

namespace Auth.Core.Entity
{
    /// <summary>
    /// Represents a base entity with a generic identifier.
    /// </summary>
    /// <typeparam name="T">The type of the identifier.</typeparam>
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
