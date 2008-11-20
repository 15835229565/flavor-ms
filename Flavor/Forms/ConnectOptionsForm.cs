using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;

namespace Flavor
{
    public partial class ConnectOptionsForm : Form
    {
        private string Port
        {
            get { return serialPort_comboBox.Text.ToString(); }
        }
        
        private ushort BaudRate
        {
            get { return ushort.Parse(baudrate_comboBox.Text); }
        }
        
        public ConnectOptionsForm()
        {
            InitializeComponent();
            serialPort_comboBox.Text = Config.Port;
            baudrate_comboBox.Text = Config.BaudRate.ToString();
            foreach (string sp in SerialPort.GetPortNames()) serialPort_comboBox.Items.Add(sp);
        }

        private void cancel_butt_Click(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ok_butt_Click(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.OK;
            ModBus.Close();
            Config.SaveConnectOptions(Port, BaudRate);
            ModBus.Open();
            this.Close();
        }
    }
}