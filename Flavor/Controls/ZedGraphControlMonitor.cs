using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;

namespace Flavor.Controls {
    public partial class ZedGraphControlMonitor: ZedGraphControlMine {
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }
        
        const string LOG_ITEM_TEXT = "Логарифмическая шкала";
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
            var pane = GraphPane;
            var xAxis = pane.XAxis;
            var yAxis = pane.YAxis;
            var grid = xAxis.MinorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 1;
            grid.DashOff = 2;

            grid = yAxis.MinorGrid;
            grid.Color = Color.Gray;
            grid.IsVisible = true;
            grid.DashOn = 1;
            grid.DashOff = 2;

            grid = xAxis.MajorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 10;
            grid.DashOff = 5;

            grid = yAxis.MajorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 10;
            grid.DashOff = 5;
            
            base.ContextMenuBuilder += ZedGraphControlMonitor_ContextMenuBuilder;
        }
        void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            var yAxis = GraphPane.YAxis;
            var item = new ToolStripMenuItem(LOG_ITEM_TEXT) {
                Checked = yAxis.Type == AxisType.Log,
                CheckOnClick = true
            };
            item.CheckedChanged += (s, e) => {
                yAxis.Type = ((ToolStripMenuItem)s).Checked ? AxisType.Log : AxisType.Linear;
                Refresh();
            };

            menuStrip.Items.Add(item);

            OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip));
        }
    }
}

