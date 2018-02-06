using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CIMOB_IPS.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        /*private readonly CIMOB_IPS_DBContext _context;

        public ProfileController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }
        */
        public ProfileController(IHostingEnvironment HostingEnvironment)
        {
            _hostingEnvironment = HostingEnvironment;
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
                    .Include(a => a.IdAddressNavigation)
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

            if (accountViewModel == null)
                return RedirectToAction("Index", "Home");

            return View("Index", accountViewModel);
        }

        public IActionResult Edit()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            if (accountViewModel.AccountType == EnumAccountType.STUDENT)
            {
                var postalCode = accountViewModel.Student.IdAddressNavigation.PostalCode;

                accountViewModel.PostalCode1 = postalCode.Substring(0, 4);
                accountViewModel.PostalCode2 = postalCode.Substring(5, 3);
            }

            return View(accountViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileStudent(ProfileViewModel model)
        {
            if (GetCurrentUserID() != model.Student.IdAccount)
                return BadRequest();

            try
            {

                using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                {

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        Console.WriteLine("====================================== HÀ FILES");
                        var ImageFile = HttpContext.Request.Form.Files[0];
                        if (ImageFile != null && ImageFile.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString();
                            var extention = Path.GetExtension(ImageFile.FileName);

                            var newFile = fileName + extention;


                            var uploadName = Path.Combine(_hostingEnvironment.WebRootPath, "images/avatars", newFile);

                            using (var fileStream = new FileStream(uploadName, FileMode.Create))
                            {
                                await ImageFile.CopyToAsync(fileStream);
                                //Carro.FicheiroFoto = "/fotografias/" + newFile;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("====================================== NÂO HÀ FILES");
                    }

                    Student newStudent = await context.Student
                    .Include(s => s.IdAddressNavigation)
                    .SingleOrDefaultAsync(s => s.IdAccount == model.Student.IdAccount);

                    if (newStudent == null)
                        return NotFound();

                    newStudent.Telephone = model.Student.Telephone;
                    newStudent.IdAddressNavigation.PostalCode = model.Student.IdAddressNavigation.PostalCode;
                    newStudent.Credits = model.Student.Credits;

                    Address studentAddress = await context.Address.SingleOrDefaultAsync(s => s.IdAddress == newStudent.IdAddress);
                    studentAddress.PostalCode = model.PostalCode1 + "-" + model.PostalCode2;
                    studentAddress.AddressDesc = model.Student.IdAddressNavigation.AddressDesc;
                    studentAddress.DoorNumber = model.Student.IdAddressNavigation.DoorNumber;
                    studentAddress.Floor = model.Student.IdAddressNavigation.Floor;

                    context.Update(studentAddress);

                    context.Update(newStudent);

                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
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


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ViewStudentProfile(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!(User.IsInRole("tecnico") || User.IsInRole("tecnico_admin")))
                return RedirectToAction("Index", "Home");

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                Student student = context.Student.Where(s => s.IdStudent == int.Parse(id))
                    .Include(s => s.IdAccountNavigation)
                    .Include(s => s.IdAddressNavigation)
                    .Include(s => s.IdCourseNavigation)
                    .Include(s => s.IdNationalityNavigation)
                    .SingleOrDefault();

                if (student == null)
                    return RedirectToAction("Index", "Home");

                return PartialView("_ViewStudentProfile", student);
            }
        }

        public async Task<int> GetCurrentStudentECTS(ClaimsPrincipal user)
        {
            var intCurrentId = int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return await context.Student.Where(s => s.IdAccount == intCurrentId).Select(s => s.Credits).SingleOrDefaultAsync();
            }
        }

    }
}