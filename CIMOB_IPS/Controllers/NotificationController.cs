using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CIMOB_IPS.Models;

namespace CIMOB_IPS.Controllers
{
    public class NotificationController : Controller
    {
        public static int NotificationsCount()
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "SELECT COUNT(*) FROM Notification where id_account = @AccountId AND ReadNotification = false";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", Account.GetCurrentUserID());
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
    }
}