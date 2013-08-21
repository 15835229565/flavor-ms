using System;
using System.Windows.Forms;

namespace Analyzer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            (new TestClass2()).doWork();
            Application.Run(new MainForm());
        }
    }
}
