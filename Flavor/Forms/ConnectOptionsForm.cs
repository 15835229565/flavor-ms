using System;
using System.Windows.Forms;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class ConnectOptionsForm: Form {
        public ConnectOptionsForm(string[] ports) {
            InitializeComponent();
            serialPortComboBox.Text = Config.Port;
            baudrateComboBox.Text = Config.BaudRate.ToString();
            serialPortComboBox.Items.AddRange(ports);
        }

        void cancel_butt_Click(object sender, EventArgs e) {
            Close();
        }

        void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalConnectOptions(serialPortComboBox.Text, int.Parse(baudrateComboBox.Text));
            Close();
        }
    }
}