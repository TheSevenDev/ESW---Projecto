using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador para o sistema de notifições da aplicação.
    /// </summary>
    /// <remarks></remarks>
    public class NotificationController : Controller
    {

        /// <summary>
        /// Retorna o número de notificações não lidas do utilizador autenticado.
        /// </summary>
        /// <param name="user">Utilizador autenticado</param>
        /// <returns>Número de notificações não lidas do utilizador autenticado.</returns>
        /// <remarks></remarks>
        public int NotificationsCount(ClaimsPrincipal user)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "SELECT COUNT(*) FROM Notification where id_account = @AccountId AND read_notification = 0";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", GetCurrentUserID(user));
                SqlDataReader dtrReader = scmCommand.ExecuteReader();
                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read())
                    {
                        return (int)dtrReader[0];
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Retorna a chave primária associada à conta do utilizador autenticado no momento.
        /// </summary>
        /// <returns>Chave primária associada à conta do utilizador autenticado no momento</returns>
        /// <remarks></remarks>
        public int GetCurrentUserID(ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }


        /// <summary>
        /// Atualiza todas as notificações do utilizador para lidas. 
        /// Este método é usado para quando o utilizador clicar no icon das notificações, este executar um pedido AJAX com a referência para esta ação e ler as notificações do utilizador autenticado.
        /// </summary>
        /// <returns>PartialView com as notificações lidas</returns>
        /// <remarks></remarks>
        [HttpPost]
        public ActionResult ReadNotifications()
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "UPDATE Notification SET read_notification = 1 where id_account = @AccountId";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", GetCurrentUserID(User));

                scmCommand.ExecuteNonQuery();
            }

            return PartialView("~/Views/Shared/_Notifications.cshtml");
        }

        /// <summary>
        /// Retorna uma lista com as notificações do utilizador autenticado.
        /// </summary>
        /// <param name="user">Utilizador autenticado</param>
        /// <returns>Lista com as notificações do utilizador autenticado.</returns>
        /// <remarks></remarks>
        public List<Notification> GetNotifications(ClaimsPrincipal user)
        {
            List<Notification> list = new List<Notification>();
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                //consulta as notificações ordenadas decrescentemente por data
                string strQuery = "Select * FROM Notification where id_account = @AccountId order by notification_date desc";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", GetCurrentUserID(user));

                Notification aux = null;

                SqlDataReader reader = scmCommand.ExecuteReader();

                while (reader.Read())
                {
                    aux = new Notification { IdNotification = (long)reader[0], IdAccount = (long)reader[1], Description = (string)reader[2], ReadNotification = (bool)reader[3], ControllerName = (string)reader[4], ActionName = (string)reader[5] }; // FALTA A DATA
                    list.Add(aux);
                }
            }

            return list;
        }


    }
}