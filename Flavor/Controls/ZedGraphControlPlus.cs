using System;
using System.Drawing;
using System.Windows.Forms;

using ZedGraph;
using PointPairListPlus = Flavor.Common.Data.Measure.PointPairListPlus;

namespace Flavor.Controls {
    partial class ZedGraphControlPlus: ZedGraphControlMine {
        public new class ContextMenuBuilderEventArgs: ZedGraphControlMine.ContextMenuBuilderEventArgs {
            public PointPairListPlus Row { get; private set; }
            public int Index { get; private set; }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip, PointPairListPlus ppl, int index)
                : base (menuStrip) {
                Row = ppl;
                Index = index;
            }
        }
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;// = delegate { }; // cannot be null, important for thread safety;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }

        public ZedGraphControlPlus()
            : base() {
            InitializeComponent();
            Dock = DockStyle.Fill;
            // TODO: move something to superclass?
            var pane = GraphPane;
            pane.Title.IsVisible = false;
            pane.Margin.All = 0;
            pane.Margin.Top = 10;
            pane.XAxis.Title.FontSpec.Size = 12;
            pane.YAxis.Title.FontSpec.Size = 12;

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
            if (pane != null && pane.FindNearestPoint(mousePt, out nearestCurve, out pointIndex))
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, (PointPairListPlus)nearestCurve.Points, pointIndex));
            else
                OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip, null, -1));
        }
    }
}

