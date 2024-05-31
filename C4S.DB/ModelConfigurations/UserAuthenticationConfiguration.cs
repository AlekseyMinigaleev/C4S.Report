using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    internal class UserAuthenticationConfiguration : IEntityTypeConfiguration<UserAuthenticationModel>
    {
        public void Configure(EntityTypeBuilder<UserAuthenticationModel> builder)
        {
            builder
                .ToTable("UserAuthentication")
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.UserId)
                .IsUnique();
        }
    }
}