using System;
using System.Drawing;
using System.Linq;
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
        const string Y_SCALE_ITEM_TEXT = "Выбрать Y-шкалу";
        const string COUNTS_ITEM_TEXT = "Счёты";
        const string NORM_ITEM_TEXT = "Нормировать";
        const string PEAK_NORM_ITEM_TEXT = "Нормировать на пик № ";
        const string TIME_ITEM_TEXT = "Шкала времени";
        const string ITERATION_TOOLTIP_FORMAT = "итерация={0:G}, ";
        const string TIME_TOOLTIP_FORMAT = "время={0:F1}, ";
        const string POINT_TOOLTIP_FORMAT = "счеты={1:F0}";
        const string NORMALIZED_POINT_TOOLTIP_FORMAT = "{1:G}";

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
            double _x, _y;
            readonly double[] xs, ys;
            readonly Func<int> xChooser, yChooser;
            public PointPairSpecial(PointPair pp, double[] extraXs, Func<int> xChooser, double[] extraYs, Func<int> yChooser) {
                _x = pp.X;
                _y = pp.Y;
                xs = extraXs;
                ys = extraYs;
                this.xChooser = xChooser;
                this.yChooser = yChooser;
            }
            PointPairSpecial(PointPairSpecial other) {
                _x = other._x;
                _y = other._y;
                xs = (double[])other.xs.Clone();
                ys = (double[])other.ys.Clone();
                xChooser = (Func<int>)other.xChooser.Clone();
                yChooser = (Func<int>)other.yChooser.Clone();
            }
            public override double X {
                get {
                    int index = xChooser == null ? -1 : xChooser();
                    if (index == -1)
                        return _x;
                    return xs[index];
                }
                set {
                    int index = xChooser == null ? -1 : xChooser();
                    if (index == -1)
                        _x = value;
                    else
                        xs[index] = value;
                }
            }
            public override double Y {
                get {
                    int index = yChooser == null ? -1 : yChooser();
                    if (index == -1)
                        return _y;
                    return ys[index];
                }
                set {
                    int index = yChooser == null ? -1 : yChooser();
                    if (index == -1)
                        _y = value;
                    else
                        ys[index] = value;
                }
            }
            public override PointPair Clone() {
                return new PointPairSpecial(this);
            }

            #region ISpecial Members
            double ISpecial.X {
                get { return _x; }
                set { _x = value; }
            }
            double ISpecial.Y {
                get { return _y; }
                set { _y = value; }
            }
            double[] ISpecial.Xs { get { return xs; } }
            double[] ISpecial.Ys { get { return ys; } }
            #endregion
        }

        long iteration = -1;
        List<PointPairListPlusWithMaxCapacity> list;
        List<long> sums;
        List<PreciseEditorData> pspec;
        int _normPeakNumber = -1;
        int NormPeakNumber {
            get { return _normPeakNumber; }
            set {
                if (_normPeakNumber != value) {
                    _normPeakNumber = value;
                    RecountNormalization();
                }
            }
        }
        void RecountNormalization() {
            if (NormPeakNumber == -1 || pspec.Count <= 1)
                return;

            double normalization;
            var l = list[NormPeakNumber];
            for (int i = 0; i < l.Count; ++i) {
                normalization = ((ISpecial)l[i]).Y;
                foreach (var row in list) {
                    var p = (ISpecial)row[i];
                    p.Ys[2] = p.Y / normalization;
                }
            }
        }
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
            throw new NotSupportedException();
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
                int rowsCount = pspec.Count;

                long sum = 0;
                double normalization = 0;
                var temp = new PointPairList();
                for (int i = 0; i < rowsCount; ++i) {
                    long peakSum = 0;
                    foreach (var p in pspec[i].AssociatedPoints)
                        peakSum += (long)p.Y;
                    var pp = new PointPair(iteration, peakSum);
                    temp.Add(pp);
                    if (rowsCount > 1) {
                        // no normalization when displaying 1 row
                        sum += peakSum;
                        if (i == NormPeakNumber)
                            // TODO: implement averaging
                            normalization = peakSum;
                    }
                }
                // remove (no need)
                sums.Add(sum);

                for (int i = 0; i < rowsCount; ++i) {
                    var pp = temp[i];
                    list[i].Add(rowsCount > 1 ?
                        new PointPairSpecial(pp, new[] { time }, XScale, new[] { pp.Y / sum, NormPeakNumber != -1 ? pp.Y / normalization : 0 }, YScale) :
                        // no normalization when displaying 1 row
                        new PointPairSpecial(pp, new[] { time }, XScale, new double[] { }, YScale));
                }
                graph.GraphPane.XAxis.Scale.Min = list[0][0].X;
                // TODO: extract method?
                graph.AxisChange();
            }
            graph.Refresh();
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
                UseTimeScale = true;
                xScale.Min = 0;
                xScale.Max = -xMax;
                xScale.MaxAuto = false;
            }
            // TODO: extract method?
            graph.AxisChange();
        }

        void NewIterationAsync(object sender, EventArgs<int[]> e) {
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
            pspec = g.PreciseData.getUsed();
            for (int i = 0; i < pspec.Count; ++i) {
                var temp = new PointPairListPlusWithMaxCapacity();
                temp.PEDreference = pspec[i];
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
                if (pspec.Count > 1) {
                    // no normalization when displaying 1 row
                    var defaultViewItem = new ToolStripMenuItem(COUNTS_ITEM_TEXT, null,
                        (s, e) => ShowNormalized = YAxisState.None);
                    var normViewItem = new ToolStripMenuItem(NORM_ITEM_TEXT, null,
                        (s, e) => ShowNormalized = YAxisState.Normalized);
                    var peakNormViewItem = new ToolStripMenuItem(PEAK_NORM_ITEM_TEXT + (NormPeakNumber != -1 ? NormPeakNumber.ToString() : ""), null,
                        (s, e) => {
                            var form = new SetNormalizationPeakForm();
                            form.Load += (ss, ee) => {
                                var eee = ((SetNormalizationPeakForm.LoadEventArgs)ee);
                                eee.NormPeakNumber = NormPeakNumber;
                                eee.PeakList = pspec.ConvertAll(ped => { return ped.pNumber.ToString() + ped.Comment; }).ToArray();
                            };
                            form.FormClosing += (ss, ee) => {
                                var result = ((SetNormalizationPeakForm)ss).DialogResult;
                                if (result == DialogResult.OK) {
                                    var eee = (SetNormalizationPeakForm.ClosingEventArgs)ee;
                                    NormPeakNumber = eee.NormPeakNumber;
                                }
                            };
                            if (form.ShowDialog() == DialogResult.OK) {
                                ShowNormalized = YAxisState.PeakNormalized;
                            }
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

                    items.Add(new ToolStripMenuItem(Y_SCALE_ITEM_TEXT, null, defaultViewItem, normViewItem, peakNormViewItem));
                }
                items.Add(new ToolStripMenuItem(TIME_ITEM_TEXT, null, (s, e) => {
                    UseTimeScale = ((ToolStripMenuItem)s).Checked;
                }) { Checked = UseTimeScale, CheckOnClick = true });
            }
        }

        string ZedGraphControlMonitor_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            var pp = (PointPairSpecial)curve[iPt];
            if (pp == null)
                return "";
            string tooltipData = string.Format((UseTimeScale ? TIME_TOOLTIP_FORMAT : ITERATION_TOOLTIP_FORMAT) +
                (ShowNormalized == YAxisState.None ? POINT_TOOLTIP_FORMAT : NORMALIZED_POINT_TOOLTIP_FORMAT), pp.X, pp.Y);
            string comment = ((PointPairListPlus)curve.Points).PEDreference.Comment;
            if (comment != null && comment != "") {
                tooltipData += "\n";
                tooltipData += comment;
            }
            return tooltipData;
        }
    }
}
