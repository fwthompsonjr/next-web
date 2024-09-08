using Microsoft.AspNetCore.Mvc;
using next.processor.models;

namespace next.processor.api.Controllers
{
    [Route("/[controller]")]
    public class DataController(DrillDownModel model) : Controller
    {
        private readonly DrillDownModel drillDownSvc = model;

        [HttpGet("status")]
        public IActionResult Index([FromQuery] string id = "")
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
            drillDownSvc.Name = id;
            return RedirectToAction("Status", "Home");
        }
    }
}