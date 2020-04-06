using System.Diagnostics;
using CoolStudents.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoolStudents.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToRoute(new {  controller = "student", action = "index" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
