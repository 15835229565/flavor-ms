using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Flavor.Controls;
using System.Collections.Generic;
using Flavor.Common;

namespace Flavor.Forms
{
    internal partial class MonitorForm : GraphForm, IMeasured {
        private const string X_AXIS_TITLE = "Время";

        private double time = -1;
        private List<PointPairList> list;

        public MonitorForm() {
            InitializeComponent();
            Panel.Graph = Graph.Instance;
        }
        protected override GraphPanel initPanel() {
            GraphPanel panel = new MeasureGraphPanel();
            return panel;
        }

        protected override sealed void CreateGraph() {
            ZedGraphRebirth(list, "Режим мониторинга");
        }

        protected override sealed void RefreshGraph() {
            PreciseSpectrum pspec = Config.PreciseData;
            int count = pspec.Count;
            int j = 0;
            for (int i = 0; i < count; ++i) {
                Utility.PreciseEditorData ped = pspec[i];
                if (!ped.Use) {
                    continue;
                }
                list[j].Add(new PointPair(time, ped.AssociatedPoints.PLSreference.PeakSum));
                ++j;
            }
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

        protected void ZedGraphRebirth(List<PointPairList> dataPoints, string title) {
            GraphPane myPane = graph.GraphPane;
            
            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = Y_AXIS_TITLE;
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

        private void ZedGraphControlMonitor_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            /*ZedGraphControlMonitor sender = control as ZedGraphControlMonitor;
            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);*/
        }

        // temporary?
        private void InvokeRefreshGraph(bool recreate) {
            if (this.InvokeRequired) {
                // TODO: NullPointerException here..
                this.Invoke(new Graph.GraphEventHandler(refreshGraph), recreate);
                return;
            }
            refreshGraph(recreate);
        }
        private void refreshGraph(bool recreate) {
            if (recreate) {
                if (time == -1)
                    CreateGraph();
                else {
                    RefreshGraph();
                    time += 1;
                }
            }
            refreshGraphicsOnMeasureStep();
        }

        #region IMeasured Members

        public void initMeasure(bool isPrecise) {
            list = new List<PointPairList>();
            PreciseSpectrum pspec = Config.PreciseData;
            int count = pspec.Count;
            for (int i = 0; i < count; ++i) {
                Utility.PreciseEditorData ped = pspec[i];
                if (!ped.Use) {
                    continue;
                }
                list.Add(new PointPairListPlus(ped, null));
            }
            time = 0;
            CreateGraph();

            // temporary?
            Graph.Instance.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);
            Show();
            Activate();
        }
        public void prepareControlsOnMeasureStart() {
            Panel.Enable();
        }
        public void refreshGraphicsOnMeasureStep() {
            MeasureGraphPanel panel = Panel as MeasureGraphPanel;
            panel.performStep();
            panel.refreshGraphicsOnPreciseStep();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            // temporary?
            Graph.Instance.OnNewGraphData -= new Graph.GraphEventHandler(InvokeRefreshGraph);
            time = -1;
        }

        #endregion
        private string ZedGraphControlMonitor_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            PointPair pp = curve[iPt];
            string tooltipData = string.Format("итерация={0:G},счеты={1:F0}", pp.X, pp.Y);
            string comment = (curve.Points as PointPairListPlus).PEDreference.Comment;
            if (comment != null && comment != "") {
                tooltipData += "\n";
                tooltipData += comment;
            }
            return tooltipData;
        }
    }
}
