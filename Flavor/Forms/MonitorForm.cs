using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Flavor.Controls;
using System.Collections.Generic;
using Graph = Flavor.Common.Data.Measure.Graph;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
using PointPairListPlus = Flavor.Common.Data.Measure.PointPairListPlus;

namespace Flavor.Forms {
    partial class MonitorForm: GraphForm, IMeasured {
        const string FORM_TITLE = "Режим мониторинга";
        const string X_AXIS_TITLE = "Итерации";
        const string Y_AXIS_RELATIVE = " (отн.)";
        const string NORM_ITEM_TEXT = "Нормировать";
        const string POINT_TOOLTIP_FORMAT = "итерация={0:G},счеты={1:F0}";

        long iteration = -1;
        class PointPairListPlusWithMaxCapacity: PointPairListPlus {
            // TODO: mix with ZedGraph.RollingPointPairList
            const int MAX_CAPACITY = int.MaxValue;
            public PointPairListPlusWithMaxCapacity()
                : base() { }
            public PointPairListPlusWithMaxCapacity(PointPairListPlus other)
                : base(other, other.PEDreference, other.PLSreference) { }
            public new void Add(PointPair pp) {
                base.Add(pp);
                if (base.Count > MAX_CAPACITY) {
                    base.RemoveAt(0);
                }
            }
        }
        class PointPairSpecial: PointPair {
            readonly double[] xs, ys;
            readonly Generator<int> xChooser, yChooser;
            public PointPairSpecial(PointPair pp, double[] extraXs, Generator<int> xChooser, double[] extraYs, Generator<int> yChooser)
                : base(pp) {
                xs = extraXs;
                ys = extraYs;
                this.xChooser = xChooser;
                this.yChooser = yChooser;
            }
            public override double X {
                get {
                    int index = xChooser();
                    if (index == -1)
                        return base.X;
                    return xs[index];
                }
                set {
                    base.X = value;
                }
            }
            public override double Y {
                get {
                    int index = yChooser();
                    if (index == -1)
                        return base.Y;
                    return ys[index];
                }
                set {
                    base.Y = value;
                }
            }
        }
        
        List<PointPairListPlusWithMaxCapacity> list;
        int rowsCount;
        List<long> sums;
        bool showNormalized = false;

        public MonitorForm() {
            // Init panel before ApplyResources
            Panel = new PreciseMeasureGraphPanel();
            Panel.Graph = Graph.MeasureGraph.Instance;
            InitializeComponent();
        }

        protected override sealed void CreateGraph() {
            ZedGraphRebirth(FORM_TITLE, 0);
        }

        int XScale() {
            // TODO:
            return -1;
        }
        int YScale() {
            return showNormalized ? 0 : -1;
        }
        protected override sealed void RefreshGraph() {
            if (iteration > -1) {
                var g = Graph.MeasureGraph.Instance;
                //BAD: every time
                // TODO: use getUsed()
                var pspec = g.PreciseData.FindAll(PreciseEditorData.PeakIsUsed);
                if (pspec.Count != rowsCount)
                    // very bad!
                    throw new NullReferenceException("Count mismatch");

                // TODO: use to form extra data row
                var dt = g.DateTime;

                long sum = 0;
                var temp = new PointPairList();
                for (int i = 0; i < rowsCount; ++i) {
                    var ped = pspec[i];
                    long peakSum = ped.AssociatedPoints == null ? 0 :
                        (ped.AssociatedPoints.PLSreference == null ? 0 :
                        ped.AssociatedPoints.PLSreference.PeakSum);
                    sum += peakSum;
                    var pp = new PointPair(iteration, peakSum);
                    temp.Add(pp);
                }
                sums.Add(sum);
                for (int i = 0; i < rowsCount; ++i) {
                    var pp = temp[i];
                    // TODO: use datetime for extra X
                    var pp2 = new PointPairSpecial(pp, new double[] { }, XScale, new double[] { pp.Y / sum }, YScale);
                    list[i].Add(pp2);
                }
                graph.GraphPane.XAxis.Scale.Min = list[0][0].X;
                graph.AxisChange();
            }
            graph.Refresh();
        }

        protected override bool saveData() {
            return base.saveData();
        }

        protected override sealed void SetSize() {
            if (graph == null)
                return;
            graph.Location = new Point(HORIZ_GRAPH_INDENT, VERT_GRAPH_INDENT);
            graph.Size = new Size(ClientSize.Width - (2 * HORIZ_GRAPH_INDENT) - (Panel.Visible ? Panel.Width : 0), (ClientSize.Height - (2 * VERT_GRAPH_INDENT)));
        }

