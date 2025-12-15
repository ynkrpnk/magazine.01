using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace magazine._01
{
    // ВАЖЛИВО: Ім'я класу має бути Form1
    //отстань
    public partial class richTextBoxSearchLog : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        private bool newRowAdding = false;

        private LinearSearch linearAlg = new LinearSearch();
        private RedBlackTree<MusicInstrument> rbTree = new RedBlackTree<MusicInstrument>();

        public richTextBoxSearchLog()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Використовуємо універсальний шлях |DataDirectory|
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\проекти.0.1\magazine.01\magazine.01\bin\Debug\Database1.mdf;Integrated Security=True;Connect Timeout=30");

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося підключитися до БД. Перевірте файл ShopDB.mdf у папці bin/Debug.\n\nПомилка: " + ex.Message);
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Command] FROM Shop", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Shop");

                dataGridView1.DataSource = dataSet.Tables["Shop"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, i] = linkCell;
                }

                FillAlgorithms();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Помилка завантаження: " + ex.Message);
            }
        }

        // Метод, який перебирає дані з таблиці і заповнює алгоритми
        private void FillAlgorithms()
        {
            // 1. Очищаємо старі дані, щоб не було дублікатів при перезавантаженні
            linearAlg.Clear();
            rbTree.Clear();

            // Перевірка, чи є дані в таблиці
            if (dataSet == null || dataSet.Tables["Shop"] == null) return;

            // 2. Проходимо по кожному рядку таблиці
            foreach (DataRow row in dataSet.Tables["Shop"].Rows)
            {
                // Пропускаємо видалені рядки (важливо, щоб не виникало помилок)
                if (row.RowState == DataRowState.Deleted) continue;

                try
                {
                    // === БЕРЕМО ДАНІ З БАЗИ ===
                    int id = Convert.ToInt32(row[0]); // ID (перша колонка)
                    string name = row["Name"].ToString(); // Назва

                    string category = row["Category"].ToString(); // Назва

                    int price = 0;
                    if (row["Price"] != DBNull.Value)
                    {
                        price = Convert.ToInt32(row["Price"]); // Ціна
                    }

                    // Створюємо інструмент
                    MusicInstrument instrument = new MusicInstrument(id, name, category, price);

                    // === ЗАПОВНЮЄМО АЛГОРИТМИ ===

                    // 1. Лінійний пошук
                    var added = linearAlg.Insert(instrument);
                    //var added = linearAlg.Insert(new MusicInstrument(1, "Гітара", 5000));

                    //richTextBox1.AppendText($"Додано: {added.Name}, ціна: {added.Price}" + Environment.NewLine);


                    // 2. Дерево (використовуємо try-catch на випадок дублікатів ID)
                    try
                    {
                        rbTree.Insert(id, instrument);
                    }
                    catch { }
                }
                catch
                {
                    // Якщо рядок пошкоджений - ігноруємо
                }
            }
        }

        private void ReloadData()
        {
            try
            {
                dataSet.Tables["Shop"].Clear();
                sqlDataAdapter.Fill(dataSet, "Shop");
                dataGridView1.DataSource = dataSet.Tables["Shop"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, i] = linkCell;
                }
                FillAlgorithms();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // === УНІВЕРСАЛЬНИЙ МЕТОД ПОШУКУ ===
        private void PerformSearch(bool isTree)
        {
            // Очистка
            if (this.Controls.ContainsKey("richTextBox1"))
                ((RichTextBox)this.Controls["richTextBox1"]).Clear();

            // Читаем напрямую (замените на реальные имена контролов из Designer)
            string nameInput = textBoxForName?.Text.Trim() ?? "";
            string categoryInput = textBoxForCategory?.Text.Trim() ?? ""; // <- исправьте имя, если оно другое

            // Для отладки временно покажите значения
            // MessageBox.Show($"name:'{nameInput}' category:'{categoryInput}'");

            if (string.IsNullOrEmpty(nameInput) || string.IsNullOrEmpty(categoryInput))
            {
                MessageBox.Show("Введіть категорію та назву для пошуку!");
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();
            string els = "";
            //MusicInstrument found = null;
            string log = "";

            if (isTree)
            {
                var res = rbTree.Find(nameInput, categoryInput);
                els = res.els;
                log = res.log;
            }
            else
            {
                var res = linearAlg.Find(nameInput, categoryInput);
                els = res.els;
                log = res.log;
            }

            sw.Stop();
            labelTime.Text = $"{sw.Elapsed.TotalMilliseconds:F4} мс";
            string algoName = isTree ? "Червоно-Чорне дерево" : "Лінійний пошук";
            PrintResultToLog(els, log, $"--- {algoName} ---\n\n");
        }

        private void buttonLinerSearch_Click(object sender, EventArgs e)
        {
            PerformSearch(isTree: false);
        }

        private void buttonRBTree_Click(object sender, EventArgs e)
        {
            PerformSearch(isTree: true);
        }

        private void PrintResultToLog(string els, string log, string algorithmName)
        {
            RichTextBox rtb = this.Controls["richTextBox1"] as RichTextBox;
            rtb.AppendText(algorithmName);

            if (rtb != null)
            {
                if (!string.IsNullOrEmpty(els))
                {
                    rtb.SelectionColor = Color.Green;
                    rtb.AppendText(els);
                    rtb.SelectionColor = Color.Black;
                }
                else
                {
                    rtb.SelectionColor = Color.Red;
                    rtb.AppendText("НЕ ЗНАЙДЕНО.\n\n");
                    rtb.SelectionColor = Color.Black;
                }

                rtb.AppendText("\n--- ЛОГ ПОШУКУ ---\n");
                rtb.AppendText(log);
            }
        }

        // --- СТАНДАРТНІ МЕТОДИ ТАБЛИЦІ ---
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 6)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Видалити?", "Видалення", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            dataSet.Tables["Shop"].Rows[e.RowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "Shop");
                            ReloadData();
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["Shop"].NewRow();
                        row["Name"] = dataGridView1.Rows[rowIndex].Cells["Name"].Value;
                        row["Category"] = dataGridView1.Rows[rowIndex].Cells["Category"].Value;
                        row["Price"] = dataGridView1.Rows[rowIndex].Cells["Price"].Value;
                        row["Number"] = dataGridView1.Rows[rowIndex].Cells["Number"].Value;
                        row["Producer"] = dataGridView1.Rows[rowIndex].Cells["Producer"].Value;
                        dataSet.Tables["Shop"].Rows.Add(row);
                        dataSet.Tables["Shop"].Rows.RemoveAt(dataSet.Tables["Shop"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";
                        sqlDataAdapter.Update(dataSet, "Shop");
                        newRowAdding = false;
                        ReloadData();
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;
                        dataSet.Tables["Shop"].Rows[r]["Name"] = dataGridView1.Rows[r].Cells["Name"].Value;
                        dataSet.Tables["Shop"].Rows[r]["Category"] = dataGridView1.Rows[r].Cells["Category"].Value;
                        dataSet.Tables["Shop"].Rows[r]["Price"] = dataGridView1.Rows[r].Cells["Price"].Value;
                        dataSet.Tables["Shop"].Rows[r]["Number"] = dataGridView1.Rows[r].Cells["Number"].Value;
                        dataSet.Tables["Shop"].Rows[r]["Producer"] = dataGridView1.Rows[r].Cells["Producer"].Value;
                        sqlDataAdapter.Update(dataSet, "Shop");
                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";
                        ReloadData();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) => ReloadData();

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try { if (!newRowAdding) { newRowAdding = true; int last = dataGridView1.Rows.Count - 2; dataGridView1[6, last] = new DataGridViewLinkCell(); dataGridView1.Rows[last].Cells["Command"].Value = "Insert"; } } catch { }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try { if (!newRowAdding) { int idx = dataGridView1.SelectedCells[0].RowIndex; dataGridView1[6, idx] = new DataGridViewLinkCell(); dataGridView1.Rows[idx].Cells["Command"].Value = "Update"; } } catch { }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 3)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null) textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
            }
        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void завантажитиДаніToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void textBoxShowSearch_TextChanged(object sender, EventArgs e) { }
        private void textBoxForSearch_TextChanged(object sender, EventArgs e) { }
        private void labelTime_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void textBoxForPrice_TextChanged(object sender, EventArgs e)
        {

        }
    }
}