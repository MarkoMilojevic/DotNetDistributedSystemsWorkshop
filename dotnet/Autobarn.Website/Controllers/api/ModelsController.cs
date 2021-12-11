using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Website.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IAutobarnDatabase db;

        public ModelsController(IAutobarnDatabase db)
        {
            this.db = db;
        }

        [HttpGet]
        [Produces("application/hal+json")]
        public IActionResult Get()
        {
            IEnumerable<dynamic> models =
                this.db
                    .ListModels()
                    .Select(m => m.ToHypermediaResource());

            return this.Ok(models);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Model vehicleModel = this.db.FindModel(id);
            if (vehicleModel == default) return this.NotFound();
            return this.Ok(vehicleModel);
        }

        [HttpPost("{id}")]
        [Produces("application/hal+json")]
        public IActionResult Post(string id, [FromBody] VehicleDto dto)
        {
            Model vehicleModel = this.db.FindModel(id);
            if (vehicleModel == default) return this.NotFound();
            Vehicle existing = this.db.FindVehicle(dto.Registration);
            if (existing != default) return this.Conflict($"Sorry, vehicle {dto.Registration} already exists in our database and you're not allowed to sell the same car twice.");
            Vehicle vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };
            this.db.CreateVehicle(vehicle);
            return Created($"/api/vehicles/{vehicle.Registration}", vehicle.ToHypermediaResource());
        }
    }
}