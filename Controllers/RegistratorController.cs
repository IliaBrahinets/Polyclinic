using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polyclinic.Data;
using Polyclinic.Models;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using OpenXmlPowerTools;
using System.Xml.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Polyclinic.Controllers
{
    public class RegistratorController : SharedController
    {
        public RegistratorController(PolyclinicContext context, ILogger<RegistratorController> logger) : base(context, logger)
        {

        }

        public async Task<IActionResult> Doctors(string Surname, string Speciality, int? RegionId, int? PatientId)
        {
            ViewBag.Specialities = await db.Specialities.ToListAsync();
            ViewBag.Regions = await db.Regions.ToListAsync();

            var Doctors = from m in db.Doctors select m;

            if (!String.IsNullOrEmpty(Surname))
            {
                Doctors = Doctors.Where(x => x.Surname.Contains(Surname.ToLower()));
            }

            if (!String.IsNullOrEmpty(Speciality))
            {
                Doctors = Doctors.Where(x => x.Speciality.Name.ToLower().Contains(Speciality));
            }

            //
            if (PatientId != null)
            {
                HttpContext.Session.SetInt32("PatientId", (int)PatientId);
            }
            else
            {
                PatientId = HttpContext.Session.GetInt32("PatientId");
            }

            ViewBag.PatientId = PatientId;


            if (PatientId != null)
            {
                Patient patient = db.Patients.Find(ViewBag.PatientId);

                if (patient.RegionId != null)
                {
                    if (RegionId == PatientId)
                        Doctors = Doctors.Where(x => x.RegionId == patient.RegionId);
                    else
                        if (RegionId == null)
                        Doctors = Doctors.Where(x => (x.RegionId == null || x.RegionId == patient.RegionId));
                    else
                        return View(new List<Doctor>());
                }
                else
                {
                    if (RegionId != null)
                        Doctors = Doctors.Where(x => x.RegionId == RegionId);

                }
                return View(Doctors);
            }

            if (RegionId != null)
                Doctors = Doctors.Where(x => x.RegionId == RegionId);

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

            return Json(data: "Такой доктор существует");


        }
        public async Task<JsonResult> IsCabinetAvaliable(int? Id, int? ChainedCabinet)
        {
            if (ChainedCabinet == null)
                return Json(data: true);

            Doctor TryDoctor = await db.Doctors.FirstOrDefaultAsync(x =>
                                        (x.ChainedCabinet == ChainedCabinet)
                                        && (x.Id != Id));

            if (TryDoctor == null)
                return Json(data: true);

            return Json(data: "Кабинет занят");
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
            await DeleteEntity<Doctor>(id);

            return RedirectToAction(nameof(Doctors));
        }

        public async Task<IActionResult> Patients(string Birthdate, string Surname, int? RegionId, bool? Sex, int? RecordId)
        {
            var Patients = from Patient in db.Patients select Patient;

            if (!String.IsNullOrEmpty(Birthdate))
            {
                var minDate = DateTime.ParseExact(Birthdate.Substring(0, 10), "dd.MM.yyyy", null);
                var maxDate = DateTime.ParseExact(Birthdate.Substring(13, 10), "dd.MM.yyyy", null);

                Patients = Patients.Where(s => s.BirthDate.CompareTo(minDate) >= 0 && s.BirthDate.CompareTo(maxDate) <= 0);
            }

            if (!String.IsNullOrEmpty(Surname))
            {
                Patients = Patients.Where(s => s.Surname.ToLower().Contains(Surname));
            }

            if (Sex != null)
            {
                Patients = Patients.Where(s => s.Sex == Sex);
            }

            ViewBag.Regions = await db.Regions.ToListAsync();

            //RecordAction
            if (RecordId != null)
                HttpContext.Session.SetInt32("RecordId", (int)RecordId);
            else
                RecordId = HttpContext.Session.GetInt32("RecordId");


            ViewBag.RecordId = RecordId;

            //RegionId filter
            int? filterId = RegionId;

            if (RecordId != null)
            {
                PatientRecord patientRecord = db.PatientRecords.Find(RecordId);

                int? DoctorsRegionId = (from Doctor in db.Doctors
                                        where Doctor.Id == patientRecord.DoctorId
                                        select Doctor.RegionId).First();

                filterId = DoctorsRegionId ?? filterId;

            }

            if (filterId != null)
                Patients = Patients.Where(x => x.RegionId == filterId);


            return View(Patients);
        }
        public IActionResult CreatePatient()
        {

            return View("EditPatient");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("Name,Surname,Lastname,BirthDate,Sex,RegionId,StreetName,HouseNumber")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                //try assign a region
                Street street = await db.Streets.FirstOrDefaultAsync(x => x.Name.Equals(patient.StreetName) && x.Addresses.Contains(patient.HouseNumber));

                if (street != null)
                    patient.RegionId = street.RegionId;

                db.Patients.Add(patient);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Patients));
            }
            return View(patient);
        }
        [HttpPost]
        public async Task<JsonResult> isPatientNotExist([Bind("Name,Surname,Lastname,BirthDate,Sex,RegionId,StreetName,HouseNumber")]Patient patient)
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
                      && x.RegionId.Equals(patient.RegionId)
                      && x.StreetName.Equals(patient.StreetName)
                      && x.HouseNumber.Equals(patient.HouseNumber)));

            if (TryPatient == null)
                return Json(data: true);

            return Json(data: "Такой пациент существует");


        }
        public async Task<IActionResult> EditPatient(int? id)
        {
            if (id != null)
            {
                Patient patient = await db.Patients.FindAsync(id);

                if (patient == null)
                    return RedirectToAction(nameof(Patients));

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

                Street StreetMatched =
                        await db.Streets.FirstOrDefaultAsync(x => (x.Name.Equals(patient.StreetName)
                                                                && x.Addresses.Contains(patient.HouseNumber)));

                if (StreetMatched != null)
                    patient.RegionId = StreetMatched.RegionId;


                db.Patients.Update(patient);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Patients));
        }
        [HttpGet]
        public async Task<IActionResult> DeletePatient(int? id)
        {
            await DeleteEntity<Patient>(id);

            return RedirectToAction(nameof(Patients));
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

        public async Task<IActionResult> AttendenceStatistics(int? Year, int? Month, int? Day, int? DoctorId)
        {
            if (DoctorId != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(DoctorId);

                if (doctor != null)
                {

                    IQueryable<AttendenceStatistics> q = db.AttendenceStatistics.Where(x => x.DoctorId == DoctorId);

                    AttendenceStatistics[] attendenceStatistics = await q.Where(x => (Year == null || x.Date.Year == Year)
                                                          && (Month == null || x.Date.Month == Month)
                                                          && (Day == null || x.Date.Day == Day)).ToArrayAsync();

                    return Json(attendenceStatistics);
                }

            }

            return NotFound();

        }

        public async Task<IActionResult> PatientsRecordsView(int? Id)
        {
            if (Id == null)
                return NotFound();

            if (!db.Patients.Any(x => x.Id == Id))
                return NotFound();

            return View(await db.PatientRecords.Where(x => x.PatientId == Id)
                                                    .Include(x => x.Patient)
                                                    .Include(x => x.Doctor)
                                                    .Include(x => x.Doctor.Speciality).ToListAsync());
        }
        public async Task<IActionResult> CreatePatientRecord(int? RecordId, int? PatientId)
        {
            if (RecordId != null && PatientId != null)
            {
                PatientRecord patientRecord = await db.PatientRecords.FindAsync(RecordId);

                HttpContext.Session.Remove("RecordId");
                HttpContext.Session.Remove("PatientId");

                if (patientRecord == null)
                {
                    return NotFound();
                }

                //check whether patient have a record on this time or have not
                bool isExistRecord = db.PatientRecords.Any(x => (x.PatientId == PatientId && patientRecord.DateTime == x.DateTime));
                if (isExistRecord)
                {
                    ViewData["PageMessage"] = PageMessage("danger", "Ошибка!", "Пациент уже имеет запись на это время!");
                    return View("EmptyPage");
                }

                //
                if (patientRecord.PatientId == null)
                {

                    patientRecord.PatientId = PatientId;

                    db.PatientRecords.Update(patientRecord);
                    await db.SaveChangesAsync();

                    return View("TalonView", patientRecord);
                }

                ViewData["PageMessage"] = PageMessage("danger", "Ошибка!", "На это время записан пациент!");
            }

            return View("EmptyPage");
        }

        public async Task<ActionResult> DeleteOutOfDatePlacesForRecords()
        {
            List<Relieve> OutOfDate = await db.Relieves.Where(x => (x.Date < DateTime.UtcNow)).ToListAsync();

            foreach (Relieve Relieve in OutOfDate)
            {
                bool NotConfirmedRecordsExist = await db.PatientRecords.AnyAsync(x => (x.PatientId != null
                                                                                && x.DoctorId == Relieve.DoctorId
                                                                                && x.DateTime >= Relieve.StartTime
                                                                                && x.DateTime < Relieve.EndTime));

                if (!NotConfirmedRecordsExist)
                {
                    db.Relieves.Remove(Relieve);
                }

                db.PatientRecords.RemoveRange(
                    db.PatientRecords.Where(x => (x.PatientId == null
                                                && x.DoctorId == Relieve.DoctorId
                                                && x.DateTime >= Relieve.StartTime
                                                && x.DateTime < Relieve.EndTime)));


            }

            await db.SaveChangesAsync();

            return Ok();

        }
        public IActionResult TalonView(int? PatientRecordId)
        {
            if (PatientRecordId == null)
                return NotFound();

            PatientRecord patientRecord = db.PatientRecords.Find(PatientRecordId);

            if (patientRecord == null)
                return NotFound();

            return View(patientRecord);
        }
        //return path to a word View of Record,dont recreate a Word file if already exists
        private async Task<string> CreatePatientRecordAsWord(int? id)
        {

            if (id != null)
            {
                PatientRecord patientRecord = db.PatientRecords.Find(id);
                if (patientRecord == null)
                {
                    return null;
                }

                string template = "WordTemplates/template.docx";
                string path = $"WordTemplates/tmp.docx";

                //if (System.IO.File.Exists(path))
                //  return path;

                await db.Entry(patientRecord).Reference(x => x.Doctor).LoadAsync();
                await db.Entry(patientRecord.Doctor).Reference(x => x.Speciality).LoadAsync();
                await db.Entry(patientRecord).Reference(x => x.Patient).LoadAsync();



                System.IO.File.Copy(template, path, true);

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(path, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDocument.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }
                    Regex regPatient = new Regex("patient");
                    Regex regDoctor = new Regex("doctor");
                    Regex regSpeciality = new Regex("speciality");
                    Regex regCabinet = new Regex("ChainedCabinet");
                    Regex regDate = new Regex("Date");
                    Regex regTime = new Regex("Time");

                    docText = regPatient.Replace(docText, patientRecord.Patient.Surname + ' ' + patientRecord.Patient.Name + ' ' + patientRecord.Patient.Lastname);
                    docText = regSpeciality.Replace(docText, patientRecord.Doctor.Speciality.Name);
                    docText = regDoctor.Replace(docText, patientRecord.Doctor.Surname + ' ' + patientRecord.Doctor.Name + ' ' + patientRecord.Doctor.Lastname);

                    docText = regCabinet.Replace(docText, patientRecord.Doctor.ChainedCabinet.ToString());
                    docText = regTime.Replace(docText, patientRecord.DateTime.ToShortTimeString().ToString());
                    docText = regDate.Replace(docText, patientRecord.DateTime.ToShortDateString().ToString());

                    byte[] byteArray = Encoding.UTF8.GetBytes(docText);
                    MemoryStream stream = new MemoryStream(byteArray);
                    wordDocument.MainDocumentPart.FeedData(stream);

                }

                return path;

            }

            return null;
        }
        public async Task<IActionResult> GetPatientRecordAsWord(int? id)
        {

            string path = await CreatePatientRecordAsWord(id);

            if (path != null)
            {
                MemoryStream mem = new MemoryStream(await System.IO.File.ReadAllBytesAsync(path));

                return File(mem, "application/octet-stream", "talon.docx");
            }

            return NotFound();
        }
        public async Task<IActionResult> GetPatientRecordAsHtml(int? id)
        {
            string path = await CreatePatientRecordAsWord(id);

            if (path == null)
                return NotFound();

            byte[] AsDocByte = System.IO.File.ReadAllBytes(path);

            string recordAsHtmlPath = $"WordTemplates/tmp.html";

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(AsDocByte, 0, AsDocByte.Length);
                using (WordprocessingDocument doc =
                    WordprocessingDocument.Open(memoryStream, true))
                {
                    HtmlConverterSettings settings = new HtmlConverterSettings()
                    {
                        PageTitle = "Talon"
                    };
                    XElement html = HtmlConverter.ConvertToHtml(doc, settings);


                    System.IO.File.WriteAllText(recordAsHtmlPath, html.ToStringNewLineOnAttributes());
                }
            }

            MemoryStream mem = new MemoryStream(System.IO.File.ReadAllBytes(recordAsHtmlPath));

            return Ok(mem);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatientRecord(int? Id)
        {
            if (Id == null)
                return NotFound();

            PatientRecord patientRecord = await db.PatientRecords.FindAsync(Id);

            if (patientRecord != null && patientRecord.PatientId != null)
            {
                patientRecord.PatientId = null;
                db.PatientRecords.Update(patientRecord);
                await db.SaveChangesAsync();

                return Json(data: true);

            }

            return NotFound();

        }

        //it's just a temporary method
        [HttpGet]
        public async Task<IActionResult> TmpDeletePatientRecord(int? Id)
        {
            if (Id == null)
                return NotFound();

            PatientRecord patientRecord = await db.PatientRecords.FindAsync(Id);

            if (patientRecord != null && patientRecord.PatientId != null)
            {
                int? currPatientId = patientRecord.PatientId;

                patientRecord.PatientId = null;
                db.PatientRecords.Update(patientRecord);
                await db.SaveChangesAsync();


                return RedirectToAction(nameof(PatientsRecordsView), new { Id = currPatientId });
            }

            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRelieve([Bind("Date,DoctorId,StartTime,EndTime")] Relieve relieve)
        {
            if (ModelState.IsValid)
            {

                //since Start[End]Time is stored as DateTime, but it's only a time information 
                //for comfort we set a Date Component as relieve's date
                relieve.StartTime = relieve.Date.Add(relieve.StartTime.TimeOfDay);
                relieve.EndTime = relieve.Date.Add(relieve.EndTime.TimeOfDay);

                bool isIntersectWithExistRel = await db.Relieves.AnyAsync(x => (x.DoctorId == relieve.DoctorId
                                                                        && ((x.StartTime >= relieve.StartTime && x.StartTime < relieve.EndTime)
                                                                              || (x.EndTime > relieve.StartTime && x.EndTime <= relieve.EndTime)
                                                                              || (x.StartTime < relieve.StartTime && x.EndTime > relieve.EndTime))));

                if (isIntersectWithExistRel)
                    return NotFound("Пересекается с существующей!");

                db.Relieves.Add(relieve);

                //now we must add PatientRecord
                Doctor doctor = await db.Doctors.FindAsync(relieve.DoctorId);

                await db.Entry(doctor).Reference(x => x.Speciality).LoadAsync();


                //initially it equals to StartTime
                DateTime RecordTime = relieve.StartTime;

                DateTime EndTime = relieve.EndTime;

                while (DateTime.Compare(RecordTime, EndTime) < 0)
                {
                    PatientRecord patientRecord = new PatientRecord { DateTime = RecordTime, DoctorId = doctor.Id };

                    db.PatientRecords.Add(patientRecord);

                    RecordTime = RecordTime.AddMinutes(doctor.Speciality.CheckUpTime);
                }


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
            Relieve relieve = await db.Relieves.FindAsync(id);

            if (relieve != null)
            {
                bool isAnyPatientRecords = await db.PatientRecords.AnyAsync(x => (x.PatientId != null
                                                                                  && x.DoctorId == relieve.DoctorId
                                                                                  && relieve.StartTime <= x.DateTime
                                                                                  && x.DateTime <= relieve.EndTime));

                if (isAnyPatientRecords)
                    return NotFound("Есть записи пациентов!");

                db.PatientRecords.RemoveRange(await db.PatientRecords.Where(x => (x.PatientId == null
                                                                               && x.DoctorId == relieve.DoctorId
                                                                               && relieve.StartTime <= x.DateTime
                                                                               && x.DateTime <= relieve.EndTime)).ToArrayAsync());

                //SaveChanges will be called in DeleteEntity
            }
            else return NotFound();

            bool isAllRight = await DeleteEntity<Relieve>(id);

            if (isAllRight)
            {
                return Json(data: true);
            }
            else
            {
                return NotFound();
            }

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
        public async Task<IActionResult> CreateRelieveTime([Bind("Description,StartTime,EndTime")] RelieveTime relieveTime)
        {
            if (ModelState.IsValid)
            {
                db.RelieveTimes.Add(relieveTime);
                await db.SaveChangesAsync();

                return Json(data: relieveTime.Id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRelieveTime(int? id)
        {

            bool isAllRight = await DeleteEntity<RelieveTime>(id);

            if (isAllRight)
            {
                return Json(data: true);
            }
            else
            {
                return NotFound();
            }

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
                    //check whether there are patients live on that street
                    IQueryable<Patient> patients = db.Patients.Where(x => (x.RegionId == null && Street.Name.Equals(x.StreetName) && Street.Addresses.Contains(x.HouseNumber)));
                    await patients.ForEachAsync(x => x.Region = region);

                    db.Streets.Add(Street);
                }
                db.Regions.Add(region);

                await db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Regions));
        }
        public async Task<IActionResult> DeleteRegion(int? id)
        {
            await DeleteEntity<Region>(id);

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

                //check whether there are patients live on that street
                IQueryable<Patient> patients = db.Patients.AsTracking().Where(x => (x.RegionId == null && street.Name.Equals(x.StreetName) && street.Addresses.Contains(x.HouseNumber)));
                await patients.ForEachAsync(x => x.RegionId = street.RegionId);


                db.Streets.Add(street);
                await db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Regions));
        }

        public async Task<IActionResult> EditStreet(int? Id)
        {
            Street street = await db.Streets.FindAsync(Id);

            if (street != null)
                return View(street);

            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStreet(Street street)
        {
            if (ModelState.IsValid)
            {
                Street prevStreet = await db.Streets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == street.Id);

                db.Streets.Update(street);

                IQueryable<Patient> prevChainedPateints = db.Patients.AsTracking().Where(x => (x.RegionId != null
                                                                                            && prevStreet.Name.Equals(x.StreetName)
                                                                                            && prevStreet.Addresses.Contains(x.HouseNumber)));
                await prevChainedPateints.ForEachAsync(x => x.RegionId = null);


                //check whether there are patients live on that street
                IQueryable<Patient> patients = db.Patients.AsTracking().Where(x => (x.RegionId == null
                                                                                    && street.Name.Equals(x.StreetName)
                                                                                    && street.Addresses.Contains(x.HouseNumber)));
                await patients.ForEachAsync(x => x.RegionId = street.RegionId);


                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Regions));
        }
        public async Task<IActionResult> DeleteStreet(int? id)
        {
            await DeleteEntity<Street>(id);

            return RedirectToAction(nameof(Regions));

        }

        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> isStreetExist()
        {

            IQueryCollection args = HttpContext.Request.Query;

            if (args.Count < 2) return Json(data: false);

            int Id = -1;
            String Name = "";
            String Addresses = "";

            foreach (String key in args.Keys)
            {
                if (key.Contains("Name"))
                    Name = args[key];

                if (key.Contains("Addresses"))
                    Addresses = args[key];

                if (key.Contains("Id"))
                    Int32.TryParse(args[key], out Id);
            }

            List<Street> StreetsMatched = await db.Streets.Where(x => (x.Name.Equals(Name) && x.Id != Id)).ToListAsync();

            if (StreetsMatched.Count == 0)
                return Json(data: true);

            String[] AddressesArr = Addresses.Split(new Char[] { ',' });

            foreach (String Address in AddressesArr)
                foreach (Street street in StreetsMatched)
                    if (street.Addresses.Contains(Address))
                        return Json("Существует адрес уже привязанный к участку");

            return Json(data: true);

        }
        //for CreatePatient
        [AcceptVerbs("Get", "Post")]
        public JsonResult IsStreetChain(String StreetName, String HouseNumber)
        {
            String ErrMessage = "Улица или номер дома не закреплены ни за одним участком";

            if ((StreetName == null) || (HouseNumber == null)) return Json(data: ErrMessage);

            Street street = db.Streets.FirstOrDefault(
                x => (x.Name.Equals(StreetName)
                && x.Addresses.Contains(HouseNumber)
                ));

            if (street == null)
            {
                return Json(data: ErrMessage);
            }
            else
            {
                HttpContext.Response.Headers.Add("Id", street.RegionId.ToString());
                return Json(data: true);
            }
        }


        public async Task<IActionResult> Specialities(String q)
        {

            if (q != null)
            {
                q = q.ToLower();
                return View(await db.Specialities.Where(x => (x.Name.ToLower().Contains(q))).ToListAsync());
            }
            else
            {
                return View(await db.Specialities.ToListAsync());
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

            Speciality spec = await db.Specialities.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(Name.ToLower()));

            if (spec == null)
                return Json(true);
            else
                return Json("Специальность с таким названием уже существует");

        }
        public async Task<IActionResult> EditSpeciality(Speciality spec)
        {
            if (ModelState.IsValid)
            {
                db.Specialities.Update(spec);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Specialities));
        }
        public async Task<IActionResult> DeleteSpeciality(int? id)
        {
            await DeleteEntity<Speciality>(id);

            return RedirectToAction(nameof(Specialities));

        }
        public IActionResult Index()
        {
            return RedirectToAction("Patients");
        }
    }
}
