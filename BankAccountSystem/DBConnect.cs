﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountSystem
{
    public  class DBConnect
    {
        SqlConnection connect = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        private string con;
        public string myConnection()
        {
            con = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=BankDatabase;Integrated Security=True;Encrypt=False";
            return con;
        }

        public DataTable getTable(string qury)
        {
            connect.ConnectionString = myConnection();
            cmd = new SqlCommand(qury, connect);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

    }
}
