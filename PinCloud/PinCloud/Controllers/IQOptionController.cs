using System;
using APinI.Models;
using APinI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IQOptionController : ControllerBase
{
    private readonly IIQOptionService _iqOptionService;
    public IQOptionController(IIQOptionService iqOptionService)
    {
        _iqOptionService = iqOptionService;
    }
    [HttpPost("add-candles")]
    public BaseResponse AddCandles([FromBody] AddCandleRequest req)
    {
        try
        {
            _iqOptionService.ValidateCandles(req.Candles);
            req.Candles = _iqOptionService.ProcessCandleIds(req.Candles);
            _iqOptionService.AddCandles(req.Candles);
            return new BaseResponse
            {
                ErrorCode = 0,
                ErrorMessage = "Candle data received successfully."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                ErrorCode = -1,
                ErrorMessage = ex.Message
            };
        }
        
    }
}
