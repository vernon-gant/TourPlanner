using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Domain;

namespace TP.Database;

public class TourLogConfiguration : IEntityTypeConfiguration<TourLog>
{
    public void Configure(EntityTypeBuilder<TourLog> builder)
    {
        builder.HasKey(tourLog => tourLog.Id);
        builder.Property(tourLog => tourLog.UserName).IsRequired().HasMaxLength(100);
        builder.Property(tourLog => tourLog.Comment).HasColumnType("text");
        builder.Property(tourLog => tourLog.Difficulty).HasConversion<string>();
        builder.Property(tourLog => tourLog.TotalDistance).HasPrecision(10, 6);
        builder.Property(tourLog => tourLog.Rating).HasColumnType("smallint");
        builder.Property(tourLog => tourLog.CreatedOn).HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");
        builder.Ignore(tourLog => tourLog.DifficultyInt);

        builder.HasOne(tourLog => tourLog.Tour)
            .WithMany(tour => tour.TourLogs)
            .HasForeignKey(tourLog => tourLog.TourId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}