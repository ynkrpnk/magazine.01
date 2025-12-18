using System;
using System.Data.SqlClient;

namespace magazine._01
{
    class DataBase
    {
        // Використовуємо Database1.mdf, як на вашому скріншоті
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Михайло\Desktop\Уник\3 семестр\алг та сд\Курсач\Database1.mdf;Integrated Security=True;Connect Timeout=30");

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