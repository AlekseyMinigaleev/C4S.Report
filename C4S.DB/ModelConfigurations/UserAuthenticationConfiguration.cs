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
               .HasOne(x => x.User)
               .WithOne(u => u.AuthenticationModel)
               .HasForeignKey<UserAuthenticationModel>(x => x.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Удалите эту строку, если вы не хотите каскадного удаления

            builder
                .HasIndex(x => x.UserId)
                .IsUnique();
        }
    }
}