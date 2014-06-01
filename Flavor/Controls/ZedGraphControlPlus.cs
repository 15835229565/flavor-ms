using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;
using Flavor.Common;
using PointPairListPlus = Flavor.Common.Data.Measure.PointPairListPlus;

namespace Flavor.Controls {
    partial class ZedGraphControlPlus: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            private ContextMenuStrip menuStrip;
            public ContextMenuStrip MenuStrip {
                get {
                    return menuStrip;
                }
            }
            PointPairListPlus ppl;
            public PointPairListPlus Row {
                get {
                    return ppl;
                }
            }
            int index;
            public int Index {
                get {
                    return index;
                }
            }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip, PointPairListPlus ppl, int index) {
                this.menuStrip = menuStrip;
                this.ppl = ppl;
                this.index = index;
            }
        }
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;// = delegate { }; // cannot be null, important for thread safety;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }

        public ZedGraphControlPlus()
            : base() {
            InitializeComponent();
            base.ContextMenuBuilder += ZedGraphControlPlus_ContextMenuBuilder;
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            if (sender != this)
                return;
            GraphPane pane = MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            int pointIndex;
            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out pointIndex))
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, nearestCurve.Points as PointPairListPlus, pointIndex));
            else
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, null, -1));
        }
    }
}

