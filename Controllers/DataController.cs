using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChartWebApp.Data;
using ChartWebApp.DTO;
using ChartWebApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace ChartWebApp.Controllers
{
    public class DataController : Controller
    {
        private readonly ChartWebAppContext _context;

        public DataController(ChartWebAppContext context)
        {
            _context = context;
        }

        // GET: Data
        public async Task<IActionResult> Index(string sortOrder, string DateFrom, string DateTo)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (Request.HasFormContentType)
            {
                if (!Request.Form["DateFrom"].ToString().IsNullOrEmpty())
                    dateFrom = DateTime.ParseExact(Request.Form["DateFrom"], "yyyy-MM-dd'T'HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture);
                if (!Request.Form["DateTo"].ToString().IsNullOrEmpty())
                    dateTo = DateTime.ParseExact(Request.Form["DateTo"], "yyyy-MM-dd'T'HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture);
            }
            var data = from s in _context.Data
                select s;
            if (dateFrom != null)
                data = data.Where(x => x.DateTime >= dateFrom);
            if (dateTo != null)
                data = data.Where(x => x.DateTime <= dateTo);
            switch (sortOrder)
            {
                case "date_desc":
                    data = data.OrderByDescending(d => d.DateTime);
                    break;
                case "price_desc":
                    data = data.OrderByDescending(d => d.MarketPrice);
                    break;
                case "Price":
                    data = data.OrderBy(d => d.MarketPrice);
                    break;
                default:
                    data = data.OrderBy(d => d.DateTime);
                    break;
            }

            var result = await data.AsNoTracking().ToListAsync(); 
            return View(result);
        }

        // GET: Data/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Data == null)
            {
                return NotFound();
            }

            var data = await _context.Data
                .FirstOrDefaultAsync(m => m.Id == id);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // GET: Data/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Data/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,MarketPrice")] ChartData chartData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(chartData);
        }

        // GET: Data/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Data == null)
            {
                return NotFound();
            }

            var data = await _context.Data.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // POST: Data/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,MarketPrice")] ChartData chartData)
        {
            if (id != chartData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataExists(chartData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(chartData);
        }

        // GET: Data/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Data == null)
            {
                return NotFound();
            }

            var data = await _context.Data
                .FirstOrDefaultAsync(m => m.Id == id);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // POST: Data/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Data == null)
            {
                return Problem("Entity set 'ChartWebAppContext.Data'  is null.");
            }

            var data = await _context.Data.FindAsync(id);
            if (data != null)
            {
                _context.Data.Remove(data);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DataExists(int id)
        {
            return (_context.Data?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not selected");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // csv.Configuration.DetectColumnCountChanges = true;
                    var records = csv.GetRecords<CsvData>().ToList();
                    var entities = records.Select(x => new ChartData()
                    {
                        DateTime = DateTime.ParseExact(x.Date, new[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy" },
                            CultureInfo.InvariantCulture, DateTimeStyles.None),
                        MarketPrice = x.MarketPrice
                    });
                    _context.Data.AddRange(entities);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> GetChartData()
        {
            var data = await _context.Data.OrderBy(x => x.DateTime).ToListAsync();
            var labels = data.Select(x => x.DateTime.Hour + ":" + x.DateTime.Minute).Distinct();
            var distinctDay = data.Select(x => x.DateTime.Date).Distinct();
            var dataGraph = new List<GraphData>();
            int i = 0;
            foreach (var day in distinctDay)
            {
                dataGraph.Add(new GraphData()
                {
                    label = day.Date.ToString("dd/MM/yyyy"),
                    data = data.Where(x => x.DateTime.Date == day.Date).Select(x => x.MarketPrice).ToList(),
                    borderColor = GetRandomChartColor(),
                    fill = false
                });
            }
            var chartData = new
            {
                labels = labels,
                datasets = dataGraph
            };

            return Json(chartData);
        }
        
        public async Task<IActionResult> GetChartData2()
        {
            var data = await _context.Data.OrderBy(x => x.DateTime).ToListAsync();
            var labels = data.Select(x => x.DateTime.ToString("dd/MM/yyyy HH:mm"));
            var dataGraph = new List<GraphData>();
            dataGraph.Add(new GraphData()
            {
                label = "MarketPrice",
                data = data.Select(x => x.MarketPrice).ToList(),
                borderColor = GetRandomChartColor(),
                fill = false
            });
            var chartData = new
            {
                labels = labels,
                datasets = dataGraph
            };

            return Json(chartData);
        }
        
        public static string GetRandomChartColor()
        {
            Random random = new Random();
            int r = random.Next(0, 256); // Random Red (0-255)
            int g = random.Next(0, 256); // Random Green (0-255)
            int b = random.Next(0, 256); // Random Blue (0-255)
            double a = Math.Round(random.NextDouble() * 0.5 + 0.5, 2); // Alpha (0.5 - 1.0)

            // return $"rgba({r}, {g}, {b}, {a})";
            return RgbaToHex(r, g, b, a);
        }
        
        public static string RgbaToHex(int r, int g, int b, double a)
        {
            int alpha = (int)(a * 255);
            return $"#{r:X2}{g:X2}{b:X2}{alpha:X2}";
        }

        [HttpGet]
        public async Task<IActionResult> GetMinValue()
        {
            var min = await _context.Data.OrderBy(x => x.MarketPrice).FirstOrDefaultAsync();
            return Json(min);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMaxValue()
        {
            var max = await _context.Data.OrderByDescending(x => x.MarketPrice).FirstOrDefaultAsync();
            return Json(max);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAverageValue()
        {
            var average = await _context.Data.AverageAsync(x => x.MarketPrice);
            return Json(average);
        }
        [HttpGet]
        public async Task<IActionResult> GetMostExpensiveHour()
        {
            var expensiveHour = await _context.Data.AsQueryable().Join(_context.Data, first => first.DateTime.AddMinutes(30),
                second => second.DateTime,
                (first, second) => new
                {
                    Start = first.DateTime,
                    End = second.DateTime,
                    MarketPrice = first.MarketPrice + second.MarketPrice
                }).OrderByDescending(x => x.MarketPrice).FirstOrDefaultAsync();
            return Json(expensiveHour);
        }
    }
}

public class GraphData
{
    public string label { get; set; }
    public List<decimal> data { get; set; }
    public string borderColor { get; set; }
    public bool fill { get; set; }
}