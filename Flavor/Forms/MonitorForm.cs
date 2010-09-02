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

        public MonitorForm() {
            InitializeComponent();
            Panel.Graph = Graph.Instance;
        }
        protected override GraphPanel initPanel() {
            GraphPanel panel = new MeasureGraphPanel();
            return panel;
        }

        protected override sealed void CreateGraph() {
            ZedGraphRebirth(new List<PointPairList>(), "MonitorForm graph test title");
            RefreshGraph();
        }

        protected override sealed void RefreshGraph() {
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
                LineItem temp = myPane.AddCurve(dataPoints[i].ToString(), dataPoints[i], rowsColors[i % rowsColors.Length], SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }
            myPane.Legend.IsShowLegendSymbols = false;
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 10000;
            myPane.YAxis.Scale.MaxAuto = true;
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
                CreateGraph();
            } else {
                RefreshGraph();
                refreshGraphicsOnMeasureStep();
            }
        }

        #region IMeasured Members

        public void initMeasure(bool isPrecise) {
            // temporary?
            Graph.Instance.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);
            //(Panel as MeasureGraphPanel).monitorToolStripButton_Click();
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
        }

        #endregion
    }
}
