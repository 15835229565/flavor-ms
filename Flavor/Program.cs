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
            mainForm MSControl = new mainForm();
            MSControl.WindowState = FormWindowState.Maximized;
            Application.Run(MSControl);
        }
    }
}