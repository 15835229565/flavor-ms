using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Flavor.Controls;
using System.Collections.Generic;
using Graph = Flavor.Common.Graph;
using PreciseEditorData = Flavor.Common.Utility.PreciseEditorData;
using PointPairListPlus = Flavor.Common.PointPairListPlus;

namespace Flavor.Forms
{
    internal partial class MonitorForm: GraphForm, IMeasured {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            if (MeasureCancelRequested != null)
                MeasureCancelRequested(this, EventArgs.Empty);
        }
        private const string FORM_TITLE = "Режим мониторинга";
        private const string X_AXIS_TITLE = "Итерации";
        private const string Y_AXIS_RELATIVE = " (отн.)";
        private const string NORM_ITEM_TEXT = "Нормировать";
        private const string POINT_TOOLTIP_FORMAT = "итерация={0:G},счеты={1:F0}";

        private double time = -1;
        public class PointPairListPlusWithMaxCapacity: PointPairListPlus {
            private const int MAX_CAPACITY = 1000;
            public PointPairListPlusWithMaxCapacity() : base() { }
            public PointPairListPlusWithMaxCapacity(PointPairListPlus other, PreciseEditorData ped, Graph.pListScaled pls) : base(other, ped, pls) { }
            public new void Add(PointPair pp) {
                base.Add(pp);
                if (base.Count > MAX_CAPACITY) {
                    base.RemoveAt(0);
                }
            }
        }
        private List<PointPairListPlusWithMaxCapacity> list;
        private int rowsCount;
        private List<long> sums;
        private List<PointPairListPlusWithMaxCapacity> normalizedList = null;

        public MonitorForm() {
            InitializeComponent();
            Panel = new PreciseMeasureGraphPanel();
            Panel.Graph = Graph.Instance;
        }

        protected override sealed void CreateGraph() {
            ZedGraphRebirth(list, FORM_TITLE);
        }

