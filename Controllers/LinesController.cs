using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;
using System.Collections.Generic;

namespace MonS3ApiLight.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinesController : ControllerBase
    {
        private readonly MonS3ReaderService _reader;

        public LinesController(MonS3ReaderService reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Získá seznam všech adres.
        /// </summary>
        [HttpGet]
        public IEnumerable<dynamic> GetAddresses() => _reader.LoadAddressRaw();
    }
}
