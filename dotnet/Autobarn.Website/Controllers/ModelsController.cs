using Autobarn.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Autobarn.Website.Controllers
{
    public class ModelsController : Controller
    {
        private readonly IAutobarnDatabase db;

        public ModelsController(IAutobarnDatabase db)
        {
            this.db = db;
        }

        public IActionResult Vehicles(string id)
        {
            Data.Entities.Model model = this.db.ListModels().FirstOrDefault(m => m.Code == id);
            return this.View(model);
        }

        public IActionResult Index()
        {
            System.Collections.Generic.IEnumerable<Data.Entities.Model> models = this.db.ListModels();
            return this.View(models);
        }
    }
}
