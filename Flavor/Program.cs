using System;
using System.Windows.Forms;
using Flavor.Forms;

namespace Flavor {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //suppress russian language user interface
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            mainForm MSControl = new mainForm();
            MSControl.WindowState = FormWindowState.Maximized;
            Application.Run(MSControl);
        }
    }
}