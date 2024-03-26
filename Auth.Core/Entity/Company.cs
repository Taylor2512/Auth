using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Core.Entity
{
    [Table("Company")]
    public class Company : BaseEntity<int>
    {
        [Column("Name")]
        public required string Name { get; set; }
        public required string RegistrationNumber { get; set; }

    }
}
