using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettingsModel>
    {
        public void Configure(EntityTypeBuilder<UserSettingsModel> builder)
        {
            builder
                .ToTable("UserSettings")
                .HasKey(x => x.Id);

            builder
                .HasOne(x => x.User)
                .WithOne(x => x.Settings)
                .HasForeignKey<UserSettingsModel>(x => x.UserId);
        }
    }
}
