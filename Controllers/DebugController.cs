using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;


namespace MonS3ApiLight.Controllers
{
    [Route("api/debug")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly MonS3ReaderServiceDebug _debugService;

        public DebugController(MonS3ReaderServiceDebug debugService)
        {
            _debugService = debugService;
        }

        /// <summary>
        /// Endpoint pro načtení seznamu adres
        /// </summary>
        [HttpGet("addresses")]
        public ActionResult<List<Dictionary<string, object>>> GetAddresses()
        {
            var result = _debugService.TestLoadAddresses();
            if (result.Any())
                return Ok(result);
            else
                return NotFound("Adresy nenalezeny.");
        }

        /// <summary>
        /// Endpoint pro načtení seznamu objednávek
        /// </summary>
        [HttpGet("orders")]
        public ActionResult<List<Dictionary<string, object>>> GetOrders()
        {
            var result = _debugService.TestLoadOrders();
            if (result.Any())
                return Ok(result);
            else
                return NotFound("Objednávky nenalezeny.");
        }

        /// <summary>
        /// Endpoint pro načtení seznamu roků agendy
        /// </summary>
        [HttpGet("years")]
        public ActionResult<List<Dictionary<string, object>>> GetYearList()
        {
            var result = _debugService.TestGetYearList();
            if (result.Any())
                return Ok(result);
            else
                return NotFound("Roky nenalezeny.");
        }
    }
}
