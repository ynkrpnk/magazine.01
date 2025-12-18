using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Михайло\Desktop\Уник\3 семестр\алг та сд\Курсач\Database1.mdf;Integrated Security=True;Connect Timeout=30");

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося підключитися до БД. Перевірте файл ShopDB.mdf у папці bin/Debug.\n\nПомилка: " + ex.Message);
            }
            
            LoadData();
            this.dataGridView1.RowPostPaint += dataGridView1_RowPostPaint;

            //ширина первых двух столбцов, остальные равномерно растягиваются
            dataGridView1.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Column1"].Width = 70;
            dataGridView1.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Name"].Width = 150;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //FillTableWithRandomData(); наполняет бд 10000 случайными записями(надо сделать один раз). уже сделано
        }

        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Shop", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Shop");

                dataGridView1.DataSource = dataSet.Tables["Shop"];
                dataGridView1.Columns["Id"].Visible = false;

                FillAlgorithms();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження: " + ex.Message);
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
                FillAlgorithms();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                // клик не по данным
                if (e.RowIndex < 0)
                    return;

                // ====== 1️⃣ КЛИК ПО PhotoPath ======
                if (dataGridView1.Columns[e.ColumnIndex].Name == "PhotoPath")
                {
                    string relativePath = dataGridView1.Rows[e.RowIndex]
                                                      .Cells["PhotoPath"]
                                                      .Value?
                                                      .ToString();

                    if (string.IsNullOrWhiteSpace(relativePath))
                        return;

                    string fullPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        relativePath.Trim()
                    );

                    if (!File.Exists(fullPath))
                    {
                        MessageBox.Show("Файл фото не найден:\n" + fullPath);
                        return;
                    }

                    Form imgForm = new Form
                    {
                        Width = 600,
                        Height = 600,
                        Text = "Фото"
                    };

                    PictureBox pb = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(fullPath)
                    };

                    imgForm.Controls.Add(pb);
                    imgForm.ShowDialog();

                    return;
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) => ReloadData();

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


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (AddProductForm f = new AddProductForm(dataSet.Tables["Shop"]))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    dataSet.Tables["Shop"].Rows.Add(f.NewRow);
                    sqlDataAdapter.Update(dataSet, "Shop");
                    //RenumberIDs();
                    ReloadData();
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи є виділений рядок
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть рядок для редагування!");
                return;
            }

            // Беремо перший виділений рядок
            int rowIndex = dataGridView1.SelectedRows[0].Index;

            // Беремо відповідний DataRow з DataTable
            DataRow row = dataSet.Tables["Shop"].Rows[rowIndex];

            // Відкриваємо форму редагування
            using (EditProductForm f = new EditProductForm(row))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    // Зберігаємо зміни в базу
                    sqlDataAdapter.Update(dataSet, "Shop");

                    // Оновлюємо DataGridView
                    ReloadData();
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть рядок для видалення!");
                return;
            }

            if (MessageBox.Show($"Видалити {dataGridView1.SelectedRows.Count} обраних елементів?",
                                "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                // Проходимо по виділеним рядкам з кінця
                for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int rowIndex = dataGridView1.SelectedRows[i].Index;
                    dataSet.Tables["Shop"].Rows[rowIndex].Delete();
                }

                // Зберігаємо зміни у базу
                sqlDataAdapter.Update(dataSet, "Shop");

                //RenumberIDs();

                // Оновлюємо DataGridView
                ReloadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка видалення: " + ex.Message);
            }
        }

        //заполнояет столбец порядкового номера. это не Id. Id хранится в бд, и генерируется автоматически, он не соответствует порядковому номеру. Column1 существует только в dataGridView1(в бд нету)
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex]
                .Cells["Column1"].Value = e.RowIndex + 1;
        }

        private void FillTableWithRandomData(int count = 10000)
        {
            if (dataSet == null || dataSet.Tables["Shop"] == null)
                return;

            var table = dataSet.Tables["Shop"];
            Random rnd = new Random();

            for (int i = 21; i <= count+21; i++)
            {
                int catVal = rnd.Next(1, 6); // 1..5
                int prVal = rnd.Next(100, 50001);
                prVal = (prVal / 100) * 100; // округление до сотен
                int prQuan = rnd.Next(10, 101);

                DataRow newRow = table.NewRow();
                newRow["Name"] = $"product-{i}";
                newRow["Category"] = $"category-{catVal}";
                newRow["Price"] = prVal;
                newRow["Quantity"] = prQuan;
                newRow["Producer"] = $"producer-{i}";

                table.Rows.Add(newRow);
            }

            // Сохраняем в базу
            sqlDataAdapter.Update(table);
            ReloadData();
            MessageBox.Show($"{count} записей успешно добавлено!");
        }

    }
}