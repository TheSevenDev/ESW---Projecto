using System;
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
        private readonly CIMOB_IPS_DBContext _context;

        public ProfileController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }

        public ProfileViewModel GetAccountModelByID(int intId)
        {
            var viewModel = new ProfileViewModel { };

            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQueryStudent = "SELECT id_student, a.email, c.name, s.name, address, cc, telephone, " +
                    "n.description, credits, student_num FROM Student s, Course c, Nationality n, Account a " +
                    "WHERE s.id_account = @Id AND s.id_course = c.id_course AND s.id_nationality = n.id_nationality " +
                    "AND s.id_account = a.id_account";

                SqlCommand scmCommandStudent = new SqlCommand(strQueryStudent, scnConnection);
                scmCommandStudent.Parameters.AddWithValue("@Id", intId);
                SqlDataReader dtrReader = scmCommandStudent.ExecuteReader();

                while (dtrReader.Read())
                {
                    var modelStudent = new Student
                    {
                        IdStudent = dtrReader.GetInt64(0),
                        IdAccount = intId,
                        IdCourseNavigation = new Course { Name = dtrReader.GetString(2) },
                        Name = dtrReader.GetString(3),
                        Address = dtrReader.GetString(4),
                        Cc = dtrReader.GetInt64(5),
                        Telephone = dtrReader.GetInt64(6),
                        IdNationalityNavigation = new Nationality { Description = dtrReader.GetString(7) },
                        Credits = dtrReader.GetInt32(8),
                        StudentNum = dtrReader.GetInt64(9)
                    };

                    modelStudent.IdAccountNavigation = new Account { IdAccount = intId, Email = dtrReader.GetString(1) };
                    viewModel.Student = modelStudent;
                    viewModel.AccountType = EnumAccountType.STUDENT;

                    dtrReader.Close();

                    return viewModel;
                }

                dtrReader.Close();

                SqlCommand scmCommandTechnician = new SqlCommand("select t.id_technician, t.name, t.telephone, t.is_admin, a.email from Technician t, Account a " +
                    "where t.id_account=@Id and a.id_account = t.id_account", scnConnection);
                scmCommandTechnician.Parameters.AddWithValue("@Id", intId);
                SqlDataReader dtrReader2 = scmCommandTechnician.ExecuteReader();

                while (dtrReader2.Read())
                {
                    var modelTech = new Technician
                    {
                        IdTechnician = dtrReader2.GetInt64(0),
                        IdAccount = intId,
                        Name = dtrReader2.GetString(1),
                        Telephone = dtrReader2.GetInt64(2),
                        IsAdmin = dtrReader2.GetBoolean(3)
                    };

                    modelTech.IdAccountNavigation = new Account { IdAccount = intId, Email = dtrReader2.GetString(4) };
                    viewModel.Technician = modelTech;
                    viewModel.AccountType = EnumAccountType.TECHNICIAN;

                    dtrReader2.Close();

                    return viewModel;
                }

                dtrReader2.Close();
            }

            return null;
        }



        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(Account.GetCurrentUserID());

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

            var accountViewModel = GetAccountModelByID(Account.GetCurrentUserID());

            return View(accountViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileStudent([Bind("IdAccount, Name, Telephone,StudentNum,Address")] Student student)
        {
            if (Account.GetCurrentUserID() != student.IdAccount)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                    {
                        using (SqlCommand scmCommand = scnConnection.CreateCommand())
                        {
                            scmCommand.CommandText = "UPDATE Student SET telephone = @Telephone, student_num = @StudentNum, address = @Address" +
                                " WHERE id_account = @IdAccount";
                            scmCommand.Parameters.AddWithValue("@Telephone", student.Telephone);
                            scmCommand.Parameters.AddWithValue("@StudentNum", student.StudentNum);
                            scmCommand.Parameters.AddWithValue("@Address", student.Address);
                            scmCommand.Parameters.AddWithValue("@IdAccount", student.IdAccount);
                            scnConnection.Open();
                            scmCommand.ExecuteNonQuery();
                            scnConnection.Close();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(student.IdAccount))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        } 

        public bool TechnicianCheck()
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                SqlCommand scmCommandTechnician = new SqlCommand("select t.id_technician from Technician t, Account a " +
                    "where t.id_account=@Id and a.id_account = t.id_account", scnConnection);
                scmCommandTechnician.Parameters.AddWithValue("@Id", Account.GetCurrentUserID());
                SqlDataReader dtrReader = scmCommandTechnician.ExecuteReader();

                return dtrReader.HasRows;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileTechnician([Bind("IdAccount, Name, Telephone")] Technician technician)
        {
            if (Account.GetCurrentUserID() != technician.IdAccount)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                    {
                        using (SqlCommand scmCommand = scnConnection.CreateCommand())
                        {
                            scmCommand.CommandText = "UPDATE Technician SET telephone = @Telephone" +
                                " WHERE id_account = @IdAccount";
                            scmCommand.Parameters.AddWithValue("@Telephone", technician.Telephone);
                            scmCommand.Parameters.AddWithValue("@IdAccount", technician.IdAccount);
                            scnConnection.Open();
                            scmCommand.ExecuteNonQuery();
                            scnConnection.Close();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(technician.IdAccount))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        private bool AccountExists(long lngId)
        {
            return _context.Account.Any(e => e.IdAccount == lngId);
        }
    }
}