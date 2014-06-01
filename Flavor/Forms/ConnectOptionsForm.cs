using System;
using System.Windows.Forms;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    internal partial class ConnectOptionsForm: Form {
        internal ConnectOptionsForm(string[] ports) {
            InitializeComponent();
            serialPortComboBox.Text = Config.Port;
            baudrateComboBox.Text = Config.BaudRate.ToString();
            serialPortComboBox.Items.AddRange(ports);
        }

        private void cancel_butt_Click(object sender, EventArgs e) {
            Close();
        }

        private void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalConnectOptions(serialPortComboBox.Text, uint.Parse(baudrateComboBox.Text));
            Close();
        }
    }
}