using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
//COMMIT

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

        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }


        public static bool IsRegistered(string _email, string _password)
        {
            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "select * from Account where email=@email";
                command.Parameters.AddWithValue("@email", _email);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        string bdpw = ToHex((byte[])reader[2], false);
                        if (bdpw.Equals(EncryptToMD5(_password)))
                        {
                            return true;
                        }
                    }
                }
                connection.Close();  
            }

            return false;
        }
             

        private static string EncryptToMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Console.WriteLine(password);
            md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(password));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            
            return strBuilder.ToString();
        
        }

        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }

    
}
