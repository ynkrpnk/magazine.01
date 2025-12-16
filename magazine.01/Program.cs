using System;
using System.Windows.Forms;

namespace magazine._01
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Запускаємо форму з вашою назвою
            Application.Run(new log_in());
           // Application.Run(new richTextBoxSearchLog());
        }
    }
}
