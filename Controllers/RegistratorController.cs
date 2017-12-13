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

         
            return View(db.Doctors.Include(x => x.Speciality).Include(x => x.Region));
        }

        public async Task<IActionResult> CreateDoctor()
        {

            ViewBag.Specialities = await db.Specialities.ToListAsync();
            ViewBag.Regions = await db.Regions.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor([Bind("Name,Surname,Lastname,ChainedCabinet,SpecialityId,RegionId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
               
                db.Add(doctor);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Doctors));
            }
            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDoctor(int? id)
        {
            if (id != null)
            {
                Doctor doctor = new Doctor { Id = id.Value };
                db.Entry(doctor).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Doctors));
            }
            return NotFound();
        }
        public async Task<IActionResult> Patients()
         {
             return View(await db.Patients.ToListAsync());
        }

    public IActionResult CreatePatient()
    {

        return View();

    }
        [HttpPost]
         [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("Id,Name,Surname,Lastname,BirthDate,Address,Sex")] Patient patient)
        {
             if (ModelState.IsValid)
             {
                 db.Patients.Add(patient);
                 await db.SaveChangesAsync();
                 return RedirectToAction(nameof(Patients));
            }
             return View(patient);
         }
         [HttpGet]
         public async Task<IActionResult> DeletePatient(int? id)
         {
             if (id != null)
             {
                Patient patient = new Patient { Id = id.Value };
                db.Entry(patient).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                 return RedirectToAction(nameof(Patients));
             }
            return NotFound();
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

                //q is a Id 
                int Id;

                if (Int32.TryParse(q, out Id))
                {
                    Region Region = db.Regions.Find(Id);

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

            bool exist = await db.Regions.AnyAsync(x => x.Id == id);

            if (exist)
            {
                Region region = new Region { Id = (int)id };
                db.Entry(region).State = EntityState.Deleted;

                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Regions));

        }

        public async Task<IActionResult> EditStreet([Bind("Id,Name,Addresses,RegionId")]Street street)
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

            bool exist = await db.Streets.AnyAsync(x => x.Id == id);

            if (exist)
            {
                Street street = new Street { Id = (int)id  };
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

       
      
        public async Task<IActionResult> EditSpeciality([Bind("Id,Name,CheckUpTime")]Speciality spec)
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
                 
            Speciality spec = new Speciality { Id = id.Value };

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
                db.Diseases.Add(disease);
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
                    
            Disease disease = new Disease { Id = id.Value };

            db.Entry(disease).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(DiseasesDirectory));
           
        }

        [HttpGet]
        public async Task<IActionResult> EditDisease([Bind("Id,Name,Description")]Disease disease)
        {
            db.Diseases.Update(disease);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(DiseasesDirectory));
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
                db.Drugs.Add(drug);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DrugsDirectory));
        }

        [HttpGet]
        public async Task<IActionResult> EditDrug([Bind("Id,Name,Description")]Drug drug)
        {
            db.Drugs.Update(drug);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(DrugsDirectory));
        }


        [HttpGet]
        public async Task<IActionResult> DeleteDrug(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Drug drug = new Drug { Id = id.Value };

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
