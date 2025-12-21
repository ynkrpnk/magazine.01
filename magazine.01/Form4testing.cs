using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace magazine._01
{
    public partial class Form4testing : Form
    {
        private richTextBoxSearchLog mainForm;

        public Form4testing(richTextBoxSearchLog form)
        {
            InitializeComponent();
            mainForm = form; // сохраняем ссылку
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = mainForm.MeasureSearchTime(progressBar1);
            label3.Text = $"{result.avgLinear} тіків";
            label4.Text = $"{result.avgTree} тіків";
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }

}
