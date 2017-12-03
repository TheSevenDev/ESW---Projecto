using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CIMOB_IPS.Models;

namespace CIMOB_IPS.Controllers
{
    public class HelpController : Controller
    {
        /*
        public IActionResult Index()
        {
            return View();
        }*/

        private static string Error = "<h1>Oops!</h1><hr><p>Ocorreu um erro a resgatar ajuda para esta página. É possível ainda não existir nenhuma informação de ajuda para a mesma, por favor tente mais tarde.</p>";

        public static string GetHelpInformation(string controller, string action)
        {
            string htmlResult;

            if (controller != null && action != null)
            {
                using (SqlConnection sqlConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    sqlConnection.Open();
                    string sqlQueryHelp = "SELECT description FROM Help WHERE controller_name = @Controller AND action_name = @Action";

                    SqlCommand commandStudent = new SqlCommand(sqlQueryHelp, sqlConnection);
                    commandStudent.Parameters.AddWithValue("@Controller", controller);
                    commandStudent.Parameters.AddWithValue("@Action", action);
                    SqlDataReader reader = commandStudent.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            htmlResult = reader[0].ToString();
                            sqlConnection.Close();
                            return htmlResult;
                        }
                    }
                }
            }

            return Error;
        }

        public IActionResult _Help(string controllerName, string ActionName)
        {
            string html = GetHelpInformation(controllerName, ActionName);

            ViewData["Help"] = html;
            return PartialView();
        }
    }

    //public class HelpViewComponent : ViewComponent
    //{

    //    public IActionResult _Help(string controllerName, string ActionName)
    //    {
    //        string html = HelpController.GetHelpInformation(controllerName, ActionName);

    //        ViewData["Help"] = html;
    //        return Redi
    //    }
    //}
}