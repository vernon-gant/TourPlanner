﻿using Microsoft.EntityFrameworkCore;
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
        builder.Property(tour => tour.DistanceMeters).IsRequired().HasPrecision(10,3);
        builder.Property(tour => tour.TransportType).HasConversion<string>();
        builder.Property(tour => tour.CreatedOn).HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");
        builder.Ignore(tour => tour.TransportTypeInt);
        builder.Property(tour => tour.UnprocessedLogsCounter);
        builder.Property(tour => tour.Popularity).HasConversion<string>();

        builder.HasMany(tour => tour.TourLogs)
            .WithOne(tourLog => tourLog.Tour)
            .HasForeignKey(tourLog => tourLog.TourId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}