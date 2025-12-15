using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace magazine._01
{
    public partial class sing_up : Form
    {
        DataBase dataBase = new DataBase();

        public sing_up()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void sing_up_Load(object sender, EventArgs e)
        {
            passUser1.PasswordChar = '*';
            pictureBox1.Visible = false;
            loginUser1.MaxLength = 50;
            passUser1.MaxLength = 50;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var login = loginUser1.Text.Trim();
            var password = passUser1.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка");
                return;
            }

            if (checkuser())
            {
                return;
            }

            string querystring = $"INSERT INTO Register (login_user, password_user) VALUES ('{login}', '{password}')";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            try
            {
                dataBase.openConnection();

                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Акаунт створено!", "Успіх!");
                    log_in frm_login = new log_in();
                    this.Hide();
                    frm_login.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Акаунт не створений (помилка БД).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
            finally
            {
                dataBase.closeConnection();
            }
        }

        private Boolean checkuser()
        {
            var loginUser = loginUser1.Text.Trim();
            var passUser = passUser1.Text.Trim();

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string querystring = $"SELECT * FROM Register WHERE login_user = '{loginUser}'";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Користувач вже існує!");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            loginUser1.Text = "";
            passUser1.Text = "";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            passUser1.UseSystemPasswordChar = true;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            passUser1.UseSystemPasswordChar = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;
        }
    }
}