using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;
using MonS3ApiLight.Models;

namespace MonS3ApiLight.Controllers;

[ApiController]
[Route("api/lines")]
public class LinesController : ControllerBase
{
    private readonly OrderService _orders;

    public LinesController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public IActionResult GetLines()
    {
        var all = _orders.LoadOrders().ToList();
        var capacity =  (decimal)1500;
        var res = all.Where(o => !string.IsNullOrWhiteSpace(o.Linka))
            .GroupBy(o => o.Linka)
            .Select(g => new LineInfo {
                Linka = g.Key,
                OrdersCount = g.Count(),
                TotalKg = g.Sum(x => x.VahaKg),
                CapacityPct = Math.Round((g.Sum(x => x.VahaKg)/capacity)*100,1),
                Emails = g.Select(x=>x.Email).Where(e=>!string.IsNullOrWhiteSpace(e)).Distinct().ToList()
            }).ToList();
        return Ok(res);
    }
}
