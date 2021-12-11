using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public IActionResult Get()
        {
            var result = new
            {
                _links = new
                {
                    vehicles = new
                    {
                        href = "/api/vehicles"
                    }
                },

                _actions = new
                {
                    create = new
                    {
                        href = "/api/vehicles",
                        type = "application/json",
                        method = "POST",
                        name = "Add a new vehicle to Autobarn",
                    },
                },

                message = "Welcome to the Autobarn API!",
            };

            return Ok(result);
        }
    }
}
