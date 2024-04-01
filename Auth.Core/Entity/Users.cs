using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Core.Entity
{
    /// <summary>
    /// Represents a user entity.
    /// </summary>
    [Table("Users")]
    public class Users : BaseEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Column("UserName")]
        public required string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Column("Email")]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Column("Password")]
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the salt used for password hashing.
        /// </summary>
        [Column("Salt")]
        public required byte[] Salt { get; set; }
    }
}
