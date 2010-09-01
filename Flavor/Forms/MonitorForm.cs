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

        protected override void saveData() {
            base.saveData();
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
            if (sender == null)
                return;
            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);*/
        }

        #region IMeasured Members

        public void prepareControlsOnMeasureStart() {
            Panel.Enable();
        }
        public void initMeasure(bool isPrecise) {
            (Panel as MeasureGraphPanel).monitorToolStripButton_Click();
            Show();
            Activate();
        }

        #endregion
    }
}
