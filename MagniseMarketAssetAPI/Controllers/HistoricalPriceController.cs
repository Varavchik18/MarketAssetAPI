using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;

/// <summary>
/// The HistoricalPricesController class handles HTTP requests related to historical price data.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HistoricalPricesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoricalPricesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance to send queries and commands.</param>
    public HistoricalPricesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves historical prices based on a count-back query.
    /// </summary>
    /// <param name="query">The query parameters to filter the historical prices:
    /// <list type="bullet">
    /// <item><description>instrumentId: The ID of the instrument.</description></item>
    /// <item><description>provider: The provider of the data.</description></item>
    /// <item><description>interval: The interval between data points.</description></item>
    /// <item><description>periodicity: The periodicity of the data.</description></item>
    /// <item><description>barsCount: The number of bars to retrieve.</description></item>
    /// </list>
    /// </param>
    /// <returns>An <see cref="IActionResult"/> containing the prices response DTO.</returns>
    /// <response code="200">Returns the prices response DTO.</response>
    /// <response code="400">If there is an error processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     {
    ///         "instrumentId": "12345",
    ///         "provider": "provider1",
    ///         "interval": 1,
    ///         "periodicity": "minute",
    ///         "barsCount": 10
    ///     }
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "assetId": "12345",
    ///         "realTimeData": {
    ///             "last": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 150.0,
    ///                 "volume": 1000
    ///             },
    ///             "ask": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 151.0,
    ///                 "volume": 500
    ///             },
    ///             "bid": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 149.0,
    ///                 "volume": 500
    ///             }
    ///         },
    ///         "historicalData": [
    ///             {
    ///                 "time": "2023-01-01T00:00:00Z",
    ///                 "open": 148.0,
    ///                 "high": 152.0,
    ///                 "low": 147.0,
    ///                 "close": 150.0,
    ///                 "volume": 1000
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpGet("count-back")]
    [ProducesResponseType(typeof(PricesResponseDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPricesCountBack([FromQuery] GetPricesCountBackQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves historical prices based on a date range query.
    /// </summary>
    /// <param name="query">The query parameters to filter the historical prices:
    /// <list type="bullet">
    /// <item><description>instrumentId: The ID of the instrument.</description></item>
    /// <item><description>provider: The provider of the data.</description></item>
    /// <item><description>interval: The interval between data points.</description></item>
    /// <item><description>periodicity: The periodicity of the data.</description></item>
    /// <item><description>startDate: The start date for the data range.</description></item>
    /// <item><description>endDate: The end date for the data range.</description></item>
    /// </list>
    /// </param>
    /// <returns>An <see cref="IActionResult"/> containing the prices response DTO.</returns>
    /// <response code="200">Returns the prices response DTO.</response>
    /// <response code="400">If there is an error processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     {
    ///         "instrumentId": "12345",
    ///         "provider": "provider1",
    ///         "interval": 1,
    ///         "periodicity": "minute",
    ///         "startDate": "2023-01-01T00:00:00Z",
    ///         "endDate": "2023-01-01T23:59:59Z"
    ///     }
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "assetId": "12345",
    ///         "realTimeData": {
    ///             "last": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 150.0,
    ///                 "volume": 1000
    ///             },
    ///             "ask": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 151.0,
    ///                 "volume": 500
    ///             },
    ///             "bid": {
    ///                 "timestamp": "2023-01-01T00:00:00Z",
    ///                 "price": 149.0,
    ///                 "volume": 500
    ///             }
    ///         },
    ///         "historicalData": [
    ///             {
    ///                 "time": "2023-01-01T00:00:00Z",
    ///                 "open": 148.0,
    ///                 "high": 152.0,
    ///                 "low": 147.0,
    ///                 "close": 150.0,
    ///                 "volume": 1000
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(PricesResponseDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPricesDateRange([FromQuery] GetPricesDateRangeQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.Message}");
        }
    }
}
