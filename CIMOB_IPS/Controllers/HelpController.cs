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

        private static string Error1 = "<h1>Oops!</h1><hr><p>Ocorreu um erro a resgatar ajuda para esta página. É possível ainda não existir nenhuma informação de ajuda para a mesma, por favor tente mais tarde.</p>";
        private static string Error = "<h1>Autenticação</h1><hr> " +
            "<p>De modo a autenticar-se na aplicação, é necessário providenciar os seguintes dados:</p>" +
            "<ul><li><span class='li-title'>E-mail: </span>O endereço de e-mail com o qual se registou na aplicação;</li>" +
            "<li><span class='li-title'>Palavra-passe: </span>A palavra-passe que definiu no registo da respectiva conta.</li></ul>" +
            "<p><b>Manter sessão: </b>É possível manter sessão da conta, de modo a que possa, mais facilmente, aceder à aplicação" +
            " quando regressar, sem a necessidade de re-autenticação.</p>" +
            "<br>" +
            "<h4>Esqueceu-se da palavra-passe?</h4>" +
            "<p>É possível, através do e-mail da conta, recuperar a palavra-passe. É enviado para este um e-mail com uma palavra-passe gerada" +
            " aleatoriamente, que será definida como a nova palavra-passe da conta até escolher modificá-la.</p>";

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