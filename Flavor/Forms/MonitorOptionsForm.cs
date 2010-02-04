using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public partial class MonitorOptionsForm : Flavor.PreciseOptionsForm
    {
        private MonitorOptionsForm()
        {
            InitializeComponent();
        }

        private static MonitorOptionsForm instance = null;
        public new static MonitorOptionsForm getInstance()
        {
            if (instance == null) instance = new MonitorOptionsForm();
            return instance;
        }
    }
}

