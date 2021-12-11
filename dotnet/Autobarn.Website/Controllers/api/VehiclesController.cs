using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
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

        public VehiclesController(IAutobarnDatabase db)
        {
            this.db = db;
        }

        // GET: api/vehicles
        [HttpGet]
        public IActionResult Get(int index = 0, int count = PageSize, string expand = "")
        {
            int total = db.CountVehicles();

            Dictionary<string, object> _links =
                HypermediaExtensions.Paginate(
                    baseUrl: "https://localhost:5001/api/vehicles",
                    index: index,
                    count: count,
                    total: total);

            IEnumerable<dynamic> items =
                db
                    .ListVehicles()
                    .Skip(index)
                    .Take(PageSize)
                    .Select(vehicle => vehicle.ToHypermediaResource(expand));

            var result = new
            {
                _links,
                items,
            };

            return Ok(result);
        }

        // GET: api/vehicles
        [HttpGet("registration/{registration}")]
        public IActionResult GetByRegistration(char registration = 'A')
        {
            int total = db.CountVehicles();

            Dictionary<string, object> _links =
                HypermediaExtensions.PaginateByLicensePlate(
                    baseUrl: "https://localhost:5001/api/vehicles",
                    index: registration);

            IEnumerable<dynamic> items =
                db
                    .ListVehicles()
                    .Where(v => v.Registration.StartsWith(registration))
                    .Select(vehicle => vehicle.ToHypermediaResource());

            var result = new
            {
                _links,
                items,
            };

            return Ok(result);
        }

        // GET api/vehicles/ABC123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Vehicle vehicle = db.FindVehicle(id);
            if (vehicle == default)
            {
                return NotFound();
            }

            return Ok(vehicle.ToHypermediaResource());
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