        void ZedGraphRebirth(string title, int xMax) {
            var myPane = graph.GraphPane;

            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = !showNormalized ? Y_AXIS_TITLE : Y_AXIS_TITLE + Y_AXIS_RELATIVE;
            myPane.XAxis.Title.Text = X_AXIS_TITLE;
            // TODO: clear only on init, not on rescaling
            myPane.CurveList.Clear();

            for (int i = 0; i < list.Count; ++i) {
                var l = list[i];
                string comment = l.PEDreference.Comment;
                var temp = myPane.AddCurve(comment, l, rowsColors[i % rowsColors.Length], SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }
            graph.Refresh();

            myPane.Legend.IsVisible = false;
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
            var yScale = myPane.YAxis.Scale;
            yScale.Min = 0;
            yScale.Max = 10000;
            yScale.MaxAuto = true;
            var xScale = myPane.XAxis.Scale;
            if (xMax == 0) {
                xScale.Min = 0;
                xScale.Max = 10000;
                xScale.MaxAuto = true;
            } else if (xMax > 0) {
                // iterations
                xScale.Min = 0;
                xScale.Max = xMax;
                xScale.MaxAuto = false;
            } else {
                // time
                // TODO: change scale to time
                xScale.Min = 0;
                xScale.Max = -xMax;
                xScale.MaxAuto = false;
            }
            graph.AxisChange();
        }

        void NewIterationAsync(object sender, EventArgs e) {
            BeginInvoke(new Action(() => {
                RefreshGraph();
                ++iteration;
            }));
        }
        void InvokeRefreshGraph(ushort pnt, uint[] counts, params int[] recreate) {
            BeginInvoke(new Action(() => {
                if (iteration == -1 && recreate.Length != 0) {
                    CreateGraph();
                }
                refreshGraphicsOnMeasureStep(pnt, counts);
            }));
        }
        void refreshGraphicsOnMeasureStep(ushort pnt, uint[] counts) {
            var panel = Panel as MeasureGraphPanel;
            panel.performStep(pnt, counts);
        }
        #region IMeasured Members
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            MeasureCancelRequested.Raise(this, EventArgs.Empty);
        }
        public void initMeasure(int progressMaximum, bool isPrecise) {
            if (progressMaximum > 0) {
                // only iterations limit
            } else if (progressMaximum < 0) {
                // time limit or combined
            } else {
                // no limit
            }
            
            list = new List<PointPairListPlusWithMaxCapacity>();
            sums = new List<long>();
            //!!
            // TODO: use extension method getUsed()
            var pspec = Graph.MeasureGraph.Instance.PreciseData.FindAll(PreciseEditorData.PeakIsUsed);
            rowsCount = pspec.Count;
            for (int i = 0; i < rowsCount; ++i) {
                //!!!!!! try to prevent nulls in PLS
                var temp = new PointPairListPlusWithMaxCapacity();
                pspec[i].AssociatedPoints = temp;
                list.Add(temp);
            }
            iteration = 0;
            //if (!showNormalized) {
            //    CreateGraph();
            //} else {
                ZedGraphRebirth(FORM_TITLE, progressMaximum);
            //}

            var panel = Panel as MeasureGraphPanel;
            panel.MeasureCancelRequested += MonitorForm_MeasureCancelRequested;

            var g = Graph.MeasureGraph.Instance;
            g.GraphDataModified += NewIterationAsync;
            g.NewGraphData += InvokeRefreshGraph;
            panel.ProgressMaximum = progressMaximum;
            panel.Enable();
            Show();
            Activate();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            var g = Graph.MeasureGraph.Instance;
            g.GraphDataModified -= NewIterationAsync;
            g.NewGraphData -= InvokeRefreshGraph;
            iteration = -1;
        }
        #endregion
        void MonitorForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            (Panel as MeasureGraphPanel).MeasureCancelRequested -= MonitorForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }
        void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ZedGraphControlMonitor.ContextMenuBuilderEventArgs args) {
            if (sender is ZedGraphControlMonitor) {
                var items = args.MenuStrip.Items;
                var item = new ToolStripMenuItem();
                item.Text = NORM_ITEM_TEXT;
                item.Checked = showNormalized;
                item.CheckOnClick = true;
                item.CheckedChanged += NormItemCheckStateChanged;

                items.Add(item);
            }
        }
        void NormItemCheckStateChanged(object sender, EventArgs e) {
            // TODO: modify graph title
            if (showNormalized) {
                showNormalized = false;
                // TODO:
                CreateGraph();
                graph.Refresh();
                return;
            }
            showNormalized = true;
            ZedGraphRebirth(FORM_TITLE, 0);
            graph.Refresh();
        }

        string ZedGraphControlMonitor_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            // curve contains simple PointPairs
            var pp = curve[iPt];
            string tooltipData;
            tooltipData = string.Format(POINT_TOOLTIP_FORMAT, pp.X, pp.Y);
            //tooltipData = !showNormalized ?
            //                     string.Format(POINT_TOOLTIP_FORMAT, pp.X, pp.Y) :
            //                     string.Format(POINT_TOOLTIP_FORMAT, pp.X, 0/*(curve.Points as PointPairListPlus).PEDreference.AssociatedPoints[iPt].Y*/);//smth strange
            string comment = (curve.Points as PointPairListPlus).PEDreference.Comment;
            var test = curve.Points as PointPairListPlusWithMaxCapacity;
            if (comment != null && comment != "") {
                tooltipData += "\n";
                tooltipData += comment;
            }
            return tooltipData;
        }
    }
}
