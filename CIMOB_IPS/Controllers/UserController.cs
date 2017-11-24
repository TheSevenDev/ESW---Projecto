using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CIMOB_IPS.Models;
using System.Data.SqlClient;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        public string connectionString = @"Data Source=esw-cimob-db.database.windows.net;Database=CIMOB_IPS_DB;
                Integrated Security=False;User ID=adminUser; Password=f00n!l06;Connect Timeout=60;Encrypt=False;
                TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public IActionResult PreRegister(IFormCollection form)
        {
            String studentName = Convert.ToString(form["Student.Name"].ToString());
            long studentNumber = Convert.ToInt64(form["Student.StudentNum"].ToString());

            InsertPreRegister(studentName, studentNumber);

            return View("Index");
        }

        public IActionResult Register()
        {

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

        [ActionName("ConvidarTecnico")]
        public IActionResult Invite()
        {
            return View("Invite");
        }

        public void InsertPreRegister(String studentName, long studentNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "insert into dbo.Pending_Account values (@Name,@StudentNumber)";
                command.Parameters.AddWithValue("@Name", studentName);
                command.Parameters.AddWithValue("@StudentNumber", studentNumber);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}