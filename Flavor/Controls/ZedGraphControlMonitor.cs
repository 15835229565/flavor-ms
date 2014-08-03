using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;

namespace Flavor.Controls {
    public partial class ZedGraphControlMonitor: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            public ContextMenuStrip MenuStrip { get; private set; }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip) {
                MenuStrip = menuStrip;
            }
        }
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }
        
        const string LOG_ITEM_TEXT = "Логарифмическая шкала";
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
            var pane = GraphPane;
            pane.IsFontsScaled = false;
            pane.Legend.IsVisible = false;
            pane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
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

