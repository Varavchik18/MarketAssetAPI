[Route("api/[controller]")]
[ApiController]
public class HistoricalPricesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistoricalPricesController(IMediator mediator)
    {
        _mediator = mediator;
    }
}