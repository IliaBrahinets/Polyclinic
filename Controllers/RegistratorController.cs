using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Polyclinic.Data;
using Polyclinic.Models;
using System.Diagnostics;

namespace Polyclinic.Controllers
{
    public class RegistratorController : Controller
    {
        private readonly PolyclinicContext _context;


        public RegistratorController(PolyclinicContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> Specialities()
        {
            return View(await _context.Specialities.ToListAsync());
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
