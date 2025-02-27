using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ChartWebApp.Controllers;

public class MainController : Controller
{
    public String Index()
    {
        return "Hello World!";
    }

    public String About()
    {
        return "Hello World! About!";
    }
    
    
    [HttpPost]
    public IActionResult ImportCsv(IFormFile file)
    {
        var folderName = Path.Combine("Resources", "Data");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (file.Length > 0)
        {
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        return RedirectToAction("Index");
    }
}