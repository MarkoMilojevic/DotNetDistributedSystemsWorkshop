using Autobarn.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Autobarn.Website.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IAutobarnDatabase db;

        public ManufacturersController(IAutobarnDatabase db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            System.Collections.Generic.IEnumerable<Data.Entities.Manufacturer> vehicles = db.ListManufacturers();
            return View(vehicles);
        }

        public IActionResult Models(string id)
        {
            Data.Entities.Manufacturer manufacturer = db.ListManufacturers().FirstOrDefault(m => m.Code == id);
            return View(manufacturer);
        }
    }
}
