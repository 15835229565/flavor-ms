using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Flavor.Controls;
using System.Collections.Generic;
using Flavor.Common.Data.Measure;

namespace Flavor.Forms {
    partial class MonitorForm: GraphForm, IMeasured {
        const string FORM_TITLE = "Режим мониторинга";
        const string X_AXIS_TITLE = "Итерации";
        const string X_AXIS_TIME_TITLE = "Время, мин.";
        const string Y_AXIS_RELATIVE = " (отн.)";
        const string NORM_ITEM_TEXT = "Нормировать";
        const string PEAK_NORM_ITEM_TEXT = "Нормировать на пик № ";
        const string TIME_ITEM_TEXT = "Шкала времени";
        const string POINT_TOOLTIP_FORMAT = "итерация={0:G},счеты={1:F0}";

        class PointPairListPlusWithMaxCapacity: PointPairListPlus {
            // TODO: mix with ZedGraph.RollingPointPairList
            const int MAX_CAPACITY = int.MaxValue;
            public PointPairListPlusWithMaxCapacity()
                : base() { }
            public PointPairListPlusWithMaxCapacity(PointPairListPlus other)
                : base(other, other.PEDreference, other.PLSreference) { }
            public new void Add(PointPair pp) {
                if (Count == MAX_CAPACITY) {
                    RemoveAt(0);
                }
                base.Add(pp);
            }
        }
        interface ISpecial {
            double X { get; set; }
            double Y { get; set; }
            double[] Xs { get; }
            double[] Ys { get; }
        }
        class PointPairSpecial: PointPair, ISpecial {
            readonly double[] xs, ys;
            readonly Func<int> xChooser, yChooser;
            public PointPairSpecial(PointPair pp, double[] extraXs, Func<int> xChooser, double[] extraYs, Func<int> yChooser)
                : base(pp) {
                xs = extraXs;
                ys = extraYs;
                this.xChooser = xChooser;
                this.yChooser = yChooser;
            }
            PointPairSpecial(PointPairSpecial other) {
                xs = (double[])other.xs.Clone();
                ys = (double[])other.ys.Clone();
                xChooser = (Func<int>)other.xChooser.Clone();
                yChooser = (Func<int>)other.yChooser.Clone();
            }
            public override double X {
                get {
                    int index = xChooser();
                    if (index == -1)
                        return base.X;
                    return xs[index];
                }
                set {
                    int index = xChooser();
                    if (index == -1)
                        base.X = value;
                    else
                        xs[index] = value;
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
                    int index = yChooser();
                    if (index == -1)
                        base.Y = value;
                    else
                        ys[index] = value;
                }
            }
            public override PointPair Clone() {
                return new PointPairSpecial(this);
            }

            #region ISpecial Members
            double ISpecial.X {
                get { return base.X; }
                set { base.X = value; }
            }
            double ISpecial.Y {
                get { return base.Y; }
                set { base.Y = value; }
            }
            double[] ISpecial.Xs { get { return xs; } }
            double[] ISpecial.Ys { get { return ys; } }
            #endregion
        }

        long iteration = -1;
        List<PointPairListPlusWithMaxCapacity> list;
        List<long> sums;
        int rowsCount;
        int normPeakNumber = -1;
        enum YAxisState {
            None = 0,
            Normalized = 1,
            PeakNormalized
        }
        YAxisState _showNormalized = YAxisState.None;
        YAxisState ShowNormalized {
            get { return _showNormalized; }
            set {
                if (_showNormalized != value) {
                    _showNormalized = value;
                    graph.GraphPane.YAxis.Title.Text = value != YAxisState.None ? Y_AXIS_TITLE + Y_AXIS_RELATIVE : Y_AXIS_TITLE;
                    // TODO: extract method
                    graph.AxisChange();
                    graph.Refresh();
                }
            }
        }
        bool _useTimeScale = false;
        bool UseTimeScale {
            get { return _useTimeScale; }
            set {
                if (_useTimeScale != value) {
                    _useTimeScale = value;
                    graph.GraphPane.XAxis.Title.Text = UseTimeScale ? X_AXIS_TIME_TITLE : X_AXIS_TITLE;
                    // TODO: extract method
                    graph.AxisChange();
                    graph.Refresh();
                }
            }
        }
        DateTime start = DateTime.MaxValue;

        public MonitorForm() {
            // Init panel before ApplyResources
            Panel = new PreciseMeasureGraphPanel { Graph = Graph.MeasureGraph.Instance };
            InitializeComponent();
        }
        [Obsolete]
        protected override sealed void CreateGraph() {
            ZedGraphRebirth(FORM_TITLE, 0);
        }

