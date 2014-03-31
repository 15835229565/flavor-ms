using System;
using System.Windows.Forms;
using Flavor.Forms;
using Flavor.Common;

namespace Flavor {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // TODO: change application type in Flavor properties to hide console
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //suppress russian language user interface
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            //mainForm MSControl = new mainForm();
            //Commander.setProgramStateWithoutUndo(ProgramStates.Start);
            //MSControl.WindowState = FormWindowState.Maximized;
            var commander = new Commander2();
            MainForm2 MSControl2 = new MainForm2(commander);
            //commander.setProgramStateWithoutUndo(ProgramStates.Start);
            MSControl2.WindowState = FormWindowState.Maximized;
            //Application.Run(MSControl);
            Application.Run(MSControl2);
        }
    }
}