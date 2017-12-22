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
     }


    }
}