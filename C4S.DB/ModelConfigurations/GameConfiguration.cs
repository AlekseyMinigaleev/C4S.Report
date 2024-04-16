using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class GameConfiguration : IEntityTypeConfiguration<GameModel>
    {
        public void Configure(EntityTypeBuilder<GameModel> builder)
        {
            builder.ToTable("Game")
                .HasKey(x => x.Id);

            builder.HasMany(x => x.GameStatistics)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);

            builder.HasMany(x => x.CategoryGameModels)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);

            builder
                .Ignore(x => x.Categories);
        }
    }
}