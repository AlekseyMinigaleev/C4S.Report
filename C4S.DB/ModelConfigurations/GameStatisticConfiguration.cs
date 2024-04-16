using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class GameStatisticConfiguration : IEntityTypeConfiguration<GameStatisticModel>
    {
        public void Configure(EntityTypeBuilder<GameStatisticModel> builder)
        {
            builder.ToTable("GameStatistic")
                .HasKey(x => x.Id);

            builder.OwnsOne(e => e.CashIncome, progress =>
            {
                progress.Property(p => p.ActualValue)
                    .HasColumnName("CashIncomeActual");

                progress.Property(p => p.ProgressValue)
                    .HasColumnName("CashIncomeProgress");
            });

            builder.OwnsOne(e => e.Rating, progress =>
            {
                progress.Property(p => p.ActualValue)
                    .HasColumnName("RatingActual");

                progress.Property(p => p.ProgressValue)
                    .HasColumnName("RatingProgress");
            });
        }
    }
}