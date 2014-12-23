using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;
using PointPairListPlus = Flavor.Common.Data.Measure.PointPairListPlus;

namespace Flavor.Controls {
    partial class ZedGraphControlPlus: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            ContextMenuStrip menuStrip;
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
            var pane = GraphPane;
            pane.IsFontsScaled = false;
            pane.Legend.IsVisible = false;
            // Fill the axis background with a color gradient
            pane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            // Fill the pane background with a color gradient
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
            base.ContextMenuBuilder += ZedGraphControlPlus_ContextMenuBuilder;
        }

        void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            if (sender != this)
                return;
            var pane = MasterPane.FindChartRect(mousePt);
            // TODO: different menus for different objects (point, axis..)
            //pane.FindNearestObject(mousePt, graphics, obj, index);
            CurveItem nearestCurve;
            int pointIndex;
            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out pointIndex))
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, (PointPairListPlus)nearestCurve.Points, pointIndex));
            else
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, null, -1));
        }
    }
}

