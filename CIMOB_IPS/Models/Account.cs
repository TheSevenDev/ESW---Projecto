using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Required(ErrorMessage = "O email não preenchido")]
        [EmailAddress(ErrorMessage = "O email deverá conter a seguinte estrutura: exemplo@dominio.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password não está preenchida")]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public byte[] Password { get; set; }

        [NotMapped] 
        [Required(ErrorMessage = "A confirmação da password não está preenchida")]
        [Compare("Password", ErrorMessage ="As passwords não coincidem.")]
        [Display(Name = "Confirme a Password:")]
        [DataType(DataType.Password)]
        public byte[] ConfirmPassword { get; set; }

        public ICollection<Notification> Notification { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Technician> Technician { get; set; }

        public static LoginState IsRegistered(string _strEmail, string _strPassword)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                string strAccountID = "";

                try { 
                    scnConnection.Open();
                }
                catch(SqlException e)
                {
                    return LoginState.CONNECTION_FAILED;
                }

                scmCommand.CommandText = "select * from Account where email=@email";
                scmCommand.Parameters.AddWithValue("@email", _strEmail);

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read())
                    {
                        strAccountID = dtrReader[0].ToString();

                        string strBDPW = ToHex((byte[])dtrReader[2], false);

                        if (!strBDPW.Equals(EncryptToMD5(_strPassword)))
                        {
                            return LoginState.WRONG_PASSWORD;
                        }
                    }
                }
                else
                {
                    return LoginState.EMAIL_NOTFOUND;
                }

                scnConnection.Close();

                if(AccountType(strAccountID) == EnumUserType.STUDENT)
                {
                    return LoginState.CONNECTED_STUDENT;
                }
                else
                {
                    return LoginState.CONNECTED_TECH;
                }              
            }    
        }

        public static string AccountID(string strEmail)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scnConnection.Open();

                scmCommand.CommandText = "select id_account from Account where email=@email";
                scmCommand.Parameters.AddWithValue("@email", strEmail);

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read()) { 
                        return dtrReader[0].ToString();
                    }
                }
            }
            return "";
        }

        public static EnumUserType AccountType(string _strAccountID)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scnConnection.Open();

                scmCommand.CommandText = "select id_student from Student where id_account=@id_account";
                scmCommand.Parameters.AddWithValue("@id_account", _strAccountID);

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (dtrReader.HasRows)
                {
                    return EnumUserType.STUDENT;
                }
                else
                {
                    return EnumUserType.TECHNICIAN;
                }
            }
        }

        public static string AccountName(string _strAccountID)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scnConnection.Open();

                if(AccountType(_strAccountID) == EnumUserType.STUDENT)
                    scmCommand.CommandText = "select name from Student where id_account=@id_account";
                else
                    scmCommand.CommandText = "select name from Technician where id_account=@id_account";

                scmCommand.Parameters.AddWithValue("@id_account", _strAccountID);

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read())
                    {
                        return dtrReader[0].ToString();
                    }             
                }

                return "";
            }
        }

        public static string IsAdmin(string _strAccountID)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            using (SqlCommand scmCommand = new SqlCommand("", scnConnection))
            {
                scnConnection.Open();
                scmCommand.CommandText = "select is_admin from Technician where id_account=@id_account";

                scmCommand.Parameters.AddWithValue("@id_account", _strAccountID);

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                if (dtrReader.HasRows)
                {
                    while (dtrReader.Read())
                    {
                        return dtrReader[0].ToString();
                    }
                }

                return "";
            }
        }

        public static string EncryptToMD5(string strPassword)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Console.WriteLine(strPassword);
            md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(strPassword));

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
            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                stringBuilder.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return stringBuilder.ToString();
        }
    }
}
