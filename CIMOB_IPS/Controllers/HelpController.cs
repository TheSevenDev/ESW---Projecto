using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CIMOB_IPS.Models;

namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador da Ajuda presente na aplicação.
    /// </summary>
    /// <remarks></remarks>
    public class HelpController : Controller
    {
        /// <summary>
        /// HTML que é usado por defeito caso não exista nenhuma ajuda registada na base de dados.
        /// </summary>
        /// <remarks></remarks>
        private static string strError = "<h1>Oops!</h1><hr><p>Ocorreu um erro a resgatar ajuda para esta página. É possível ainda não existir nenhuma informação de ajuda para a mesma, por favor tente mais tarde.</p>";


        /// <summary>
        /// Retorna a mensagem de ajuda do controlador e ação correspondente à página atual.
        /// </summary>
        /// <param name="strController">Nome do controlador</param>
        /// <param name="strAction">Nome da ação</param>
        /// <returns>HTML com ajuda</returns>
        /// <remarks></remarks>
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
                    if (dtrReader.HasRows)
                    {
                        while (dtrReader.Read())
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

        /// <summary>
        /// Retorna a partial view da ajuda, mostrada em todas as páginas.
        /// </summary>
        /// <param name="strControllerName">Nome do Controlador</param>
        /// <param name="strActionName">Nome da ação</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IActionResult _Help(string strControllerName, string strActionName)
        {
            string html = GetHelpInformation(strControllerName, strActionName);

            ViewData["Help"] = html;
            return PartialView();
        }
    }
}