        int XScale() {
            return UseTimeScale ? 0 : -1;
        }
        int YScale() {
            switch (ShowNormalized) {
                case YAxisState.Normalized:
                    return 0;
                case YAxisState.PeakNormalized:
                    return 1;
                default:
                    return -1;
            }
        }
        protected override sealed void RefreshGraph() {
            if (iteration > -1) {
                double time = (Graph.MeasureGraph.Instance.DateTime - start).TotalMinutes;

                long sum = 0;
                var temp = new PointPairList();
                for (int i = 0; i < rowsCount; ++i) {
                    var pls = list[i].PLSreference;
                    long peakSum = pls == null ? 0 : pls.PeakSum;
                    sum += peakSum;
                    var pp = new PointPair(iteration, peakSum);
                    temp.Add(pp);
                }
                sums.Add(sum);
                // TODO: proper normalization on selected peak
                double normalization = sum;

                for (int i = 0; i < rowsCount; ++i) {
                    var pp = temp[i];
                    var pp2 = new PointPairSpecial(pp, new[] { time }, XScale, new[] { pp.Y / sum, pp.Y / normalization }, YScale);
                    list[i].Add(pp2);
                }
                graph.GraphPane.XAxis.Scale.Min = list[0][0].X;
                // TODO: extract method?
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
            myPane.YAxis.Title.Text = ShowNormalized != YAxisState.None ? Y_AXIS_TITLE + Y_AXIS_RELATIVE : Y_AXIS_TITLE;
            myPane.XAxis.Title.Text = UseTimeScale ? X_AXIS_TIME_TITLE : X_AXIS_TITLE;
            myPane.CurveList.Clear();

            for (int i = 0; i < list.Count; ++i) {
                var l = list[i];
                string comment = l.PEDreference.Comment;
                var temp = myPane.AddCurve(comment, l, rowsColors[i % rowsColors.Length], SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }

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
            // TODO: extract method?
            graph.AxisChange();
        }

        void NewIterationAsync(object sender, EventArgs e) {
            BeginInvoke(new Action(() => {
                RefreshGraph();
                ++iteration;
            }));
        }
        void InvokeRefreshGraph(ushort pnt, uint[] counts, params int[] recreate) {
            BeginInvoke(new Action(() => refreshGraphicsOnMeasureStep(pnt, counts)));
        }
        void refreshGraphicsOnMeasureStep(ushort pnt, uint[] counts) {
            var panel = (MeasureGraphPanel)Panel;
            panel.performStep(pnt, counts);
        }
        #region IMeasured Members
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            MeasureCancelRequested.Raise(this, EventArgs.Empty);
        }
        public void initMeasure(int progressMaximum, bool isPrecise) {
            list = new List<PointPairListPlusWithMaxCapacity>();
            sums = new List<long>();
            var g = Graph.MeasureGraph.Instance;
            var pspec = g.PreciseData.getUsed();
            rowsCount = pspec.Count;
            for (int i = 0; i < rowsCount; ++i) {
                var temp = new PointPairListPlusWithMaxCapacity();
                pspec[i].AssociatedPoints = temp;
                list.Add(temp);
            }
            
            iteration = 0;

            var panel = (MeasureGraphPanel)Panel;
            panel.MeasureCancelRequested += MonitorForm_MeasureCancelRequested;

            g.GraphDataModified += NewIterationAsync;
            g.NewGraphData += InvokeRefreshGraph;
            panel.ProgressMaximum = progressMaximum;
            panel.Enable();

            if (progressMaximum > 0) {
                // only iterations limit
                UseTimeScale = false;
            } else if (progressMaximum < 0) {
                // time limit or combined
                UseTimeScale = true;
            } else {
                // no limit
                UseTimeScale = false;
            }
            ZedGraphRebirth(FORM_TITLE, progressMaximum);
            
            Show();
            Activate();

            start = DateTime.Now;
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            var g = Graph.MeasureGraph.Instance;
            g.GraphDataModified -= NewIterationAsync;
            g.NewGraphData -= InvokeRefreshGraph;
            iteration = -1;
            start = DateTime.MaxValue;
        }
        #endregion
        void MonitorForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            ((MeasureGraphPanel)Panel).MeasureCancelRequested -= MonitorForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }
        void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ZedGraphControlMonitor.ContextMenuBuilderEventArgs args) {
            if (sender is ZedGraphControlMonitor) {
                var items = args.MenuStrip.Items;
                items.Add(new ToolStripSeparator());

                var defaultViewItem = new ToolStripMenuItem("Счёты", null,
                    (s, e) => ShowNormalized = YAxisState.None);
                var normViewItem = new ToolStripMenuItem(NORM_ITEM_TEXT, null,
                    (s, e) => ShowNormalized = YAxisState.Normalized);
                var peakNormViewItem = new ToolStripMenuItem(PEAK_NORM_ITEM_TEXT + (normPeakNumber != -1 ? normPeakNumber.ToString() : ""), null,
                    (s, e) => {
                        // TODO: show dialog to select peak number
                        ShowNormalized = YAxisState.PeakNormalized;
                        // TODO: recalculate if number changed 
                    });

                switch (ShowNormalized) {
                    case YAxisState.None:
                        defaultViewItem.Checked = true;
                        break;
                    case YAxisState.Normalized:
                        normViewItem.Checked = true;
                        break;
                    case YAxisState.PeakNormalized:
                        peakNormViewItem.Checked = true;
                        break;
                }

                items.Add(new ToolStripMenuItem("Выбрать Y-шкалу", null, defaultViewItem, normViewItem, peakNormViewItem));

                items.Add(new ToolStripMenuItem(TIME_ITEM_TEXT, null, (s, e) => {
                    UseTimeScale = ((ToolStripMenuItem)s).Checked;
                }) { Checked = UseTimeScale, CheckOnClick = true });
            }
        }

        string ZedGraphControlMonitor_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            var pp = (PointPairSpecial)curve[iPt];
            if (pp == null)
                return "";
            string tooltipData = string.Format(POINT_TOOLTIP_FORMAT, pp.X, pp.Y);
            string comment = ((PointPairListPlus)curve.Points).PEDreference.Comment;
            if (comment != null && comment != "") {
                tooltipData += "\n";
                tooltipData += comment;
            }
            return tooltipData;
        }
    }
}
