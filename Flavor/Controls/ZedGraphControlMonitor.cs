using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls
{
    public partial class ZedGraphControlMonitor : ZedGraphControl {
        public ZedGraphControlMonitor() : base() {
            InitializeComponent();
            //GraphPane.YAxis.Type = AxisType.Log;
        }
        private void ZedGraphControlMonitor_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            if (control != this)
                return;
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = "Логарифмическая шкала";
            item.Checked = (GraphPane.YAxis.Type == AxisType.Log);
            item.CheckOnClick = true;
            item.CheckedChanged += new System.EventHandler(LogItemCheckStateChanged);
            menuStrip.Items.Add(item);
        }
        private void LogItemCheckStateChanged(object sender, EventArgs e) {
            GraphPane.YAxis.Type = (sender as ToolStripMenuItem).Checked ? AxisType.Log : AxisType.Linear;
        }
    }
}

