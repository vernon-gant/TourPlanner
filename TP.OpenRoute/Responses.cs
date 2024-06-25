namespace TP.OpenRoute;

public class ReverseGeocodeResponse
{
    public List<GeoFeature> Features { get; set; } = null!;
}

public class GeoFeature
{
    public GeoGeometry Geometry { get; set; } = null!;
}

public class GeoGeometry
{
    public List<decimal> Coordinates { get; set; } = null!;
}

public class DirectionsResponse
{
    public List<Feature> Features { get; set; } = null!;
}

public class Feature
{
    public Properties Properties { get; set; } = null!;

    public Geometry Geometry { get; set; } = null!;
}

public class Properties
{
    public List<Segment> Segments { get; set; } = null!;
}

public class Geometry
{
    public List<List<decimal>> Coordinates { get; set; } = null!;
}

public class Segment
{
    public decimal Distance { get; set; }

    public decimal Duration { get; set; }
}

public class ErrorResponse
{
    public Error Error { get; set; } = null!;
}

public class Error
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;
}