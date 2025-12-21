using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace magazine._01
{
    public partial class log_in : Form
    {
        DataBase database = new DataBase();

        public log_in()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void sing_in_Load(object sender, EventArgs e)
        {
            passUser.PasswordChar = '*';
            pictureBox1.Visible = false;
            loginUser.MaxLength = 50;
            passUser.MaxLength = 50;
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            var login = loginUser.Text.Trim();
            var pass = passUser.Text.Trim();

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            // ВИПРАВЛЕНО: Беремо всі колонки (*), щоб не помилитися з назвою ID
            string querystring = $"SELECT * FROM Register WHERE login_user = '{login}' AND password_user = '{pass}'";

            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            adapter.SelectCommand = command;

            try
            {
                adapter.Fill(table);

                if (table.Rows.Count == 1)
                {
                    MessageBox.Show("   Ви увійшли!        ", "Успішно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Запускаємо головну форму (richTextBoxSearchLog)
                    richTextBoxSearchLog.UserName = login;
                    richTextBoxSearchLog frm1 = new richTextBoxSearchLog();
                    this.Hide();
                    frm1.ShowDialog();
                    // Закриваємо програму після закриття головної форми
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("   Такого акаунта не існує!    ", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка БД: " + ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sing_up frm_sing = new sing_up();
            frm_sing.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            loginUser.Text = "";
            passUser.Text = "";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            passUser.UseSystemPasswordChar = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            passUser.UseSystemPasswordChar = true;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        }
    }
}