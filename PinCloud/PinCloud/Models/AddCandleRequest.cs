using System;

namespace APinI.Models;

public class AddCandleRequest
{
    public List<IQOptionCandle> Candles { get; set; } = new List<IQOptionCandle>();
}
