using System;
using System.Data.SqlClient;

namespace magazine._01
{
    class DataBase
    {
        // C:\Users\Михайло\Desktop\Уник\3 семестр\алг та сд\Курсач\Database1.mdf
        // C:\Users\user\RiderProjects\magazine.01\Database1.mdf
        public static string MdfFilePath = @"C:\Users\user\RiderProjects\magazine.01\Database1.mdf";
        // Використовуємо Database1.mdf, як на вашому скріншоті
        //SqlConnection con = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={MdfFilePath};Integrated Security=True;Connect Timeout=30");

        // Имя базы, которую ты создал через FOR ATTACH_REBUILD_LOG
        public static string DatabaseName = "Database2";

        // Строка подключения к LocalDB через имя базы
        SqlConnection con = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;Database={MdfFilePath};Integrated Security=True;Connect Timeout=30");


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