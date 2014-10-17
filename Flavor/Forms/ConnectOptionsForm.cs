using System;
using System.Windows.Forms;
using Config = Flavor.Common.Config;
//?
using Commander = Flavor.Common.Commander;

namespace Flavor.Forms {
    internal partial class ConnectOptionsForm: Form {
        internal ConnectOptionsForm() {
            InitializeComponent();
            serialPortComboBox.Text = Config.Port;
            baudrateComboBox.Text = Config.BaudRate.ToString();
            serialPortComboBox.Items.AddRange(Commander.AvailablePorts);
        }

        private void cancel_butt_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalConnectOptions(serialPortComboBox.Text, ushort.Parse(baudrateComboBox.Text));
            Commander.reconnect();
            this.Close();
        }
    }
}