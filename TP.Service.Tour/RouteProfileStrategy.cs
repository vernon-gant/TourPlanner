namespace TP.Service.Tour;

public interface RouteProfileStrategy
{
    string GetProfileName();
}

public class WalkingProfileStrategy : RouteProfileStrategy
{
    public string GetProfileName() => "foot-walking";
}

public class CarProfileStrategy : RouteProfileStrategy
{
    public string GetProfileName() => "driving-car";
}

public class TruckProfileStrategy : RouteProfileStrategy
{
    public string GetProfileName() => "driving-hgv";
}

public class BicycleProfileStrategy : RouteProfileStrategy
{
    public string GetProfileName() => "cycling-regular";
}

public class BikeProfileStrategy : RouteProfileStrategy
{
    public string GetProfileName() => "cycling-electric";
}