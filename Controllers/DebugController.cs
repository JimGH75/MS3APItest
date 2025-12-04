using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly MonS3ReaderServiceDebug _service;

    public DebugController(MonS3ReaderServiceDebug service)
    {
        _service = service;
    }

    [HttpGet("init")]
    public IActionResult Init()
    {
        bool ok = _service.Init();
        return Ok(new { success = ok, log = _service.Log });
    }

    [HttpGet("addresses")]
    public IActionResult Addresses()
    {
        return Ok(_service.TestLoadAddresses());
    }

    [HttpGet("orders")]
    public IActionResult Orders()
    {
        return Ok(_service.TestLoadOrders());
    }
}
