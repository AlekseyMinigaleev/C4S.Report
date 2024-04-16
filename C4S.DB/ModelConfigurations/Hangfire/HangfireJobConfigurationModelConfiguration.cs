using C4S.DB.Models.Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations.Hangfire
{
    public class HangfireJobConfigurationModelConfiguration : IEntityTypeConfiguration<HangfireJobConfigurationModel>
    {
        public void Configure(EntityTypeBuilder<HangfireJobConfigurationModel> builder)
        {
            builder
                .ToTable("HangfireJobConfiguration")
                .HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithMany(x => x.HangfireJobConfigurationModels)
                .HasForeignKey(x => x.UserId);
        }
    }
}