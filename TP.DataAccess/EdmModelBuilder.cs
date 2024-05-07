using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using TP.Domain;

namespace TP.DataAccess;

public static class EdmModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new()
        {
            ContainerName = "TourPlannerContainer"
        };
        builder.EnableLowerCamelCase();
        AddTourModel(builder);
        AddTourLogModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddTourModel(ODataConventionModelBuilder builder)
    {
        EntityTypeConfiguration<Tour> tourModelConfig = builder.EntitySet<Tour>("tours").EntityType;
        tourModelConfig.Name = "tour";
        tourModelConfig.HasKey(tour => tour.Id);
        tourModelConfig.Property(tour => tour.Description);
        tourModelConfig.Property(tour => tour.Name);
        tourModelConfig.EnumProperty(tour => tour.TransportType);
        tourModelConfig.Property(tour => tour.TransportTypeInt);
        tourModelConfig.Property(tour => tour.Start);
        tourModelConfig.ComplexProperty(tour => tour.StartCoordinates);
        tourModelConfig.Property(tour => tour.End);
        tourModelConfig.ComplexProperty(tour => tour.EndCoordinates);
        tourModelConfig.Property(tour => tour.DistanceMeters);
        tourModelConfig.Property(tour => tour.EstimatedTime);
        tourModelConfig.EnumProperty(tour => tour.Popularity);
        tourModelConfig.Property(tour => tour.ChildFriendliness);
        tourModelConfig.Property(tour => tour.CreatedOn);
        tourModelConfig.Ignore(tour => tour.UnprocessedLogsCounter);

        tourModelConfig.HasMany(tour => tour.TourLogs);

        tourModelConfig.Expand().Filter().Count().OrderBy().Page().Select();
    }

    private static void AddTourLogModel(ODataConventionModelBuilder builder)
    {
        EntityTypeConfiguration<TourLog> tourLogModelConfig = builder.EntitySet<TourLog>("tour-logs").EntityType;
        tourLogModelConfig.Name = "tour-log";
        tourLogModelConfig.HasKey(tourLog => tourLog.Id);
        tourLogModelConfig.Property(tourLog => tourLog.Comment);
        tourLogModelConfig.EnumProperty(tourLog => tourLog.Difficulty);
        tourLogModelConfig.Property(tourLog => tourLog.DifficultyInt);
        tourLogModelConfig.Property(tourLog => tourLog.TotalDistanceMeters);
        tourLogModelConfig.Property(tourLog => tourLog.TotalTime);
        tourLogModelConfig.Property(tourLog => tourLog.Rating);
        tourLogModelConfig.Property(tourLog => tourLog.CreatedOn);

        tourLogModelConfig.HasRequired(tourLog => tourLog.Tour);

        tourLogModelConfig.Expand().Filter().Count().OrderBy().Page().Select();
    }
}