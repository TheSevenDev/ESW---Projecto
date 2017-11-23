using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using System.Data.SqlClient;


namespace CIMOB_IPS.Controllers
{
    public class ProfileController : Controller
    {
        public string connection= @"Data Source=esw-cimob-db.database.windows.net;Database=CIMOB_IPS_DB;
                Integrated Security=False;User ID=adminUser; Password=f00n!l06;Connect Timeout=60;Encrypt=False;
                TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public void SelectIdStudent(int id) {
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            using (SqlCommand command = new SqlCommand("", sqlConnection))
            {
                command.CommandText = "select id_student from dbo.Student where id_account=@Id";
                command.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    command.CommandText = "select id_technician from dbo.Technician where id_account=@Id";
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader2 = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        var model = new Technician();
                        model.IdTechnician = id;
                    }
                }
                else
                {
                    var model = new Student();
                    model.IdStudent = id;
                }
            }
        }

            public IActionResult Index()
        {
            return View();
        }
    }
}