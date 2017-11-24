using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnetapp.Models;

namespace aspnetapp.Controllers
{
    public class RegistratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PatientCard() {
            ViewData["ParentController"] = "Registrator";
            
            return View();
        }
        
        [HttpPost]
        public IActionResult CreatePatient() {
            
            Console.WriteLine(Request.Form["pwd"]);

            return null;

        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
