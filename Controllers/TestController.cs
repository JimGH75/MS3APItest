using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;   // ← OPRAVENÝ NAMESPACE!!!

namespace MonS3ApiLight.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly MonS3ReaderService _reader;

        public TestController(MonS3ReaderService reader)
        {
            _reader = reader;
        }

        [HttpGet("connection")]
        public IActionResult TestConnection()
        {
            try
            {
                _reader.Init();

                var addresses = _reader.LoadAddressRaw();
                int count = addresses.Count();

                return Ok(new
                {
                    success = true,
                    message = "Money S3 připojení OK",
                    addressCount = count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
