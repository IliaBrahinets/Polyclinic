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
        private readonly PolyclinicContext db;


        public RegistratorController(PolyclinicContext context)
        {
            db = context;
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
            return View(await db.Specialities.ToListAsync());
        }

        public IActionResult CreateSpeciality()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpeciality([Bind("ID,Name,CheckUpTime")] Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                db.Add(speciality);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Specialities));
            }
            return View(speciality);
        }

        public async Task<IActionResult> EditSpeciality(int? id)
        {
            if (id != null)
            {
                Speciality spec = await db.Specialities.FirstOrDefaultAsync(p => p.ID == id);
                if (spec != null)
                    return View(spec);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditSpeciality(Speciality spec)
        {
            db.Specialities.Update(spec);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Specialities));
        }

        [HttpGet]
       // [ActionName("DeleteSpeciality")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Speciality spec = await db.Specialities.FirstOrDefaultAsync(p => p.ID == id);
                if (spec != null)
                    return View(spec);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSpeciality(int? id)
        {
            if (id != null)
            {
                Speciality spec = new Speciality { ID = id.Value };
                db.Entry(spec).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Specialities));
            }
            return NotFound();
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
