using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Core.Entity
{
    /// <summary>
    /// Represents a company entity.
    /// </summary>
    [Table("Company")]
    public class Company : BaseEntity<int>
    {
        [Column("Name")]
        public required string Name { get; set; }
        public required string RegistrationNumber { get; set; }

    }
}
