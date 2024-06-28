[Route("api/[controller]")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAssets([FromQuery] GetAssetsListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}