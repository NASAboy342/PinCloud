using System;
using APinI.Models;
using APinI.Repository;

namespace APinI.Services;

public class IQOptionService : IIQOptionService
{
    private readonly PinDataRepository _pinDataRepository = new PinDataRepository();
    
    public void AddCandles(List<IQOptionCandle> candles)
    {
        _pinDataRepository.SaveCandles(candles);
    }

    public List<IQOptionCandle> ProcessCandleIds(List<IQOptionCandle> candles)
    {
        //get unique candles
        var uniqueCandlesIds = candles.Select(c => c.Id).Distinct();
        var uniqueCandles = new List<IQOptionCandle>();
        foreach (var id in uniqueCandlesIds)
        {
            var candle = candles.Last(c => c.Id == id);
            uniqueCandles.Add(candle);
        }
        return uniqueCandles;
    }

    public void ValidateCandles(List<IQOptionCandle> candles)
    {
        if (candles == null || !candles.Any())
        {
            throw new ArgumentException("Candle list cannot be null or empty.");
        }
    }
}
