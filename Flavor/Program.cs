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

            // TODO: load config to use proper Commander and set proper form parameters
            Config.getInitialDirectory();
            EventHandler onLoad = null;
            try {
                Config.loadGlobalConfig();
            } catch (Config.ConfigLoadException cle) {
                onLoad = (s, e) => cle.visualise();
            }

            var commander = new AlmazovCommander();
            //var commander = new SevMorGeoCommader();
            ConsoleWriter.Subscribe(commander);

            MainForm2 MSControl = new MainForm2(commander);
            MSControl.Load += onLoad;
            MSControl.WindowState = FormWindowState.Maximized;

            commander.Bind(MSControl);

            Application.Run(MSControl);
        }
    }
}