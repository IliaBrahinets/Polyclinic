using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polyclinic.Data;
using Polyclinic.Models;
using System.Diagnostics;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

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
            db.Entry(patient).Reference(x=>x.Street).Load();
            ViewBag.Specialities = db.Specialities.ToList();

            return View(patient);
        }
     

        public async Task<IActionResult> Doctors(string q,int w,string e)
        {
            ViewBag.Specialities = await db.Specialities.ToListAsync();
            ViewBag.Regions = await db.Regions.ToListAsync();
            var Doctors = from m in db.Doctors select m;
            if (!String.IsNullOrEmpty(e))
            {
                Doctors = Doctors.Where(s => s.Surname.ToLower().Contains(e));
            }
            if (!String.IsNullOrEmpty(q))
            {
                Doctors = Doctors.Where(s => s.Speciality.Name.ToLower().Contains(q));
            }
            if (w!=0)
            {
                Doctors = Doctors.Where(x => x.Region.Id==w);
            }
            return View(Doctors);
            

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
        public async Task<IActionResult> Patients(string birthdate,string surname,int region,bool? sex)
         {
            ViewBag.Regions = await db.Regions.ToListAsync();
            
            var Patients = from m in db.Patients select m;

            if (!String.IsNullOrEmpty(birthdate))
            {
                var minDate = DateTime.ParseExact(birthdate.Substring(0, 10), "dd.MM.yyyy", null);
                var maxDate = DateTime.ParseExact(birthdate.Substring(13, 10), "dd.MM.yyyy", null);

                Patients = Patients.Where(s => s.BirthDate.CompareTo(minDate) >= 0 && s.BirthDate.CompareTo(maxDate) <= 0);
            }
            foreach (Patient patient in db.Patients)
            {
              
                await db.Entry(patient).Reference(x => x.Street).LoadAsync();
            }
            if (!String.IsNullOrEmpty(surname))
            {
                Patients = Patients.Where(s => s.Surname.ToLower().Contains(surname));
            }
            if (region!=0)
            {
                Patients = Patients.Where(s => s.Street.RegionId==region);
            }
            if (sex==false || sex == true)
            {
                Patients = Patients.Where(s => s.Sex == sex);
            }



                return View(Patients);
        }
       
        public async Task<IActionResult> ExportToWord(PatientRecord patientRecord)
        {
            if (patientRecord.Id != 0)
            {
                
                MemoryStream mem = new MemoryStream();
                using (WordprocessingDocument wordDoc =
                    WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    wordDoc.AddMainDocumentPart();
                    // instantiate the members of the hierarchy
                    Document doc = new Document();
                    Body body = new Body();
                    Paragraph para = new Paragraph();
                    Run run = new Run();
                    Text text = new Text() {Text = patientRecord.Doctor.Id.ToString()};

                    // put the hierarchy together
                    run.Append(text);
                    para.Append(run);
                    body.Append(para);
                    doc.Append(body);
                    wordDoc.MainDocumentPart.Document = doc;
                    wordDoc.Close();
                }
                return File(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "talon.docx");
            }
            return View();
        }
        public async Task<IActionResult> ViewPatientRecord(Patient patientId)
        {
            
            ViewBag.Doctors = await db.Doctors.ToListAsync();
            ViewBag.Patients = await db.Patients.ToListAsync();

            ViewBag.Specialities = await db.Specialities.ToListAsync();
            var PatientRecords = from m in db.PatientRecords select m;

            if (patientId.Id != 0)
            {
                PatientRecords = PatientRecords.Where(s => s.PatientId == patientId.Id);
            }
            return View(PatientRecords);
        }

        public async Task<IActionResult> CreatePatientRecord()
        {
            
            
            ViewBag.Doctors = await db.Doctors.ToListAsync();
            ViewBag.Patients = await db.Patients.ToListAsync();
            ViewBag.Specialities = await db.Specialities.ToListAsync();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatientRecord([Bind("DateTime,PatientId,DoctorId")] PatientRecord patientRecord)
        {
            if (ModelState.IsValid)
            {
                
                db.PatientRecords.Add(patientRecord);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(ViewPatientRecord));
            }
            return View(patientRecord);
        }
        [HttpGet]
        public async Task<IActionResult> DeletePatientRecord(int? id)
        {
            if (id != null)
            {
                PatientRecord patientRecord = new PatientRecord { Id = id.Value };
                db.Entry(patientRecord).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(ViewPatientRecord));
            }
            return NotFound();
        }
        
        public async Task<IActionResult> CreatePatient()
        { 
        ViewBag.Streets = await db.Streets.ToListAsync();
        return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("Name,Surname,Lastname,BirthDate,StreetId,Sex,Housenumber")] Patient patient)
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
                db.Add(street);
                await db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> EditStreet([Bind("Name,Addresses,RegionId")]Street street)
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
