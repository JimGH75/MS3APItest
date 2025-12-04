using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;

namespace MonS3ApiLight.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;

    public OrdersController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery]string? linka)
    {
        var list = _orders.LoadOrders();
        if (!string.IsNullOrWhiteSpace(linka))
            list = list.Where(x => x.Linka.Equals(linka, StringComparison.OrdinalIgnoreCase));

        return Ok(list);
    }

    [HttpGet("{cradku:long}")]
    public IActionResult GetDetail(long cradku)
    {
        var o = _orders.LoadOrders().FirstOrDefault(x => x.CRadku == cradku);
        if (o == null) return NotFound();
        return Ok(o);
    }
}
