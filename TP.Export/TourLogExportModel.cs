﻿using TP.Domain;

namespace TP.Export;

public class TourLogExportModel
{
    public int TourNumber { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    public decimal TotalDistanceMeters { get; set; }

    public TimeSpan TotalTime { get; set; }

    public short Rating { get; set; }
}