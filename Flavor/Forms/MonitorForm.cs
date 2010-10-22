using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Flavor.Controls;
using System.Collections.Generic;
using Flavor.Common;

namespace Flavor.Forms
{
    internal partial class MonitorForm: GraphForm, IMeasured {
        private const string FORM_TITLE = "Режим мониторинга";
        private const string X_AXIS_TITLE = "Время";
        private const string NORM_ITEM_TEXT = "Нормировать";
        private const string POINT_TOOLTIP_FORMAT = "итерация={0:G},счеты={1:F0}";

        private double time = -1;
        private List<PointPairList> list;
        private int rowsCount;
        private List<long> sums;
        private List<PointPairList> normalizedList = null;

        public MonitorForm() {
            InitializeComponent();
            Panel.Graph = Graph.Instance;
        }
        protected override GraphPanel initPanel() {
            return new MeasureGraphPanel();
        }

        protected override sealed void CreateGraph() {
            ZedGraphRebirth(list, FORM_TITLE);
        }

        protected override sealed void RefreshGraph() {
            PreciseSpectrum pspec = Config.PreciseData;
            int j = 0;
            long sum = 0;
            for (int i = 0; i < rowsCount; ++i) {
                Utility.PreciseEditorData ped = pspec[i];
                if (!ped.Use) {
                    continue;
                }
                long peakSum = ped.AssociatedPoints.PLSreference.PeakSum;
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
                if (time == -1) {
                    /*if (normalizedList != null) {
                        int index = sums.Count - 1;
                        foreach (PointPairList ppl in normalizedList) {
                            ppl[index].Y /= sums[index];
                        }
                    }*/
                    CreateGraph();
                } else {
                    RefreshGraph();
                    time += 1;
                }
            }
            refreshGraphicsOnMeasureStep();
        }

        #region IMeasured Members

        public void initMeasure(bool isPrecise) {
            list = new List<PointPairList>();
            sums = new List<long>();
            PreciseSpectrum pspec = Config.PreciseData;
            rowsCount = pspec.Count;
            for (int i = 0; i < rowsCount; ++i) {
                Utility.PreciseEditorData ped = pspec[i];
                if (!ped.Use) {
                    continue;
                }
                list.Add(new PointPairListPlus(ped, null));
            }
            time = 0;
            if (normalizedList == null) {
                CreateGraph();
            } else {
                normalizedList = new List<PointPairList>();
                foreach (PointPairListPlus ppl in list) {
                    normalizedList.Add(new PointPairListPlus(ppl, ppl.PEDreference, null));
                }
                ZedGraphRebirth(normalizedList, FORM_TITLE);
            }

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

        private void ZedGraphControlMonitor_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            ZedGraphControlMonitor sender = control as ZedGraphControlMonitor;
            if (sender == null)
                return;
            sender.CommonContextMenuBuilder(menuStrip, mousePt, objState);//better to subscribe to new created event in control..
            
            ToolStripItemCollection items = menuStrip.Items;

            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = NORM_ITEM_TEXT;
            item.Checked = (normalizedList != null);
            item.CheckOnClick = true;
            item.CheckedChanged += new System.EventHandler(NormItemCheckStateChanged);

            items.Add(item);
        }
        private void NormItemCheckStateChanged(object sender, EventArgs e) {
            // TODO: modify graph title
            if (normalizedList != null) {
                normalizedList = null;
                CreateGraph();
                graph.Refresh();
                return;
            }
            normalizedList = new List<PointPairList>();
            foreach (PointPairListPlus ppl in list) {
                PointPairList temp = new PointPairListPlus(ppl, ppl.PEDreference, null);//???references
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
