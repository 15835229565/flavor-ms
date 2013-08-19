using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;
using Flavor.Common;

namespace Flavor.Controls
{
    public partial class ZedGraphControlMonitor: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            private ContextMenuStrip menuStrip;
            public ContextMenuStrip MenuStrip {
                get {
                    return menuStrip;
                }
            }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip) {
                this.menuStrip = menuStrip;
            }
        }
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }
        
        private const string LOG_ITEM_TEXT = "Логарифмическая шкала";
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
            base.ContextMenuBuilder += ZedGraphControlMonitor_ContextMenuBuilder;
        }

        private void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = LOG_ITEM_TEXT;
            item.Checked = (GraphPane.YAxis.Type == AxisType.Log);
            item.CheckOnClick = true;
            item.CheckedChanged += new System.EventHandler((s, e) => {
                GraphPane.YAxis.Type = (s as ToolStripMenuItem).Checked ? AxisType.Log : AxisType.Linear;
                this.Refresh();
            });

            menuStrip.Items.Add(item);

            OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip));
        }
    }
}

