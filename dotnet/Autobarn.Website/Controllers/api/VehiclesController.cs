using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Autobarn.Website.MessageHandlers;
using Autobarn.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        const int PageSize = 10;

        private readonly IAutobarnDatabase db;
        private readonly IBus bus;

        public VehiclesController(IAutobarnDatabase db, IBus bus)
        {
            this.db = db;
            this.bus = bus;
        }

        // GET: api/vehicles
        [HttpGet]
        public IActionResult Get(int index = 0, int count = PageSize, string expand = "")
        {
            int total = this.db.CountVehicles();

            Dictionary<string, object> _links =
                HypermediaExtensions.Paginate(
                    baseUrl: "https://localhost:5001/api/vehicles",
                    index: index,
                    count: count,
                    total: total);

            IEnumerable<dynamic> items =
                this.db
                    .ListVehicles()
                    .Skip(index)
                    .Take(PageSize)
                    .Select(vehicle => vehicle.ToHypermediaResource(expand));

            var result = new
            {
                _links,
                items,
            };

            return this.Ok(result);
        }

        // GET: api/vehicles
        [HttpGet("registration/{registration}")]
        public IActionResult GetByRegistration(char registration = 'A')
        {
            Dictionary<string, object> _links =
                HypermediaExtensions.PaginateByLicensePlate(
                    baseUrl: "https://localhost:5001/api/vehicles",
                    index: registration);

            IEnumerable<dynamic> items =
                this.db
                    .ListVehicles()
                    .Where(v => v.Registration.StartsWith(registration))
                    .Select(vehicle => vehicle.ToHypermediaResource());

            var result = new
            {
                _links,
                items,
            };

            return this.Ok(result);
        }

        // GET api/vehicles/ABC123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Vehicle vehicle = this.db.FindVehicle(id);
            if (vehicle == default)
            {
                return this.NotFound();
            }

            return Ok(vehicle.ToHypermediaResource());
        }

        // POST api/vehicles
        [HttpPost]
        public IActionResult Post([FromBody] VehicleDto dto)
        {
            Vehicle existingVehicle = this.db.FindVehicle(dto.Registration);
            if (existingVehicle != default)
            {
                string error =
                    $"Sorry, vehicle {dto.Registration} already exists " +
                    $"in our database and you're not allowed " +
                    $"to sell the same car twice.";

                return base.Conflict(error);
            }

            Model vehicleModel = this.db.FindModel(dto.ModelCode);
            if (vehicleModel == null)
            {
                return this.BadRequest($"Sorry, we don't know what kind of car '{dto.ModelCode}' is.");
            }

            Vehicle vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };

            this.db.CreateVehicle(vehicle);

            this.bus.PublishNewVehicleMessage(vehicle);

            return this.Created($"/api/vehicles/{vehicle.Registration}", dto);
        }

        // PUT api/vehicles/ABC123
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] VehicleDto dto)
        {
            Model vehicleModel = this.db.FindModel(dto.ModelCode);
            
            Vehicle vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                ModelCode = vehicleModel.Code
            };

            Vehicle existingVehicle = this.db.FindVehicle(id);
            if (existingVehicle == default)
            {
                this.bus.PublishNewVehicleMessage(vehicle);
            }

            this.db.UpdateVehicle(vehicle);

            return this.Ok(dto);
        }

        // DELETE api/vehicles/ABC123
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Vehicle vehicle = this.db.FindVehicle(id);
            if (vehicle == default) return this.NotFound();
            this.db.DeleteVehicle(vehicle);
            return this.NoContent();
        }
    }
}
