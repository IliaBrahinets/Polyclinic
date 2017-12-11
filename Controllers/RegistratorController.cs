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

        public async Task<IActionResult> Regions(String q)
        {
            if (q != null)
            {

                //q is a ID 
                int ID;

                if (Int32.TryParse(q, out ID))
                {
                    Region Region = db.Regions.Find(ID);

                    db.Entry(Region).Collection(x => x.Streets).Load();

                    return View(new List<Region> { Region });
                }

                //q is a street name
                q = q.ToLower();

                List<Region> result = new List<Region>();

                dynamic Regions = await db.Regions.Include(x => x.Streets).ToListAsync();

                foreach (Region Region in Regions)
                {
                    
                    foreach (Street Street in Region.Streets)

                        if (Street.Name.ToLower().Contains(q))
                        {
                            result.Add(Region);
                            break;
                        }
                }

                return View(result);

            }
            else
            {
                return View(await db.Regions.Include(x => x.Streets).ToListAsync());
            }
        }

        public IActionResult CreateRegion()
        {
            return View();
        }   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRegion([Bind("Name,Addresses")]ICollection<Street> Streets)
        {

            if (ModelState.IsValid)
            {
                Region region = new Region { Streets = Streets };

                foreach (Street Street in Streets)
                {
                    db.Streets.Add(Street);
                }
                db.Regions.Add(region);

                await db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> DeleteRegion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            bool exist = await db.Regions.AnyAsync(x => x.ID == id);

            if (exist)
            {
                Region region = new Region { ID = (int)id };
                db.Entry(region).State = EntityState.Deleted;

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

            bool exist = await db.Streets.AnyAsync(x => x.ID == id);

            if (exist)
            {
                Street street = new Street { ID = (int)id  };
                db.Entry(street).State = EntityState.Deleted;

                await db.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(Regions));

        }

        public IActionResult Specialities(String q)
        {
           
            if (q != null)
            {
                q = q.ToLower();
                return View(db.Specialities.Where(x => (x.Name.ToLower().Contains(q))));
            }
            else
            {
               return View(db.Specialities.ToList());
            }
        }

        public IActionResult CreateSpeciality()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpeciality([Bind("Name,CheckUpTime")] Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                db.Add(speciality);
                await db.SaveChangesAsync();
               
            }
            return RedirectToAction(nameof(Specialities));
        }

       
      
        public async Task<IActionResult> EditSpeciality([Bind("ID,Name,CheckUpTime")]Speciality spec)
        {
            db.Specialities.Update(spec);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Specialities));
        }

      
        public async Task<IActionResult> DeleteSpeciality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
                 
            Speciality spec = new Speciality { ID = id.Value };

            db.Entry(spec).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Specialities));
           
        }



        public IActionResult DiseasesDirectory(String q)
        {
            if (q != null)
            {
                q = q.ToLower();

                return View(db.Diseases.Where(x => (x.Name.ToLower().Contains(q) || x.Description.ToLower().Contains(q))));
            }
            else
            {
                return View(db.Diseases.ToList());
            }
        }

        public IActionResult CreateDisease()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDisease([Bind("Name,Description")] Disease disease)
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
            if (id == null)
            {
                return NotFound();
            }
                    
            Disease disease = new Disease { ID = id.Value };

            db.Entry(disease).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(DiseasesDirectory));
           
        }
        public IActionResult EditDisease()
        {
            return View();
        }


        public IActionResult DrugsDirectory(String q)
        {
            if (q != null)
            { 
                q = q.ToLower();

                return View(db.Drugs.Where(x => ( x.Name.ToLower().Contains(q) || x.Description.ToLower().Contains(q) ) ) );
            }
            else
            {
                return View(db.Drugs.ToList());
            }
        }

        public IActionResult CreateDrug()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDrug([Bind("Name,Description")] Drug drug)
        {
            if (ModelState.IsValid)
            {
                db.Add(drug);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DrugsDirectory));
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
            if (id == null)
            {
                return NotFound();
            }
            Drug drug = new Drug { ID = id.Value };

            db.Entry(drug).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(DrugsDirectory));
           
        }
        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
