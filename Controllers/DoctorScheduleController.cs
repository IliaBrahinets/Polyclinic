using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Polyclinic.Models;

namespace Polyclinic.Controllers
{
    public class DoctorScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
