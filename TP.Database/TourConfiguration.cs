using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Domain;

namespace TP.Database;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.HasKey(tour => tour.Id);
        builder.Property(tour => tour.Description).IsRequired().HasColumnType("text");
        builder.Property(tour => tour.Name).IsRequired().HasMaxLength(Tour.MaxNameLength);
        builder.Property(tour => tour.Start).IsRequired().HasMaxLength(Tour.MaxPointDescriptionLength);
        builder.Property(tour => tour.End).IsRequired().HasMaxLength(Tour.MaxPointDescriptionLength);
        builder.Property(tour => tour.DistanceMeters).IsRequired().HasPrecision(10, 3);
        builder.Property(tour => tour.RouteGeometry).IsRequired().HasColumnType("text");
        builder.Property(tour => tour.TransportType).IsRequired().HasConversion<int>();
        builder.Property(tour => tour.CreatedOn).HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");
        builder.Property(tour => tour.UnprocessedLogsCounter);
        builder.ComplexProperty(tour => tour.StartCoordinates);
        builder.ComplexProperty(tour => tour.EndCoordinates);
        builder.Property(tour => tour.Popularity).HasConversion<int>();

        builder.HasMany(tour => tour.TourLogs)
            .WithOne(tourLog => tourLog.Tour)
            .HasForeignKey(tourLog => tourLog.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasGeneratedTsVectorColumn(
                tour => tour.SearchVector, "english",
                tour => new { tour.Name, tour.Description, tour.Start, tour.End, tour.ChildFriendliness, tour.Popularity })
           .HasIndex(m => m.SearchVector).HasMethod("GIN");
    }
}