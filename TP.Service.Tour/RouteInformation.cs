using TP.Utils;

namespace TP.Service.Tour;

public record RouteInformation(decimal DistanceMeters, decimal EstimatedTimeSeconds, Coordinates StartCoordinates, Coordinates EndCoordinates);