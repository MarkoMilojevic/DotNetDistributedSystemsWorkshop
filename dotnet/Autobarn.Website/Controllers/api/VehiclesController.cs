using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly int PageSize = 10;

        private readonly IAutobarnDatabase db;

        public VehiclesController(IAutobarnDatabase db)
        {
            this.db = db;
        }

        // GET: api/vehicles
        [HttpGet]
        public IActionResult Get(int index = 0)
        {
            dynamic _links = new ExpandoObject();
            _links.self = new { href = $"/api/vehicles?index={index}" };
            _links.next = new { href = $"/api/vehicles?index={index + PageSize}" };

            if (index > 0)
            {
                _links.previous = new { href = $"/api/vehicles?index={Math.Max(index - PageSize, 0)}" };
            }

            IEnumerable<Vehicle> items =
                db
                    .ListVehicles()
                    .Skip(index)
                    .Take(PageSize);

            var result = new
            {
                _links,
                items
            };

            return Ok(result);
        }

        // GET api/vehicles/ABC123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Vehicle vehicle = db.FindVehicle(id);

            return vehicle == default
                ? NotFound()
                : (IActionResult)Ok(vehicle);
        }

        // POST api/vehicles
        [HttpPost]
        public IActionResult Post([FromBody] VehicleDto dto)
        {
            Model vehicleModel = db.FindModel(dto.ModelCode);
            Vehicle vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };
            db.CreateVehicle(vehicle);
            return Ok(dto);
        }

        // PUT api/vehicles/ABC123
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] VehicleDto dto)
        {
            Model vehicleModel = db.FindModel(dto.ModelCode);
            Vehicle vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                ModelCode = vehicleModel.Code
            };
            db.UpdateVehicle(vehicle);
            return Ok(dto);
        }

        // DELETE api/vehicles/ABC123
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Vehicle vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            db.DeleteVehicle(vehicle);
            return NoContent();
        }
    }
}
