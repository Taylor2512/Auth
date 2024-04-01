using Auth.Core.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configuration
{
    /// <summary>
    /// Represents the configuration for the <see cref="Users"/> entity.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {

        }
    }
}
