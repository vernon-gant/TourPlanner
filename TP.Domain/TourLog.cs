using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace TP.Domain;

public enum Difficulty
{
    VeryHard = 100,
    Hard = 200,
    Normal = 300,
    Easy = 400,
    VeryEasy = 500
}

public class TourLog
{
    public Guid Id { get; set; }

    public string Comment { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    public decimal TotalDistanceMeters { get; set; }

    public TimeSpan TotalTime { get; set; }

    public short Rating { get; set; }

    public DateTime CreatedOn { get; set; }

    [Column("FTX")]
    public NpgsqlTsVector SearchVector { get; } = null!;

    public Tour? Tour { get; set; }

    public Guid TourId { get; set; }

    public const int MaxUserNameLength = 100;

    public const short MinRating = 0;

    public const short MaxRating = 10;
}