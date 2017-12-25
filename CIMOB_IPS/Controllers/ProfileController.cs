﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CIMOB_IPS.Controllers
{
    public class ProfileController : Controller
    {
        /*private readonly CIMOB_IPS_DBContext _context;

        public ProfileController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }
        */
        public ProfileController()
        {

        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public ProfileViewModel GetAccountModelByID(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                ProfileViewModel profileViewModel = new ProfileViewModel { };

                profileViewModel.Student = context.Student
                    .Include(a => a.IdAccountNavigation)
                    .Include(a => a.IdNationalityNavigation)
                    .Include(a => a.IdCourseNavigation)
                    .FirstOrDefault(s => s.IdAccount == intId);

                profileViewModel.Technician = context.Technician
                    .Include(a => a.IdAccountNavigation)
                    .FirstOrDefault(s => s.IdAccount == intId);

                if (profileViewModel.Student != null)
                {
                    profileViewModel.AccountType = EnumAccountType.STUDENT;

                    return profileViewModel;
                }
                else if (profileViewModel.Technician != null)
                {
                    profileViewModel.AccountType = EnumAccountType.TECHNICIAN;

                    return profileViewModel;
                }

                return null;
            }
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            ViewData["edit-profile-display"] = "block";
            //add verificação
            return View(accountViewModel);
        }

        public IActionResult Get(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(id);
            ViewData["edit-profile-display"] = "none";
            
            if(accountViewModel == null)
                return RedirectToAction("Index", "Home");

            return View("Index", accountViewModel);
        }

        public IActionResult Edit()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            return View(accountViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileStudent([Bind("IdAccount, Name, Telephone,StudentNum,Address")] Student student)
        {
            if (GetCurrentUserID() != student.IdAccount)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                    {
                        Student newStudent = await context.Student.SingleOrDefaultAsync(s => s.IdAccount == student.IdAccount);

                        if (newStudent == null)
                            return NotFound();

                        newStudent.Telephone = student.Telephone;
                        newStudent.Address = student.Address;
                        newStudent.StudentNum = student.StudentNum;

                        context.Update(newStudent);

                        await context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileTechnician([Bind("IdAccount, Name, Telephone")] Technician technician)
        {
            if (GetCurrentUserID() != technician.IdAccount)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                    {
                        Technician newTechnician = await context.Technician.SingleOrDefaultAsync(t => t.IdAccount == technician.IdAccount);

                        if (newTechnician == null)
                            return NotFound();

                        newTechnician.Telephone = technician.Telephone;

                        context.Update(newTechnician);
                        await context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }
    }
}