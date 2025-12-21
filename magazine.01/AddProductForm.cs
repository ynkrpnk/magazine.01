using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace magazine._01
{
    public partial class AddProductForm : Form
    {
        private readonly DataTable shopTable;
        private string selectedPhotoFileName = null;
        private richTextBoxSearchLog mainForm;

        public DataRow NewRow { get; private set; }

        public AddProductForm(DataTable table, richTextBoxSearchLog mainForm)
        {
            InitializeComponent();
            shopTable = table;
            this.mainForm = mainForm;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnSelectPhoto.Click += new System.EventHandler(this.btnSelectPhoto_Click);
            comboBoxCategory.DataSource = mainForm.AllCategories;
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

            /*if (selectedPhotoFileName == null)
            {
                MessageBox.Show("Оберіть фото");
                return;
            }*/

            NewRow = shopTable.NewRow();

            NewRow["Name"] = txtName.Text;
            NewRow["Category"] = comboBoxCategory.Text;
            NewRow["Price"] = numPrice.Value;
            NewRow["Quantity"] = numNumber.Value;
            NewRow["Producer"] = txtProducer.Text;
            NewRow["PhotoPath"] = @"img\" + selectedPhotoFileName;

            DialogResult = DialogResult.OK;
            mainForm.AddCategory(comboBoxCategory.Text);
            Close();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

}
