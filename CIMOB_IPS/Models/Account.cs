﻿using System;
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


        public static LoginState IsRegistered(string _email, string _password)
        {
            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                string accountID = "";
                try { 
                    connection.Open();
                }
                catch(SqlException e)
                {
                    return LoginState.CONNECTION_FAILED;
                }

                command.CommandText = "select * from Account where email=@email";
                command.Parameters.AddWithValue("@email", _email);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        accountID = reader[0].ToString();
                        string bdpw = ToHex((byte[])reader[2], false);
                        if (!bdpw.Equals(EncryptToMD5(_password)))
                        {
                            return LoginState.WRONG_PASSWORD;
                        }
                    }
                }
                else
                {
                    return LoginState.EMAIL_NOTFOUND;
                }

                connection.Close();
                if(AccountType(accountID) == UserType.STUDENT)
                {
                    return LoginState.CONNECTED_STUDENT;
                }
                else
                {
                    return LoginState.CONNECTED_TECH;
                }              
            }    
        }

        public static string AccountID(string email)
        {
            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();

                command.CommandText = "select id_account from Account where email=@email";
                command.Parameters.AddWithValue("@email", email);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read()) { 
                        return reader[0].ToString();
                    }
                }
            }
            return "";
        }


        public static UserType AccountType(string _accountId)
        {

            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();

                command.CommandText = "select id_student from Student where id_account=@id_account";
                command.Parameters.AddWithValue("@id_account", _accountId);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return UserType.STUDENT;
                }
                else
                {
                    return UserType.TECHNICIAN;
                }
            }
        }

        public static string AccountName(string _accountId)
        {

            using (SqlConnection connection = new SqlConnection(Startup.connection))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                if(AccountType(_accountId) == UserType.STUDENT)
                    command.CommandText = "select name from Student where id_account=@id_account";
                else
                    command.CommandText = "select name from Technician where id_account=@id_account";


                command.Parameters.AddWithValue("@id_account", _accountId);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("=================NOME=======================" + reader[0]);
                        return reader[0].ToString();
                    }             
                }

                return "";
            }
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
