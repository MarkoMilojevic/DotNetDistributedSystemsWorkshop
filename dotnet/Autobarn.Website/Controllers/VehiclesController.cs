﻿using Autobarn.Data;
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
            System.Collections.Generic.IEnumerable<Vehicle> vehicles = this.db.ListVehicles();
            return this.View(vehicles);
        }

        public IActionResult Details(string id)
        {
            Vehicle vehicle = this.db.FindVehicle(id);
            return this.View(vehicle);
        }

        [HttpGet]
        public IActionResult Advertise(string id)
        {
            Model vehicleModel = this.db.FindModel(id);
            VehicleDto dto = new VehicleDto()
            {
                ModelCode = vehicleModel.Code,
                ModelName = $"{vehicleModel.Manufacturer.Name} {vehicleModel.Name}"
            };
            return this.View(dto);
        }

        [HttpPost]
        public IActionResult Advertise(VehicleDto dto)
        {
            Vehicle existingVehicle = this.db.FindVehicle(dto.Registration);
            if (existingVehicle != default)
                this.ModelState.AddModelError(nameof(dto.Registration),
                    "That registration is already listed in our database.");
            Model vehicleModel = this.db.FindModel(dto.ModelCode);
            if (vehicleModel == default)
            {
                this.ModelState.AddModelError(nameof(dto.ModelCode),
                    $"Sorry, {dto.ModelCode} is not a valid model code.");
            }
            if (!this.ModelState.IsValid) return this.View(dto);
            Vehicle vehicle = new Vehicle()
            {
                Registration = dto.Registration,
                Color = dto.Color,
                VehicleModel = vehicleModel,
                Year = dto.Year
            };
            this.db.CreateVehicle(vehicle);
            return this.RedirectToAction("Details", new { id = vehicle.Registration });
        }
    }
}
