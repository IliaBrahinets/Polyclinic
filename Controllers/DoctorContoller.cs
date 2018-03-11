using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polyclinic.Data;
using Polyclinic.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Polyclinic.Controllers
{
    public class DoctorController : SharedController
    {
        public DoctorController(PolyclinicContext context,ILogger<DoctorController> logger):base(context, logger)
        {
        }

        //work with a session
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ViewBag.DoctorId = HttpContext.Session.GetInt32("DoctorId");
        }

        public override Task<IActionResult> DoctorScheduleView(int? id)
        {
            id = id ?? HttpContext.Session.GetInt32("DoctorId");

            return base.DoctorScheduleView(id);
        }

        public IActionResult Index()
        {

            return RedirectToAction("DoctorScheduleView");
        }

        public IActionResult WriteCheckUpInfo(int? RecordId)
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WriteCheckUpInfo([Bind("Medicines", "Diagnosis")]DoctorVisit doctorVisit,int? RecordId,bool? NonAppeared)
        {

            //doctor visit's datetime is chained to patientrecord's datetime
            PatientRecord patientRecord = db.PatientRecords.Find(RecordId);

            if (patientRecord == null) return NotFound();

            //init for upd statistics
            AttendenceStatistics attendence = db.AttendenceStatistics.FirstOrDefault(x => x.Date == patientRecord.DateTime.Date);

            if (attendence == null)
            {
                attendence = new AttendenceStatistics
                {
                    Date = patientRecord.DateTime.Date,
                    DoctorId = patientRecord.DoctorId
                };

                db.AttendenceStatistics.Add(attendence);

            }

            //Appeared
            if (ModelState.IsValid)
            {

                doctorVisit.DateTime = patientRecord.DateTime;
                doctorVisit.DoctorId = patientRecord.DoctorId;
                doctorVisit.PatientId = (int)patientRecord.PatientId;

                //statistics
                attendence.Appeared += 1;
                //

                db.DoctorVisits.Add(doctorVisit);

                //

            }
            else if (NonAppeared == true)
            {

                //statistics
                attendence.NonAppeared += 1;

            }
                else return NotFound();


            //if record is the last 
            Relieve relieve = db.Relieves.FirstOrDefault(x => (x.DoctorId == patientRecord.DoctorId
                                                          && x.StartTime <= patientRecord.DateTime
                                                          && patientRecord.DateTime < x.EndTime));

            int RecordsCount = db.PatientRecords.Count(x => (relieve.StartTime <= x.DateTime
                                                                && x.DateTime < relieve.EndTime));

            if (RecordsCount == 1)
                db.Relieves.Remove(relieve);
            //

            db.PatientRecords.Remove(patientRecord);

            db.SaveChanges();

            return Json(data: true);

        }

    }
}
