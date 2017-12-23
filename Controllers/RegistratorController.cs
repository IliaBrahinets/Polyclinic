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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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

        public IActionResult PatientCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            db.Entry(patient).Collection(x => x.DoctorVisits).Load();
            db.Entry(patient).Reference(x => x.Street).Load();
            return View(patient);
        }


        public async Task<IActionResult> Doctors(string q, int w)
        {
            ViewBag.Specialities = await db.Specialities.ToListAsync();
            ViewBag.Regions = await db.Regions.ToListAsync();
            var Doctors = from m in db.Doctors select m;
            if (!String.IsNullOrEmpty(q))
            {
                Doctors = Doctors.Where(s => s.Speciality.Name.ToLower().Contains(q));
            }
            if (w != 0)
            {
                Doctors = Doctors.Where(x => x.Region.Id == w);
            }
            return View(Doctors);


        }

        public async Task<IActionResult> CreateDoctor()
        {

            ViewBag.Specialities = await db.Specialities.ToListAsync();
            ViewBag.Regions = await db.Regions.ToListAsync();

            return View("EditDoctor");
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

        [HttpPost]
        public async Task<JsonResult> IsDoctorNotExist([Bind("Name,Surname,Lastname,ChainedCabinet,SpecialityId,RegionId")] Doctor doctor)
        {
            if (doctor == null)
                return Json(data: true);

            if (!ModelState.IsValid)
            {
                return Json(data: true);
            }

            Doctor TryDoctor = await db.Doctors.FirstOrDefaultAsync(
                x => (x.Name.Equals(doctor.Name)
                      && x.Surname.Equals(doctor.Surname)
                      && x.Lastname.Equals(doctor.Lastname)
                      && x.ChainedCabinet.Equals(doctor.ChainedCabinet)
                      && x.SpecialityId.Equals(doctor.SpecialityId)
                      && x.RegionId.Equals(doctor.RegionId)));

            if (TryDoctor == null)
                return Json(data: true);

            return Json(data: "“акой доктор существует");


        }

        public async Task<JsonResult> IsCabinetAvaliable(int? Id, int? ChainedCabinet)
        {
            if (ChainedCabinet == null)
                return Json(data: true);

            Doctor TryDoctor = await db.Doctors.FirstOrDefaultAsync(x => 
                                        (x.ChainedCabinet == ChainedCabinet)
                                        &&(x.Id != Id));

            if (TryDoctor == null)
                return Json(data: true);

            return Json(data: " абинет зан€т");
        }

        public async Task<IActionResult> EditDoctor(int? id)
        {
            if (id != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(id);

                if (doctor == null)
                    return RedirectToAction(nameof(Doctors));

                ViewBag.Specialities = await db.Specialities.ToListAsync();
                ViewBag.Regions = await db.Regions.ToListAsync();

                return View(doctor);
            }

            return RedirectToAction(nameof(Doctors));
           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {

                db.Doctors.Update(doctor);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Doctors));
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
            List<Patient> patients = await db.Patients.ToListAsync();

            foreach (Patient patient in patients)
            {
                await db.Entry(patient).Reference(x => x.Street).LoadAsync();
            }


            return View(patients);
        }

        public async Task<IActionResult> CreatePatient()
        {

            return View("EditPatient");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("Name,Surname,Lastname,BirthDate,Sex,StreetId,StreetName,HouseNumber")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.CreationDate = DateTime.Today.Date;

                //we need check for this Street

                if (patient.StreetId == null)
                {
                    Street StreetMatched =
                           await db.Streets.FirstOrDefaultAsync(x => (x.Name.Equals(patient.StreetName)
                                             && x.Addresses.Contains(patient.HouseNumber)));


                    patient.StreetId = StreetMatched.Id;
                }



                db.Patients.Add(patient);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Patients));
            }
            return View(patient);
        }

        [HttpPost]
        public async Task<JsonResult> isPatientNotExist([Bind("Name,Surname,Lastname,BirthDate,Sex,StreetId,StreetName,HouseNumber")]Patient patient)
        {
            if (patient == null)
                return Json(data: true);

            if (!ModelState.IsValid)
            {
                return Json(data: true);
            }
            Patient TryPatient = await db.Patients.FirstOrDefaultAsync(
                x => (x.BirthDate.Equals(patient.BirthDate)
                      && x.Name.Equals(patient.Name) 
                      && x.Surname.Equals(patient.Surname)
                      && x.Lastname.Equals(patient.Lastname)
                      && x.Sex.Equals(patient.Sex)
                      && x.StreetId.Equals(patient.StreetId)
                      && x.HouseNumber.Equals(patient.HouseNumber)) );

            if(TryPatient == null)
                return Json(data: true);

            return Json(data: "“акой пациент существует");


        }

      
        public async Task<IActionResult> EditPatient(int? id)
        {
            if (id != null)
            {
                Patient patient = await db.Patients.FindAsync(id);

                if(patient == null)
                    return RedirectToAction(nameof(Patients));

                await db.Entry(patient).Reference(x => x.Street).LoadAsync();

                return View(patient);
            }

            return RedirectToAction(nameof(Patients));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPatient(Patient patient)
        {
            if (ModelState.IsValid)
            {
                if (patient.StreetId == null)
                {
                    Street StreetMatched =
                           await db.Streets.FirstOrDefaultAsync(x => (x.Name.Equals(patient.StreetName)
                                             && x.Addresses.Contains(patient.HouseNumber)));


                    patient.StreetId = StreetMatched.Id;
                }

                db.Patients.Update(patient);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Patients));
        }

        [HttpGet]
        public async Task<IActionResult> DeletePatient(int? id)
        {
            if (id != null)
            {
                Patient patient = new Patient { Id = id.Value };
                db.Entry(patient).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Patients));
        }


        public async Task<IActionResult> DoctorScheduleView(int? id)
        {
            Relieve relieve= new Relieve{Date = DateTime.Parse("23.11.17"),DoctorId = 1,StartTime=DateTime.Parse("08:00"),EndTime=DateTime.Parse("15:00") };
            db.Relieves.Add(relieve);
            db.SaveChanges();

            ViewData["ParentController"] = "Registrator";

            if(id != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(id);

                if (doctor != null)
                {
                    await db.Entry(doctor).Reference(x => x.Speciality).LoadAsync();

                    return View(doctor);
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> DoctorScheduleEdit(int? id)
        {
         
            if (id != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(id);

                if (doctor != null)
                {
                    await db.Entry(doctor).Reference(x => x.Speciality).LoadAsync();

                    return View(doctor);
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Relieves(int? Year,int? Month,int? Day,int? DoctorId)
        {
            if(DoctorId != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(DoctorId);

                if (doctor != null)
                {

                    IQueryable<Relieve> q = db.Entry(doctor).Collection(x => x.Schedule).Query();

                    List<Relieve> Relieves = await q.Where(x => (Year == null || x.Date.Year == Year)
                                                          && (Month == null || x.Date.Month == Month)
                                                          && (Day == null || x.Date.Day == Day)).ToListAsync();

                   

                    return Json(Relieves);
                }

            }

            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRelieve([Bind("Date,DoctorId,StartTime,EndTime")] Relieve relieve)
        {
            if (ModelState.IsValid)
            {
                db.Relieves.Add(relieve);
                await db.SaveChangesAsync();

                return Json(data: relieve.Id);
            }

            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRelieve(Relieve relieve)
        {
            if (ModelState.IsValid)
            {
                db.Relieves.Update(relieve);
                await db.SaveChangesAsync();
                return Json(data: true);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRelieve(int? id)
        {

            if (id != null)
            {
                Relieve relieve = await db.Relieves.FindAsync(id);


                if (relieve != null)
                {
                    db.Entry(relieve).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                    return Json(data: true);
                }
            }

            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRelieveTime([Bind("Description,StartTime,EndTime")] RelieveTime relieveTime)
        {
            if (ModelState.IsValid)
            { 
                db.RelieveTimes.Add(relieveTime);
                await db.SaveChangesAsync();

                return Json(data:relieveTime.Id);
            }

            return NotFound();
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRelieveTime(RelieveTime relieveTime)
        {
            if (ModelState.IsValid)
            {
                db.RelieveTimes.Update(relieveTime);
                await db.SaveChangesAsync();
                return Json(data: true);
            }

            return NotFound();

        }


        public async Task<IActionResult> RelieveTimes(int? id)
        {
          
            if (id != null)
            {
                RelieveTime relieveTime = await db.RelieveTimes.FindAsync(id);

                if (relieveTime != null)
                    return Json(relieveTime);
            }
            else
            {
                return Json(await db.RelieveTimes.ToListAsync());
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRelieveTime(int? id)
        {

            if (id != null)
            {
                RelieveTime relieveTime = await db.RelieveTimes.FindAsync(id);


                if (relieveTime != null)
                {
                    db.Entry(relieveTime).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                    return Json(data:true);
                }
            }
     
            return NotFound();
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
        public async Task<IActionResult> CreateStreet()
        {

            ViewBag.Regions = await db.Regions.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStreet([Bind("Name,Addresses,RegionId")] Street street)
        {

            if (ModelState.IsValid)
            {
                street.Addresses = street.Addresses.ToUpper();
                db.Add(street);
                await db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> EditStreet(Street street)
        {
            db.Streets.Update(street);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> DeleteStreet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            bool exist = await db.Streets.AnyAsync(x => x.Id == id);

            if (exist)
            {
                Street street = new Street { Id = (int)id };
                db.Entry(street).State = EntityState.Deleted;

                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Regions));

        }

        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> isStreetExist()
        {

            IQueryCollection args = HttpContext.Request.Query;

            if (args.Count != 2) return Json(data: false);

            String Name = "";
            String Addresses = "";

            foreach(String key in args.Keys)
            {
                if (key.Contains("Name"))
                    Name = args[key];

                if (key.Contains("Addresses"))
                    Addresses = args[key];
            }

            List<Street> StreetsMatched = await db.Streets.Where(x => x.Name.Equals(Name)).ToListAsync();

            if (StreetsMatched.Count == 0)
                return Json(data: true);

            String[] AddressesArr = Addresses.Split(new Char[]{','});

            foreach(String Address in AddressesArr)
                foreach(Street street in StreetsMatched)
                    if (street.Addresses.Contains(Address))
                        return Json("—уществует адрес уже прив€занный к участку");

            return Json(data: true);                

        }


        //for CreatePatient
        [AcceptVerbs("Get", "Post")]
        public JsonResult isStreetChain(String StreetName,String HouseNumber)
        {
            if ((StreetName == null) || (HouseNumber == null)) return Json(data:false);

            Street street = db.Streets.FirstOrDefault(
                x => (x.Name.Equals(StreetName) 
                && x.Addresses.Contains(HouseNumber)
                ) );

            if (street == null)
            {
                return Json(data: "”лица или номер дома не закреплены ни за одним участком");
            }
            else
            {
                HttpContext.Response.Headers.Add("Id", street.Id.ToString());
                return Json(data: true);
            }
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
        public async Task<JsonResult> isSpecialityExist(String Name)
        {
            if (Name == null) return Json(false);

            Speciality spec = await db.Specialities.FirstOrDefaultAsync(x => x.Name.ToLower().Equals( Name.ToLower() ) );

            if (spec == null)
                return Json(true);
            else
                return Json("—пециальность с таким названием уже существует");
        
        }


        public async Task<IActionResult> EditSpeciality(Speciality spec)
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

        public async Task<JsonResult> isDiseaseExist(String Name)
        {
            if (Name == null) return Json(false);

            Disease spec = await db.Diseases.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(Name.ToLower()));

            if (spec == null)
                return Json(true);
            else
                return Json("Ѕолезнь с таким названием уже существует");

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
        public async Task<IActionResult> EditDisease(Disease disease)
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

        public async Task<JsonResult> isDrugExist(String Name)
        {
            if (Name == null) return Json(false);

            Drug spec = await db.Drugs.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(Name.ToLower()));

            if (spec == null)
                return Json(true);
            else
                return Json("Ћекарство с таким названием уже существует");

        }

        [HttpGet]
        public async Task<IActionResult> EditDrug(Drug drug)
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
