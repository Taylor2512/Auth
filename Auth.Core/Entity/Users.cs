using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Core.Entity
{
    [Table("Users")]
    public class Users : BaseEntity<Guid>
    {
        [Column("UserName")]
        public required string UserName { get; set; }
        [Column("Email")]
        public required string Email { get; set; }
        [Column("Password")]
        public required string Password { get; set; }
        [Column("Salt")]
        public required byte[] Salt { get; set; }
    }
}
