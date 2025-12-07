using Microsoft.AspNetCore.Mvc;
using MonS3ApiLight.Services;
using System.Collections.Generic;

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

        // =====================================================================
        //  ZÁKLADNÍ TESTOVACÍ ENDPOINTY
        // =====================================================================

        /// <summary>
        /// Načtení adres z agendy
        /// </summary>
        [HttpGet("addresses")]
        public ActionResult GetAddresses()
        {
            try
            {
                var result = _debugService.LoadAddresses();

                return Ok(new
                {
                    success = true,
                    count = result.Count,
                    sample = result.Count > 0 ? result[0] : null,
                    total = result.Count
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Načtení objednávek z agendy (aktuální/poslední rok)
        /// </summary>
        [HttpGet("orders")]
        public ActionResult GetOrders()
        {
            try
            {
                var result = _debugService.LoadOrders();

                return Ok(new
                {
                    success = true,
                    count = result.Count,
                    sample = result.Count > 0 ? result[0] : null,
                    total = result.Count
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Načtení roků z agendy
        /// </summary>
        [HttpGet("years")]
        public ActionResult GetYears()
        {
            try
            {
                var result = _debugService.LoadYears();

                return Ok(new
                {
                    success = true,
                    count = result.Count,
                    years = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // =====================================================================
        //  PRÁCE S KONKRÉTNÍMI ROKY
        // =====================================================================

        /// <summary>
        /// Načtení objednávek pro konkrétní rok
        /// </summary>
        [HttpGet("orders/{year}")]
        public ActionResult GetOrdersForYear(int year)
        {
            try
            {
                var result = _debugService.LoadOrdersForYear(year);

                return Ok(new
                {
                    success = true,
                    year = year,
                    count = result.Count,
                    sample = result.Count > 0 ? result[0] : null,
                    total = result.Count
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Prohledat všechny roky v agendě
        /// </summary>
        [HttpGet("explore-years")]
        public ActionResult ExploreYears()
        {
            var result = _debugService.ExploreAllYears();

            if ((bool)result["success"])
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Načtení položek objednávek pro konkrétní rok
        /// </summary>
        [HttpGet("order-items/{year}")]
        public ActionResult GetOrderItemsForYear(int year)
        {
            try
            {
                var result = _debugService.LoadOrderItemsForYear(year);

                return Ok(new
                {
                    success = true,
                    year = year,
                    count = result.Count,
                    sample = result.Count > 0 ? result[0] : null,
                    total = result.Count
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // =====================================================================
        //  DIAGNOSTIKA A INFORMACE
        // =====================================================================

        /// <summary>
        /// Test připojení a inicializace
        /// </summary>
        [HttpGet("connection-test")]
        public ActionResult ConnectionTest()
        {
            try
            {
                // 1. Nejprve roky - to testuje základní připojení
                var years = _debugService.LoadYears();

                // 2. Pak adresy
                var addresses = _debugService.LoadAddresses();

                // 3. Pak objednávky (aktuální rok)
                var orders = _debugService.LoadOrders();

                return Ok(new
                {
                    success = true,
                    message = "Money S3 připojení OK",
                    statistics = new
                    {
                        yearsCount = years.Count,
                        addressesCount = addresses.Count,
                        ordersCount = orders.Count
                    },
                    sampleData = new
                    {
                        firstYear = years.Count > 0 ? years[0] : null,
                        firstAddress = addresses.Count > 0 ? addresses[0] : null,
                        firstOrder = orders.Count > 0 ? orders[0] : null
                    }
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Test různých tabulek v agendě
        /// </summary>
        [HttpGet("test-tables")]
        public ActionResult TestTables()
        {
            try
            {
                var result = _debugService.TestAllTables();

                return Ok(new
                {
                    success = true,
                    tablesTested = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Získat log z debug služby
        /// </summary>
        [HttpGet("log")]
        public ActionResult GetLog()
        {
            return Ok(new
            {
                log = _debugService.Log,
                logCount = _debugService.Log.Count
            });
        }

        /// <summary>
        /// Vyčistit log
        /// </summary>
        [HttpPost("clear-log")]
        public ActionResult ClearLog()
        {
            _debugService.Log.Clear();
            return Ok(new
            {
                success = true,
                message = "Log vyčištěn"
            });
        }

        // =====================================================================
        //  SPECIÁLNÍ TESTOVACÍ ENDPOINTY
        // =====================================================================

        /// <summary>
        /// Test struktury objednávek
        /// </summary>
        [HttpGet("orders-structure/{year}")]
        public ActionResult GetOrdersStructure(int year)
        {
            try
            {
                var result = _debugService.AnalyzeOrdersStructure(year);

                return Ok(new
                {
                    success = true,
                    year = year,
                    analysis = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Test filtrování objednávek (jen otevřené, atd.)
        /// </summary>
        [HttpGet("test-filters/{year}")]
        public ActionResult TestFilters(int year)
        {
            try
            {
                var result = _debugService.TestOrderFilters(year);

                return Ok(new
                {
                    success = true,
                    year = year,
                    filters = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Komplexní test všech dat
        /// </summary>
        [HttpGet("full-test")]
        public ActionResult FullTest()
        {
            try
            {
                var result = new Dictionary<string, object>();

                // 1. Roky
                var years = _debugService.LoadYears();
                result["years"] = new
                {
                    count = years.Count,
                    years = years
                };

                // 2. Adresy
                var addresses = _debugService.LoadAddresses();
                result["addresses"] = new
                {
                    count = addresses.Count,
                    sample = addresses.Count > 0 ? addresses[0] : null
                };

                // 3. Pro každý rok objednávky
                var ordersByYear = new Dictionary<int, object>();
                foreach (var yearItem in years)
                {
                    int year = (int)yearItem["Year"];
                    try
                    {
                        var orders = _debugService.LoadOrdersForYear(year);
                        ordersByYear[year] = new
                        {
                            count = orders.Count,
                            sample = orders.Count > 0 ? orders[0] : null
                        };
                    }
                    catch (System.Exception ex)
                    {
                        ordersByYear[year] = new { error = ex.Message };
                    }
                }
                result["ordersByYear"] = ordersByYear;

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // =====================================================================
        //  POMOCNÉ ENDPOINTY PRO LADĚNÍ
        // =====================================================================

        /// <summary>
        /// Test přístupu ke konkrétní tabulce v konkrétním roce
        /// </summary>
        [HttpGet("test-table/{year}/{tableName}")]
        public ActionResult TestSpecificTable(int year, string tableName)
        {
            try
            {
                var result = _debugService.TestTableAccess(year, tableName);

                return Ok(new
                {
                    success = true,
                    year = year,
                    table = tableName,
                    result = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Zjistit dostupné tabulky v konkrétním roce
        /// </summary>
        [HttpGet("available-tables/{year}")]
        public ActionResult GetAvailableTables(int year)
        {
            try
            {
                var result = _debugService.GetAvailableTables(year);

                return Ok(new
                {
                    success = true,
                    year = year,
                    tables = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Test rychlosti načítání
        /// </summary>
        [HttpGet("performance-test")]
        public ActionResult PerformanceTest()
        {
            try
            {
                var result = _debugService.RunPerformanceTest();

                return Ok(new
                {
                    success = true,
                    performance = result
                });
            }
            catch (System.Exception ex)
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