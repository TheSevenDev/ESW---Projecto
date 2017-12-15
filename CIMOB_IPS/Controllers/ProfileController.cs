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

        public ProfileViewModel GetAccountModelByID(int id)
        {
            var viewModel = new ProfileViewModel { };

            using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                sqlConnection.Open();
                string sqlQueryStringStudent = "SELECT id_student, a.email, c.name, s.name, address, cc, telephone, " +
                    "n.description, credits, student_num FROM Student s, Course c, Nationality n, Account a " +
                    "WHERE s.id_account = @Id AND s.id_course = c.id_course AND s.id_nationality = n.id_nationality " +
                    "AND s.id_account = a.id_account";

                SqlCommand commandStudent = new SqlCommand(sqlQueryStringStudent, sqlConnection);
                commandStudent.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = commandStudent.ExecuteReader();

                while (reader.Read())
                {
                    var modelStudent = new Student
                    {
                        IdStudent = reader.GetInt64(0),
                        IdAccount = id,
                        IdCourseNavigation = new Course { Name = reader.GetString(2) },
                        Name = reader.GetString(3),
                        Address = reader.GetString(4),
                        Cc = reader.GetInt64(5),
                        Telephone = reader.GetInt64(6),
                        IdNationalityNavigation = new Nationality { Description = reader.GetString(7) },
                        Credits = reader.GetInt32(8),
                        StudentNum = reader.GetInt64(9)
                    };

                    modelStudent.IdAccountNavigation = new Account { IdAccount = id, Email = reader.GetString(1) };
                    viewModel.Student = modelStudent;
                    viewModel.AccountType = EnumAccountType.STUDENT;

                    reader.Close();

                    return viewModel;
                }

                reader.Close();

                SqlCommand commandtechnician = new SqlCommand("select t.id_technician, t.name, t.telephone, t.is_admin, a.email from Technician t, Account a " +
                    "where t.id_account=@Id and a.id_account = t.id_account", sqlConnection);
                commandtechnician.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader2 = commandtechnician.ExecuteReader();

                while (reader2.Read())
                {
                    var modelTech = new Technician
                    {
                        IdTechnician = reader2.GetInt64(0),
                        IdAccount = id,
                        Name = reader2.GetString(1),
                        Telephone = reader2.GetInt64(2),
                        IsAdmin = reader2.GetBoolean(3)
                    };

                    modelTech.IdAccountNavigation = new Account { IdAccount = id, Email = reader2.GetString(4) };
                    viewModel.Technician = modelTech;
                    viewModel.AccountType = EnumAccountType.TECHNICIAN;

                    reader2.Close();

                    return viewModel;
                }

                reader2.Close();
            }

            return null;
        }

        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public IActionResult Index()
        {

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            ViewData["edit-profile-display"] = "block";
            //add verificação
            return View(accountViewModel);
        }

        public IActionResult Get(int id)
        {
            var accountViewModel = GetAccountModelByID(id);
            ViewData["edit-profile-display"] = "none";
            
            if(accountViewModel == null)
                return RedirectToAction("Index", "Home");


            return View("Index", accountViewModel);
        }

        public IActionResult Edit()
        {
            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            //add verificação
            return View(accountViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileStudent([Bind("IdAccount, Name, Telephone,StudentNum,Address")] Student student)
        {

            if (GetCurrentUserID() != student.IdAccount)
            {
                return BadRequest();
            }

            //não está a apresentar erros na pagina
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                    {
                        //create await  

                        using (SqlCommand command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = "UPDATE Student SET telephone = @Telephone, student_num = @StudentNum, address = @Address" +
                                " WHERE id_account = @IdAccount";
                            command.Parameters.AddWithValue("@Telephone", student.Telephone);
                            command.Parameters.AddWithValue("@StudentNum", student.StudentNum);
                            command.Parameters.AddWithValue("@Address", student.Address);
                            command.Parameters.AddWithValue("@IdAccount", student.IdAccount);
                            sqlConnection.Open();
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
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
            using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                SqlCommand commandtechnician = new SqlCommand("select t.id_technician from Technician t, Account a " +
                    "where t.id_account=@Id and a.id_account = t.id_account", sqlConnection);
                commandtechnician.Parameters.AddWithValue("@Id", GetCurrentUserID());
                SqlDataReader readerTechnician = commandtechnician.ExecuteReader();

                return readerTechnician.HasRows;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileTechnician([Bind("IdAccount, Name, Telephone")] Technician technician)
        {

            if (GetCurrentUserID() != technician.IdAccount)
            {
                return BadRequest();
            }

            //não está a apresentar erros na pagina
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                    {
                        //create await  

                        using (SqlCommand command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = "UPDATE Technician SET telephone = @Telephone" +
                                " WHERE id_account = @IdAccount";
                            command.Parameters.AddWithValue("@Telephone", technician.Telephone);
                            command.Parameters.AddWithValue("@IdAccount", technician.IdAccount);
                            sqlConnection.Open();
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
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

        private bool AccountExists(long id)
        {
            return _context.Account.Any(e => e.IdAccount == id);
        }
    }
}