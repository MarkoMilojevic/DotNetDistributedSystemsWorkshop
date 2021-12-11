using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Autobarn.Website.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IAutobarnDatabase db;

        public VehiclesController(IAutobarnDatabase db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            System.Collections.Generic.IEnumerable<Vehicle> vehicles = db.ListVehicles();
            return View(vehicles);
        }

        public IActionResult Details(string id)
        {
            Vehicle vehicle = db.FindVehicle(id);
            return View(vehicle);
        }

        [HttpGet]
        public IActionResult Advertise(string id)
        {
            Model vehicleModel = db.FindModel(id);
            VehicleDto dto = new VehicleDto()
            {
                ModelCode = vehicleModel.Code,
                ModelName = $"{vehicleModel.Manufacturer.Name} {vehicleModel.Name}"
            };
            return View(dto);
        }

        [HttpPost]
        public IActionResult Advertise(VehicleDto dto)
        {
            Vehicle existingVehicle = db.FindVehicle(dto.Registration);
            if (existingVehicle != default)
                ModelState.AddModelError(nameof(dto.Registration),
                    "That registration is already listed in our database.");
            Model vehicleModel = db.FindModel(dto.ModelCode);
            if (vehicleModel == default)
            {
                ModelState.AddModelError(nameof(dto.ModelCode),
                    $"Sorry, {dto.ModelCode} is not a valid model code.");
            }
            if (!ModelState.IsValid) return View(dto);
            Vehicle vehicle = new Vehicle()
            {
                Registration = dto.Registration,
                Color = dto.Color,
                VehicleModel = vehicleModel,
                Year = dto.Year
            };
            db.CreateVehicle(vehicle);
            return RedirectToAction("Details", new { id = vehicle.Registration });
        }
    }
}
