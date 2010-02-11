using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor.Forms
{
    internal partial class MonitorOptionsForm : PreciseOptionsForm
    {
        private MonitorOptionsForm()
        {
            InitializeComponent();
        }

        private static MonitorOptionsForm instance = null;
        internal new static MonitorOptionsForm getInstance()
        {
            if (instance == null) instance = new MonitorOptionsForm();
            return instance;
        }

        private void MonitorOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }
    }
}

