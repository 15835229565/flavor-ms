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
            GraphPane.IsFontsScaled = false;
            base.ContextMenuBuilder += ZedGraphControlMonitor_ContextMenuBuilder;
        }
        void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            var item = new ToolStripMenuItem();
            item.Text = LOG_ITEM_TEXT;
            item.Checked = (GraphPane.YAxis.Type == AxisType.Log);
            item.CheckOnClick = true;
            item.CheckedChanged += (s, e) => {
                GraphPane.YAxis.Type = (s as ToolStripMenuItem).Checked ? AxisType.Log : AxisType.Linear;
                this.Refresh();
            };

            menuStrip.Items.Add(item);

            OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip));
        }
    }
}

