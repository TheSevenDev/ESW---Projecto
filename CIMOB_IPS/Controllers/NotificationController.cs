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
    public class NotificationController : Controller
    {
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

        public int GetCurrentUserID(ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //return int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }


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

            return PartialView("~/Views/Shared/_Notifications.cshtml", NotificationsCount(User).ToString());
        }

        public List<Notification> GetNotifications(ClaimsPrincipal user)
        {
            List<Notification> list = new List<Notification>();
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "Select * FROM Notification where id_account = @AccountId";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", GetCurrentUserID(user));

                Notification aux = null;

                SqlDataReader reader = scmCommand.ExecuteReader();

                while (reader.Read())
                {
                    aux = new Notification {IdNotification = (long)reader[0], IdAccount = (long)reader[1], Description = (string)reader[2], ReadNotification = (bool)reader[3], ControllerName = (string)reader[4], ActionName = (string)reader[5] }; // FALTA A DATA
                    list.Add(aux);
                }
            }

            return list;
        }


    }
}