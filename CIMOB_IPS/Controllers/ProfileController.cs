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
        public Account GetAccountModelByID(int id)
        {
            var account = new Account{ IdAccount = id };

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

                    account.Email = reader.GetString(1);

                    reader.Close();

                    //VER SE HA MELHOR FORMA PA FAZER ISTO
                    account.Student.Add(modelStudent);

                    return account;
                }

                reader.Close();

                SqlCommand commandtechnician = new SqlCommand("select t.*, a.email from Technician t, Account a " + 
                    "where t.id_account=@Id and a.id_account = t.id_account", sqlConnection);
                commandtechnician.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader2 = commandtechnician.ExecuteReader();

                while (reader2.Read())
                {
                    var modelTech = new Technician
                    {
                        IdTechnician = reader2.GetInt64(0),
                        Name = reader2.GetString(2),
                        Telephone = reader2.GetInt64(3),
                        IsAdmin = reader2.GetBoolean(4)
                    };

                    account.Email = reader2.GetString(5);

                    reader2.Close();

                    account.Technician.Add(modelTech);

                    return account;
                }
            }

            return null;
        }

        public IActionResult Index()
        {
            var account = GetAccountModelByID(int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value));

            //add verificação
            return View(account);
        }
    }
}