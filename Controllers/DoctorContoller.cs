using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnetapp.Models;

namespace aspnetapp.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WriteCheckUpInfo()
        {

            return View();

        }

        //it's a shared page with Registrator
        public IActionResult PatientCard()
        {

            ViewData["ParentController"] = "Doctor";

            return View();
        }

        public IActionResult DoctorScheduleView()
        {

            ViewData["ParentController"] = "Doctor";

            return View();
        }

        public IActionResult DiseasesDirectory()
        {
            return View();
        }

        public IActionResult CreateDisease()
        {
            return View();
        }

        public IActionResult DrugsDirectory()
        {
            return View();
        }

        public IActionResult CreateDrug()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
