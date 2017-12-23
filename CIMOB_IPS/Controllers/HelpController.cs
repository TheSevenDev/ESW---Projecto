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
        private static string strError = "<h1>Oops!</h1><hr><p>Ocorreu um erro a resgatar ajuda para esta página. É possível ainda não existir nenhuma informação de ajuda para a mesma, por favor tente mais tarde.</p>";


        public static string GetHelpInformation(string strController, string strAction)
        {
            string strHtmlResult;

            if (strController != null && strAction != null)
            {
                using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
                {
                    scnConnection.Open();
                    string strQuery = "SELECT description FROM Help WHERE controller_name = @Controller AND action_name = @Action";

                    SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                    scmCommand.Parameters.AddWithValue("@Controller", strController);
                    scmCommand.Parameters.AddWithValue("@Action", strAction);
                    SqlDataReader dtrReader = scmCommand.ExecuteReader();
                    if(dtrReader.HasRows)
                    {
                        while(dtrReader.Read())
                        {
                            strHtmlResult = dtrReader[0].ToString();
                            scnConnection.Close();
                            return strHtmlResult;
                        }
                    }
                }
            }

            return strError;
        }

        public IActionResult _Help(string strControllerName, string strActionName)
        {
            string html = GetHelpInformation(strControllerName, strActionName);

            ViewData["Help"] = html;
            return PartialView();
        }
    }
}