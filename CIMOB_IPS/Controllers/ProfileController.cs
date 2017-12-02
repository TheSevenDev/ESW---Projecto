using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using System.Data.SqlClient;
using System.Security.Claims;

namespace CIMOB_IPS.Controllers
{
    public class ProfileController : Controller
    {
        public ProfileViewModel GetAccountModelByID(int id)
        {
            var viewModel = new ProfileViewModel{ };

            using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                sqlConnection.Open();
                string sqlQueryStringStudent = "SELECT id_student, a.email, c.name, s.name, address, cc, telephone, "+
                    "n.description, credits, student_num FROM Student s, Course c, Nationality n, Account a "+
                    "WHERE s.id_account = @Id AND s.id_course = c.id_course AND s.id_nationality = n.id_nationality "+
                    "AND s.id_account = a.id_account";

                SqlCommand commandStudent = new SqlCommand(sqlQueryStringStudent, sqlConnection);
                commandStudent.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = commandStudent.ExecuteReader();

                while (reader.Read())
                {
                    var modelStudent = new Student
                    {
                        IdStudent = reader.GetInt64(0),
                        IdCourseNavigation = new Course { Name = reader.GetString(2) },
                        Name = reader.GetString(3),
                        Address = reader.GetString(4),
                        Cc = reader.GetInt64(5),
                        Telephone = reader.GetInt64(6),
                        IdNationalityNavigation = new Nationality { Description = reader.GetString(7) },
                        Credits = reader.GetInt32(8),
                        StudentNum = reader.GetInt64(9)
                    };

                    modelStudent.IdAccountNavigation = new Account { Email = reader.GetString(1) };
                    viewModel.Account = modelStudent;
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
                        Name = reader2.GetString(1),
                        Telephone = reader2.GetInt64(2),
                        IsAdmin = reader2.GetBoolean(3)
                    };

                    modelTech.IdAccountNavigation = new Account { Email = reader.GetString(4) };
                    viewModel.Account = modelTech;
                    viewModel.AccountType = EnumAccountType.TECHNICIAN;

                    reader2.Close();

                    return viewModel;
                }

                reader2.Close();
            }

            return null;
        }

        public IActionResult Index()
        {
            var accountViewModel = GetAccountModelByID(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value));

            //add verificação
            return View(accountViewModel);
        }

        public IActionResult Edit()
        {
            var accountViewModel = GetAccountModelByID(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value));

            //add verificação
            return View(accountViewModel);
        }
    }
}