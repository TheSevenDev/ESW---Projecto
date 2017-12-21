using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using CIMOB_IPS.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CIMOB_IPS.Controllers
{
    public class NotificationController : Controller
    {
        public int NotificationsCount()
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "SELECT COUNT(*) FROM Notification where id_account = @AccountId AND ReadNotification = false";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AccountId", GetCurrentUserID());
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

     public int GetCurrentUserID()
     {
        return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
     }


    }
}