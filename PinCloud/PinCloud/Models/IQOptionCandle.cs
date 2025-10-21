using System;

namespace APinI.Models;

public class IQOptionCandle
{
    public long Id { get; set; }
    public long From { get; set; }
    public long To { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int Volume { get; set; }
}