        protected override sealed void RefreshGraph() {
            //BAD: every time
            // TODO: use getUsed()
            List<PreciseEditorData> pspec = Graph.Instance.PreciseData.FindAll(PreciseEditorData.PeakIsUsed);
            if (pspec.Count != rowsCount)
                // very bad!
                throw new NullReferenceException("Count mismatch");
            
            int j = 0;
            long sum = 0;
            for (int i = 0; i < rowsCount; ++i) {
                PreciseEditorData ped = pspec[i];
                // TODO: exceptions here, problem with backward lines also here?
                long peakSum = ped.AssociatedPoints == null ? 0 :
                    (ped.AssociatedPoints.PLSreference == null ? 0 :
                    ped.AssociatedPoints.PLSreference.PeakSum);
                sum += peakSum;
                list[j].Add(new PointPair(time, peakSum));
                if (normalizedList != null) {
                    normalizedList[j].Add(new PointPair(time, peakSum));
                }
                ++j;
            }
            sums.Add(sum);
            if (normalizedList != null) {
                int index = sums.Count - 1;
                foreach (PointPairList ppl in normalizedList) {
                    ppl[index].Y /= sums[index];
                }
            }
            graph.GraphPane.XAxis.Scale.Min = list[0][0].X;
            graph.AxisChange();
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

        protected void ZedGraphRebirth(List<PointPairListPlusWithMaxCapacity> dataPoints, string title) {
            GraphPane myPane = graph.GraphPane;
            
            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = normalizedList == null ? Y_AXIS_TITLE : Y_AXIS_TITLE + Y_AXIS_RELATIVE;
            myPane.XAxis.Title.Text = X_AXIS_TITLE;
            myPane.CurveList.Clear();

            for (int i = 0; i < dataPoints.Count; ++i) {
                LineItem temp = myPane.AddCurve("My Curve", dataPoints[i], rowsColors[i % rowsColors.Length], SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }

            myPane.Legend.IsVisible = false;
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 10000;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 10000;
            myPane.XAxis.Scale.MaxAuto = true;
            graph.AxisChange();
        }

        // temporary?
        private void InvokeRefreshGraph(Graph.Recreate recreate) {
            if (this.InvokeRequired) {
                // TODO: NullPointerException here..
                this.Invoke(new Graph.GraphEventHandler(refreshGraph), recreate);
                return;
            }
            refreshGraph(recreate);
        }
        private void refreshGraph(Graph.Recreate recreate) {
            if (recreate != Graph.Recreate.None) {
                if (time == -1) {
                    CreateGraph();
                } else {
                    RefreshGraph();
                    time += 1;
                }
            }
            refreshGraphicsOnMeasureStep();
        }
        private void refreshGraphicsOnMeasureStep() {
            MeasureGraphPanel panel = Panel as MeasureGraphPanel;
            panel.performStep();
        }
        #region IMeasured Members
        //parameter here is obsolete
        public void initMeasure(bool isPrecise) {
            list = new List<PointPairListPlusWithMaxCapacity>();
            sums = new List<long>();
            //!!
            // TODO: use extension method getUsed()
            List<PreciseEditorData> pspec = Graph.Instance.PreciseData.FindAll(PreciseEditorData.PeakIsUsed);
            rowsCount = pspec.Count;
            for (int i = 0; i < rowsCount; ++i) {
                //!!!!!! try to prevent nulls in PLS
                var temp = new PointPairListPlusWithMaxCapacity();
                pspec[i].AssociatedPoints = temp;
                list.Add(temp);
            }
            time = 0;
            if (normalizedList == null) {
                CreateGraph();
            } else {
                normalizedList = new List<PointPairListPlusWithMaxCapacity>();
                foreach (PointPairListPlus ppl in list) {
                    normalizedList.Add(new PointPairListPlusWithMaxCapacity(ppl, ppl.PEDreference, null));
                }
                ZedGraphRebirth(normalizedList, FORM_TITLE);
            }

            (Panel as MeasureGraphPanel).MeasureCancelRequested += MonitorForm_MeasureCancelRequested;
            // temporary?
            Graph.Instance.OnNewGraphData += InvokeRefreshGraph;
            Panel.Enable();
            Show();
            Activate();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            // temporary?
            Graph.Instance.OnNewGraphData -= InvokeRefreshGraph;
            time = -1;
        }
        #endregion
        private void MonitorForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            (Panel as MeasureGraphPanel).MeasureCancelRequested -= MonitorForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }
        private void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ZedGraphControlMonitor.ContextMenuBuilderEventArgs args) {
            if (sender is ZedGraphControlMonitor) {
                ToolStripItemCollection items = args.MenuStrip.Items;
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = NORM_ITEM_TEXT;
                item.Checked = (normalizedList != null);
                item.CheckOnClick = true;
                item.CheckedChanged += NormItemCheckStateChanged;

                items.Add(item);
            }
        }
        private void NormItemCheckStateChanged(object sender, EventArgs e) {
            // TODO: modify graph title
            if (normalizedList != null) {
                normalizedList = null;
                CreateGraph();
                graph.Refresh();
                return;
            }
            normalizedList = new List<PointPairListPlusWithMaxCapacity>();
            foreach (PointPairListPlus ppl in list) {
                var temp = new PointPairListPlusWithMaxCapacity(ppl, ppl.PEDreference, null);//???references
                for (int i = 0; i < sums.Count; ++i) {
                    temp[i].Y /= sums[i];
                }
                normalizedList.Add(temp);
            }
            ZedGraphRebirth(normalizedList, FORM_TITLE);
            graph.Refresh();
        }

        private string ZedGraphControlMonitor_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            PointPair pp = curve[iPt];
            string tooltipData = normalizedList == null ?
                                 string.Format(POINT_TOOLTIP_FORMAT, pp.X, pp.Y) :
                                 string.Format(POINT_TOOLTIP_FORMAT, pp.X, 0/*(curve.Points as PointPairListPlus).PEDreference.AssociatedPoints[iPt].Y*/);//smth strange
            string comment = (curve.Points as PointPairListPlus).PEDreference.Comment;
            if (comment != null && comment != "") {
                tooltipData += "\n";
                tooltipData += comment;
            }
            return tooltipData;
        }
    }
}
