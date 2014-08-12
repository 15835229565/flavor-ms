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
        const string TIME_TOOLTIP_FORMAT = "время={0:F2}, ";
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
            readonly double[] xs, ys;
            readonly Func<int> xChooser, yChooser;
            public PointPairSpecial(double x, double y, double[] extraXs, Func<int> xChooser, double[] extraYs, Func<int> yChooser)
                : base(x, y) {
                xs = extraXs;
                ys = extraYs;
                this.xChooser = xChooser;
                this.yChooser = yChooser;
            }
            PointPairSpecial(PointPairSpecial other)
                : base(((ISpecial)other).X, ((ISpecial)other).Y) {
                xs = other.xs == null ? null : (double[])other.xs.Clone();
                ys = other.ys == null ? null : (double[])other.ys.Clone();
                xChooser = other.xChooser;
                yChooser = other.yChooser;
            }
            public override double X {
                get {
                    if (xChooser == null)
                        return base.X;
                    else {
                        int index = xChooser();
                        if (index == -1)
                            return base.X;
                        return xs[index];
                    }
                }
                set {
                    if (xChooser == null)
                        base.X = value;
                    else {
                        int index = xChooser();
                        if (index == -1)
                            base.X = value;
                        else
                            xs[index] = value;
                    }
                }
            }
            public override double Y {
                get {
                    if (yChooser == null)
                        return base.Y;
                    else {
                        int index = yChooser();
                        if (index == -1)
                            return base.Y;
                        return ys[index];
                    }
                }
                set {
                    if (yChooser == null)
                        base.Y = value;
                    else {
                        int index = yChooser();
                        if (index == -1)
                            base.Y = value;
                        else
                            ys[index] = value;
                    }
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
        readonly object _locker = new object();
        List<PointPairListPlusWithMaxCapacity> list;
        List<PreciseEditorData> pspec;
        int _normPeakNumber = -1;
        int NormPeakNumber {
            get { return _normPeakNumber; }
            set {
                if (_normPeakNumber != value) {
                    _normPeakNumber = value;
                    RecountNormalization(value);
                }
            }
        }
        void RecountNormalization(int n) {
            if (n == -1 || pspec.Count <= 1)
                return;

            var l = list[n];
            int count;
            lock (_locker) {
                count = l.Count - 1;
                // TODO: form here latest averaging data. lock may start earlier.
            }
            //old data is recounting here. any new data (if appeared) will use already new peak number
            double normalization;
            // may be problem if list rolls during operation
            for (int i = count; i >= 0; --i) {
                normalization = ((ISpecial)l[i]).Y;
                foreach (var row in list) {
                    var p = (ISpecial)row[i];
                    // TODO: implement averaging
                    p.Ys[1] = p.Y / normalization;
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
                    var pane = graph.GraphPane;
                    pane.XAxis.Title.Text = UseTimeScale ? X_AXIS_TIME_TITLE : X_AXIS_TITLE;
                    pane.GraphObjList.Clear();
                    foreach (var pp in labels) {
                        AddLabel(pp.X, (int)pp.Y);
                    }
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
        readonly List<PointPairSpecial> labels = new List<PointPairSpecial>();
        public void AddLabel(int n) {
            double time = (DateTime.Now - start).TotalMinutes;
            var pp = new PointPairSpecial(iteration + 0.5, n, new[] { time }, XScale, null, null);
            labels.Add(pp);
            AddLabel(pp.X, n);
            graph.Refresh();
        }
        void AddLabel(double x, int n) {
            var pane = graph.GraphPane;
            var scale = pane.YAxis.Scale;

            var line = new LineObj(x, scale.Min, x, scale.Max);
            var l = line.Line;
            l.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            l.Color = Color.Red;
            l.Width = 2;
            pane.GraphObjList.Add(line);
            
            // TODO: check text displaying
            var text = new TextObj(n.ToString(), x, scale.Max - 0.1 * (scale.Max - scale.Min));
            var fontSpec = text.FontSpec;
            fontSpec.Border.IsVisible = true;
            fontSpec.FontColor = Color.Red;
            pane.GraphObjList.Add(text);
        }
        [Obsolete]
        protected override sealed void CreateGraph() {
            //ZedGraphRebirth(FORM_TITLE, 0);
            //throw new NotSupportedException();
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

                if (rowsCount == 1) {
                    // no normalization when displaying 1 row
                    long peakSum = 0;
                    // TODO: move PeakSum counting to PointPairListPlus
                    foreach (var p in pspec[0].AssociatedPoints)
                        peakSum += (long)p.Y;
                    list[0].Add(new PointPairSpecial(iteration, peakSum, new[] { time }, XScale, null, null));
                } else {
                    var temp = new PointPairList();
                    long sum = 0;
                    for (int i = 0; i < rowsCount; ++i) {
                        long peakSum = 0;
                        // TODO: move PeakSum counting to PointPairListPlus
                        foreach (var p in pspec[i].AssociatedPoints)
                            peakSum += (long)p.Y;
                        var pp = new PointPair(iteration, peakSum);
                        temp.Add(pp);
                        sum += peakSum;
                    }

                    lock (_locker) {
                        int normPeakNumber = NormPeakNumber;
                        double normalization = normPeakNumber == -1 ? 1 : temp[normPeakNumber].Y;
                        for (int i = 0; i < rowsCount; ++i) {
                            var pp = temp[i];
                            double x = pp.X;
                            double y = pp.Y;
                            // TODO: implement averaging
                            var pp2 = new PointPairSpecial(x, y, new[] { time }, XScale, new[] { y / sum, normPeakNumber != -1 ? y / normalization : 0 }, YScale);
                            list[i].Add(pp2);
                        }
                    }
                }
                //graph.GraphPane.XAxis.Scale.Min = list[0][0].X;
                // TODO: extract method?
                graph.AxisChange();
            }
            graph.Refresh();
        }

        protected override sealed void SetSize() {
            if (graph == null)
                return;
            graph.Location = new Point(HORIZ_GRAPH_INDENT, VERT_GRAPH_INDENT);
            graph.Size = new Size(ClientSize.Width - 2 * HORIZ_GRAPH_INDENT - (Panel.Visible ? Panel.Width : 0), ClientSize.Height - 2 * VERT_GRAPH_INDENT);
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
            graph.Refresh();
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
            var g = Graph.MeasureGraph.Instance;
            pspec = g.PreciseData.getUsed();
            foreach (var ped in pspec) {
                var temp = new PointPairListPlusWithMaxCapacity();
                temp.PEDreference = ped;
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
            var items = args.MenuStrip.Items;
            items.Add(new ToolStripSeparator());
            if (pspec.Count > 1) {
                // no normalization when displaying 1 row
                var defaultViewItem = new ToolStripMenuItem(COUNTS_ITEM_TEXT, null,
                    (s, e) => ShowNormalized = YAxisState.None);
                var normViewItem = new ToolStripMenuItem(NORM_ITEM_TEXT, null,
                    (s, e) => ShowNormalized = YAxisState.Normalized);
                int n = NormPeakNumber;
                var peakNormViewItem = new ToolStripMenuItem(PEAK_NORM_ITEM_TEXT + (n != -1 ? pspec[n].pNumber.ToString() : ""), null,
                    (s, e) => {
                        var form = new SetNormalizationPeakForm();
                        form.Load += (ss, ee) => {
                            var eee = ((SetNormalizationPeakForm.LoadEventArgs)ee);
                            eee.NormPeakNumber = n;
                            eee.PeakList = pspec.ConvertAll(ped => { return ped.pNumber.ToString() + " " + ped.Comment; }).ToArray();
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

        void graph_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState) {
            if (sender != graph)
                return;
            // TODO: check actually zoom is changed
            graph.GraphPane.GraphObjList.Clear();
            foreach (var pp in labels) {
                AddLabel(pp.X, (int)pp.Y);
            }
        }
    }
}
