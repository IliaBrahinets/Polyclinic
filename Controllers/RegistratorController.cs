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
            return View(db.Regions.Include(x => x.Streets));
        }

        public IActionResult CreateRegion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegion([FromBody]ICollection<Street> Streets)
        {
            Region region = new Region { Streets = Streets }; 
            
            foreach(Street Street in Streets)
            {
                db.Streets.Add(Street);
            }
            db.Regions.Add(region);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> DeleteRegion(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var region = await db.Regions.FirstOrDefaultAsync(x => x.ID == id);
            if (region != null)
            {
                db.Regions.Remove(region);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> EditStreet([Bind("ID,Name,Addresses,RegionID")]Street street)
        {
            db.Streets.Update(street);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Regions));
        }
        public async Task<IActionResult> DeleteStreet(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var Street = await db.Streets.FirstOrDefaultAsync(x => x.ID == id);

            if (Street != null)
            {
                db.Streets.Remove(Street);

                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Regions));

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

        
        [HttpGet]
        public async Task<IActionResult> EditSpeciality([Bind("ID,Name,CheckUpTime")]Speciality spec)
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



        public async Task<IActionResult> DiseasesDirectory()
        {
            return View(await db.Diseases.ToListAsync());
        }

        public IActionResult CreateDisease()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDisease([Bind("ID,Name,Description")] Disease disease)
        {
            if (ModelState.IsValid)
            {
                db.Add(disease);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(DiseasesDirectory));
            }
            return View(disease);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteDisease(int? id)
        {
            if (id != null)
            {
                Disease disease = new Disease { ID = id.Value };
                db.Entry(disease).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(DiseasesDirectory));
            }
            return NotFound();
        }
        public IActionResult EditDisease()
        {
            return View();
        }


        public async Task<IActionResult> DrugsDirectory()
        {
            return View(await db.Drugs.ToListAsync());
        }

        public IActionResult CreateDrug()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDrug([Bind("ID,Name,Description")] Drug drug)
        {
            if (ModelState.IsValid)
            {
                db.Add(drug);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(DrugsDirectory));
            }
            return View(drug);
        }

        [HttpGet]
        public async Task<IActionResult> EditDrug([Bind("ID,Name,Description")]Drug drug)
        {
            db.Drugs.Update(drug);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(DrugsDirectory));
        }


        [HttpGet]
        public async Task<IActionResult> DeleteDrugs(int? id)
        {
            if (id != null)
            {
                Drug drug = new Drug { ID = id.Value };
                db.Entry(drug).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(DrugsDirectory));
            }
            return NotFound();
        }
        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
