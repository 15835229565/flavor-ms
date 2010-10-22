using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls
{
    public partial class ZedGraphControlMonitor: ZedGraphControl {
        private const string LOG_ITEM_TEXT = "Логарифмическая шкала";
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
        }
        internal void CommonContextMenuBuilder(ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = LOG_ITEM_TEXT;
            item.Checked = (GraphPane.YAxis.Type == AxisType.Log);
            item.CheckOnClick = true;
            item.CheckedChanged += new System.EventHandler(LogItemCheckStateChanged);

            menuStrip.Items.Add(item);
        }
        private void LogItemCheckStateChanged(object sender, EventArgs e) {
            GraphPane.YAxis.Type = (sender as ToolStripMenuItem).Checked ? AxisType.Log : AxisType.Linear;
            this.Refresh();
        }
    }
}

