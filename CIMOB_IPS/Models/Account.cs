using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CIMOB_IPS.Models
{
    public partial class Account
    {
        public Account()
        {
            Notification = new HashSet<Notification>();
            Student = new HashSet<Student>();
            Technician = new HashSet<Technician>();
        }

        public long IdAccount { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public bool Active { get; set; }

        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }


        public static bool IsRegistered(string email, string password)
        {

            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = "select email from Student where email=@email and password=@password";
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", EncryptToMD5(password));
                SqlDataReader reader = command.ExecuteReader();

                return reader.HasRows;


            }
        }
    
               
                

        private static string EncryptToMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(password));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        
    }
    }

    
}
