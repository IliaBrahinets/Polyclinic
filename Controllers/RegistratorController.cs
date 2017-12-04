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

        public IActionResult PatientCard()
        {

            ViewData["ParentController"] = "Registrator";

            return View();
        }

        public IActionResult Doctors()
        {
            return View();
        }

        public IActionResult CreateDoctor()
        {
            return View();
        }

        public IActionResult Patients()
        {
            return View();
        }


        public IActionResult CreatePatient()
        {

            return View();

        }

        public IActionResult DoctorScheduleView()
        {

            ViewData["ParentController"] = "Registrator";

            return View();
        }

        public IActionResult DoctorScheduleEdit()
        {

            return View();
        }

        public IActionResult Regions()
        {
            return View();
        }

        public IActionResult CreateRegion()
        {
            return View();
        }

        public IActionResult Specialities()
        {
            return View();
        }

        public IActionResult CreateSpeciality()
        {
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
