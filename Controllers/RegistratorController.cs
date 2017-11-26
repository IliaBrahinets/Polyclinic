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

        public IActionResult DoctorData() {
            return View();
        }
        
        public IActionResult SearchDoctor() {
            return View();
        }

        public IActionResult CreateDoctor() {
            return View();
        }

        public IActionResult SearchPatient() {
            return View();
        }


        public IActionResult CreatePatient() {

            return View();
            
        }

        public IActionResult DoctorScheduleView() {

            ViewData["ParentController"] = "Registrator";
            
            return View();
        }

        public IActionResult DoctorScheduleEdit() {
            
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
