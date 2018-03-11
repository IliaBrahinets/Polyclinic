using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Polyclinic.Data;
using Polyclinic.Helpers;
using Polyclinic.Models;
using Microsoft.Extensions.Logging;

namespace Polyclinic.Controllers
{
    public abstract class SharedController : Controller
    {
        protected readonly PolyclinicContext db;
        protected readonly ILogger _logger;

        public SharedController(PolyclinicContext context, ILogger logger)
        {
            db = context;
            _logger = logger;
        }

        public async Task<IActionResult> PatientCard(int? Id,int? SpecialityId,string DateRange)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Patient patient = await db.Patients.FindAsync(Id);

            if (patient == null)
            {
                return NotFound();
            }

            await db.Entry(patient).Collection(x => x.DoctorVisits).LoadAsync();
            //by a date
            if (!String.IsNullOrEmpty(DateRange))
            {
                var minDate = DateTime.ParseExact(DateRange.Substring(0, 10), "dd.MM.yyyy", null);
                var maxDate = DateTime.ParseExact(DateRange.Substring(13, 10), "dd.MM.yyyy", null);

                patient.DoctorVisits = patient.DoctorVisits.Where(s => ((s.DateTime >= minDate) && (s.DateTime <= maxDate))).ToList();
            }
            //
            foreach (DoctorVisit doctorVisit in patient.DoctorVisits)
                await db.Entry(doctorVisit).Reference(x => x.Doctor).LoadAsync();

            //by a speciality
            if (SpecialityId != null)
                if (db.Specialities.Any(x => x.Id == SpecialityId))
                    patient.DoctorVisits = patient.DoctorVisits.Where(x => x.Doctor.SpecialityId == SpecialityId).ToList();
            //

            foreach (DoctorVisit doctorVisit in patient.DoctorVisits)
                await db.Entry(doctorVisit.Doctor).Reference(x => x.Speciality).LoadAsync();

            ViewBag.Specialities = await db.Specialities.ToListAsync();

            patient.DoctorVisits = patient.DoctorVisits.OrderBy(x => x.DateTime).ToList();

            return View(patient);
        }


        public virtual async Task<IActionResult> DoctorScheduleView(int? id)
        {

            if (id != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(id);

                if (doctor != null)
                {
                    await db.Entry(doctor).Reference(x => x.Speciality).LoadAsync();

                    ViewBag.PatientId = HttpContext.Session.GetInt32("PatientId");

                    return View(doctor);
                }
            }

            return NotFound();
        }
        public async Task<IActionResult> Relieves(int? Year, int? Month, int? Day, int? DoctorId, bool? WithRecordsCount)
        {
            if (DoctorId != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(DoctorId);

                if (doctor != null)
                {

                    IQueryable<Relieve> q = db.Entry(doctor).Collection(x => x.Schedule).Query();

                    Relieve[] Relieves = await q.Where(x => (Year == null || x.Date.Year == Year)
                                                          && (Month == null || x.Date.Month == Month)
                                                          && (Day == null || x.Date.Day == Day)).ToArrayAsync();

                    if (WithRecordsCount == true)
                    {
                        await db.Entry(doctor).Collection(x => x.PatientRecords).LoadAsync();

                        JArray toSend = new JArray();

                        foreach (Relieve Relieve in Relieves)
                        {

                            DateTime StartDateTime = Relieve.Date.Add(Relieve.StartTime.TimeOfDay);
                            DateTime EndDateTime = Relieve.Date.Add(Relieve.EndTime.TimeOfDay);


                            int All = doctor.PatientRecords.Where(
                                x => (x.DateTime >= StartDateTime && x.DateTime < EndDateTime)).Count();


                            int Busy = doctor.PatientRecords.Where(
                                x => (x.DateTime >= StartDateTime && x.DateTime < EndDateTime
                                      && x.PatientId != null)).Count();

                            JObject JSONRelieve = JObject.FromObject(Relieve);

                            JSONRelieve["Count"] = Busy + "/" + All;

                            toSend.Add(JSONRelieve);
                        }

                        return Json(toSend);

                    }
                    else
                    {

                        return Json(Relieves);
                    }

                }

            }

            return NotFound();

        }
        public async Task<IActionResult> PatientRecords(int? DoctorId, DateTime StartDateTime, DateTime EndDateTime)
        {
            if (DoctorId != null)
            {
                List<PatientRecord> PatientRecords = await db.PatientRecords.Where(x => (x.DoctorId == DoctorId
                                                                                      && x.DateTime >= StartDateTime
                                                                                      && x.DateTime < EndDateTime))
                                                    .OrderBy(x => x.DateTime).Include(x => x.Patient).ToListAsync();

                JArray toSend = new JArray();

                foreach (PatientRecord PatientRecord in PatientRecords)
                {
                    JObject JSONRecord = JObject.FromObject(PatientRecord);

                    if (PatientRecord.Patient != null)
                        JSONRecord["PatientShortName"] = PatientRecord.Patient.GetShortName();

                    toSend.Add(JSONRecord);
                }

                return Json(toSend);
            }

            return NotFound();

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
        public async Task<IActionResult> EditDisease(Disease disease)
        {
            if (ModelState.IsValid)
            {
                db.Diseases.Update(disease);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DiseasesDirectory));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteDisease(int? id)
        {
            await DeleteEntity<Disease>(id);

            return RedirectToAction(nameof(DiseasesDirectory));

        }


        public IActionResult DrugsDirectory(String q)
        {
            if (q != null)
            {
                q = q.ToLower();

                return View(db.Drugs.Where(x => (x.Name.ToLower().Contains(q) || x.Description.ToLower().Contains(q))));
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
            if (ModelState.IsValid)
            {
                db.Drugs.Update(drug);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DrugsDirectory));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteDrug(int? id)
        {
            await DeleteEntity<Drug>(id);

            return RedirectToAction(nameof(DrugsDirectory));

        }
        public async Task<bool> DeleteEntity<T>(int? Id) where T : class
        {
            if (Id == null)
            {
                return false;
            }

            T entity = await db.FindAsync<T>(Id);


            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error deleting an Entity");
                    return false;
                }

                return true;
            }

            return false;

        }

        public String GetCurrentDate()
        {
            //as an ISO date
            return DateTime.UtcNow.ToString("s");
        }
        public object PageMessage(string type, string title, string message) {

            dynamic obj = new System.Dynamic.ExpandoObject();

            obj.type = type;
            obj.message = message;
            obj.title = title;

            return obj;

        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
