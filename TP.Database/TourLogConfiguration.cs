using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Domain;

namespace TP.Database;

public class TourLogConfiguration : IEntityTypeConfiguration<TourLog>
{
    public void Configure(EntityTypeBuilder<TourLog> builder)
    {
        builder.HasKey(tourLog => tourLog.Id);
        builder.Property(tourLog => tourLog.Comment).HasColumnType("text");
        builder.Property(tourLog => tourLog.Difficulty).HasConversion<int>();
        builder.Property(tourLog => tourLog.TotalDistanceMeters).HasPrecision(12, 3);
        builder.Property(tourLog => tourLog.Rating).HasColumnType("smallint");
        builder.Property(tourLog => tourLog.CreatedOn).HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");

        builder.HasOne(tourLog => tourLog.Tour)
            .WithMany(tour => tour.TourLogs)
            .HasForeignKey(tourLog => tourLog.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasGeneratedTsVectorColumn(
                    tourLog => tourLog.SearchVector, "english",
                    tourLog => new { tourLog.Comment})
               .HasIndex(m => m.SearchVector).HasMethod("GIN");
    }
}