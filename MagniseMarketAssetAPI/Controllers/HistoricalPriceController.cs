[Route("api/[controller]")]
[ApiController]
public class HistoricalPricesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistoricalPricesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("count-back")]
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

    [HttpGet("date-range")]
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