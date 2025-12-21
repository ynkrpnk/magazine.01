using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace magazine._01
{
    public partial class EditProductForm : Form
    {
        private readonly DataRow rowToEdit;
        private string selectedPhotoFileName = null;
        private richTextBoxSearchLog mainForm;

        public EditProductForm(DataRow row, richTextBoxSearchLog mainForm)
        {
            InitializeComponent();
            rowToEdit = row;
            this.mainForm = mainForm;

            txtCategory.DataSource = mainForm.AllCategories;

            // Заповнюємо поля значеннями з рядка
            txtName.Text = row["Name"].ToString();
            txtCategory.Text = row["Category"].ToString();
            numPrice.Value = Convert.ToDecimal(row["Price"]);
            numNumber.Value = Convert.ToDecimal(row["Quantity"]);
            txtProducer.Text = row["Producer"].ToString();
            txtPhoto.Text = row["PhotoPath"].ToString();

            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnSelectPhoto.Click += new System.EventHandler(this.btnSelectPhoto_Click);
        }

        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Images (*.png;*.jpg)|*.png;*.jpg"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
            Directory.CreateDirectory(imgDir);

            string fileName = Path.GetFileName(ofd.FileName);
            string destPath = Path.Combine(imgDir, fileName);
            File.Copy(ofd.FileName, destPath, true);

            selectedPhotoFileName = fileName;
            txtPhoto.Text = @"img\" + fileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введіть назву");
                return;
            }

            // Зберігаємо зміни у рядку
            rowToEdit["Name"] = txtName.Text;
            rowToEdit["Category"] = txtCategory.Text;
            rowToEdit["Price"] = numPrice.Value;
            rowToEdit["Quantity"] = numNumber.Value;
            rowToEdit["Producer"] = txtProducer.Text;

            if (!string.IsNullOrEmpty(selectedPhotoFileName))
                rowToEdit["PhotoPath"] = @"img\" + selectedPhotoFileName;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
