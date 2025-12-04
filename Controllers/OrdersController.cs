using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;
using System.Collections.Generic;

namespace MonS3ApiLight.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly MonS3ReaderService _reader;

        public OrdersController(MonS3ReaderService reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Získá všechny otevøené objednávky.
        /// </summary>
        [HttpGet]
        public IEnumerable<dynamic> GetOrders() => _reader.LoadOrdersRaw();

        /// <summary>
        /// Získá položky všech objednávek.
        /// </summary>
        [HttpGet("items")]
        public IEnumerable<dynamic> GetOrderItems() => _reader.LoadItemsRaw();
    }
}
