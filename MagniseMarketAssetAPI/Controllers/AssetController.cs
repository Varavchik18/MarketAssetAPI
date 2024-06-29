using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// The AssetsController class handles HTTP requests related to asset data.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance to send queries and commands.</param>
    public AssetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a list of assets based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters to filter the assets list: 
    ///    Request Parameters:
    ///     provider: (optional) The provider of the assets.
    ///     kind: (optional) The kind of assets.
    ///     symbol: (optional) The symbol of the assets.
    ///     page: (optional) The page number for pagination (default is 1).
    ///     size: (optional) The number of assets per page(default is 10). </param>
    /// 
    /// <returns>An <see cref="IActionResult"/> containing the assets list response DTO.</returns>
    /// <response code="200">Returns the assets list response DTO.</response>
    /// <response code="400">If there is an error processing the request.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     {
    ///    "provider": "provider",
    ///    "kind": "equity",
    ///    "symbol": "AAPL",
    ///    "page": 1,
    ///    "size": 10
    ///}
    ///
    ///
    /// Sample response:
    ///
    ///     {
    ///         "paging": {
    ///             "page": 1,
    ///             "pages": 10,
    ///             "items": 100
    ///         },
    ///         "data": [
    ///             {
    ///                 "id": "123",
    ///                 "symbol": "AAPL",
    ///                 "kind": "equity",
    ///                 "description": "Apple Inc.",
    ///                 "tickSize": 0.01,
    ///                 "currency": "USD",
    ///                 "baseCurrency": "USD",
    ///                 "mappings": {
    ///                     "NASDAQ": {
    ///                         "symbol": "AAPL",
    ///                         "exchange": "NASDAQ",
    ///                         "defaultOrderSize": 100
    ///                     }
    ///                 }
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    /// 

    [HttpGet("list")]
    [ProducesResponseType(typeof(AssetsResponseDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAssets([FromQuery] GetAssetsListQuery query)
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
}