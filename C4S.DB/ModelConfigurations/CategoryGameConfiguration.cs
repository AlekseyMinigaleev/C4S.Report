using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4S.DB.ModelConfigurations
{
    public class CategoryGameConfiguration : IEntityTypeConfiguration<CategoryGameModel>
    {
        public void Configure(EntityTypeBuilder<CategoryGameModel> builder)
        {
            builder.ToTable("CategoryGame");

            builder.HasKey(x => new { x.GameId, x.CategoryId });

            builder.HasOne(x => x.Game)
                .WithMany()
                .HasForeignKey(x => x.GameId);
        }
    }
}