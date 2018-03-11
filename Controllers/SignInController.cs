using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Polyclinic.Helpers;
using Polyclinic.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Polyclinic.Controllers
{
    public class SignInController:Controller
    {
        protected readonly PolyclinicContext db;

        public SignInController(PolyclinicContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Doctors.ToListAsync());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? Id)
        {
            if(Id != null)
            {
                Doctor doctor = await db.Doctors.FindAsync(Id);

                if(doctor != null)
                {
                    HttpContext.Session.SetInt32("DoctorId", (int)Id);

                    return RedirectToAction("DoctorScheduleView", "Doctor");
                }
            }

            return NotFound();

        }
    }
}
