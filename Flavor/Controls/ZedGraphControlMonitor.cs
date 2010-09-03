using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls {
    public partial class ZedGraphControlMonitor: ZedGraphControl {
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
            //this.GraphPane.YAxis.Type = AxisType.Log;
        }
    }
}

