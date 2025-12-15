using System;
using System.Data.SqlClient;

namespace magazine._01
{
    class DataBase
    {
        // Використовуємо Database1.mdf, як на вашому скріншоті
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\проекти.0.1\magazine.01\magazine.01\bin\Debug\Database1.mdf;Integrated Security=True;Connect Timeout=30");

        public void openConnection()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void closeConnection()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return con;
        }
    }
}