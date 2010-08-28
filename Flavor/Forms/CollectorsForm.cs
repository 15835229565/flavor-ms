using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;
using ZedGraph;

namespace Flavor.Forms {
    internal abstract partial class CollectorsForm: GraphForm {
        private const string COL1_TITLE = "Первый коллектор";
        private const string COL2_TITLE = "Второй коллектор";
		private const string DIFF_TITLE = "Diff - ";
		private const string PREC_TITLE = " (прециз.)";
		private const string SCAN_TITLE = " (скан.)";
        private const int HORIZ_GRAPH_INDENT = 12;
        private const int VERT_GRAPH_INDENT = 12;
		
		private string col1Text;
		private string col2Text;
		private string modeText;
		
        private Graph graph;
		
		private ZedGraphControlPlus[] graphs;
        protected bool preciseSpecterDisplayed;
        private ushort[] minX = { 0, 0 }, maxX = { 1056, 1056 };
        private ushort[] minXprev = { 0, 0 }, maxXprev = { 1056, 1056 };
        private Color[] rowsColors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.DarkViolet, Color.DeepPink,
        Color.Black, Color.Magenta,};
        internal bool specterSavingEnabled {
            set {
                saveToolStripMenuItem.Enabled = value;
                //!!!???
                distractFromCurrentToolStripMenuItem.Enabled = value &&
                    (graph.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        internal bool specterDiffEnabled {
            set {
                distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled &&
                    value && (graph.DisplayingMode != Graph.Displaying.Diff);
            }
            get { return distractFromCurrentToolStripMenuItem.Enabled; }
        }
		public CollectorsForm(bool isPrecise, Graph graph) {
			InitializeComponent();

            this.graph = graph;
            graph.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);

            preciseSpecterDisplayed = isPrecise;
            modeText = preciseSpecterDisplayed ? PREC_TITLE : SCAN_TITLE;
			col1Text = COL1_TITLE + modeText;
			col2Text = COL2_TITLE + modeText;

            graph.OnAxisModeChanged += new Graph.AxisModeEventHandler(InvokeAxisModeChange);

            graphs = new ZedGraphControlPlus[] { collect1_graph, collect2_graph };
            graphs[0].GraphPane.Legend.IsVisible = false;
            graphs[1].GraphPane.Legend.IsVisible = false;

            ToolStripItemCollection items = this.MainMenuStrip.Items;
            (items[items.IndexOfKey("FileMenu")] as ToolStripMenuItem).DropDownItems.Add(distractFromCurrentToolStripMenuItem);
        }
        protected override GraphPanel initPanel() {
            GraphPanel panel = new GraphPanel();
            panel.Graph = graph;
			return panel;
		}

        private void InvokeAxisModeChange() {
            if (this.InvokeRequired) {
                this.Invoke(new Graph.AxisModeEventHandler(AxisModeChange));
                return;
            }
            AxisModeChange();
        }
        private void AxisModeChange() {
            axisModeChange();
        }
        protected virtual bool axisModeChange() {
            if (graph.DisplayingMode == Graph.Displaying.Diff) {
	            DisplayDiff();
                return true;
            }
			return false;
		}

        protected sealed override void CreateGraph() {
            ZedGraphRebirth(0, graph.DisplayedRows1, col1Text);
            ZedGraphRebirth(1, graph.DisplayedRows2, col2Text);
            RefreshGraph();
        }
        protected sealed override void SetSize() {
            Size size = new Size(ClientSize.Width - (2 * HORIZ_GRAPH_INDENT) - (Panel.Visible ? Panel.Width : 0), (ClientSize.Height - (3 * VERT_GRAPH_INDENT)) / 2);
            collect1_graph.Location = new Point(HORIZ_GRAPH_INDENT, VERT_GRAPH_INDENT);
            collect1_graph.Size = size;

            collect2_graph.Location = new Point(HORIZ_GRAPH_INDENT, VERT_GRAPH_INDENT + (ClientSize.Height - (VERT_GRAPH_INDENT)) / 2);
            collect2_graph.Size = size;
        }

        protected void setXScaleLimits() {
            setXScaleLimits(Config.sPoint, Config.ePoint, Config.sPoint, Config.ePoint);
        }
        protected void setXScaleLimits(ushort minX1, ushort maxX1, ushort minX2, ushort maxX2) {
            storeXScaleLimits();
            minX[0] = minX1;
            minX[1] = minX2;
            maxX[0] = maxX1;
            maxX[1] = maxX2;
        }
        protected void setXScaleLimits(List<Utility.PreciseEditorData> peds) {
            storeXScaleLimits();
            ushort[] minX = { 1056, 1056 }, maxX = { 0, 0 };
            foreach (Utility.PreciseEditorData ped in peds) {
                if (minX[ped.Collector - 1] > ped.Step - ped.Width)
                    minX[ped.Collector - 1] = (ushort)(ped.Step - ped.Width);
                if (maxX[ped.Collector - 1] < ped.Step + ped.Width)
                    maxX[ped.Collector - 1] = (ushort)(ped.Step + ped.Width);
            }
            this.minX = minX;
            this.maxX = maxX;
        }
        private void storeXScaleLimits() {
            minXprev = minX;
            maxXprev = maxX;
        }
        private void restoreXScaleLimits() {
            minX = minXprev;
            maxX = maxXprev;
        }

        protected override void RefreshGraph() {
            graphs[0].Refresh();
            graphs[1].Refresh();
        }
        protected void yAxisChange() {
            graphs[0].AxisChange();
            graphs[1].AxisChange();
        }

        private void setAutoScales() {
            graphs[0].RestoreScale(graphs[0].GraphPane);
            graphs[1].RestoreScale(graphs[1].GraphPane);
            graphs[0].GraphPane.ZoomStack.Clear();
            graphs[1].GraphPane.ZoomStack.Clear();
        }

        internal void DisplayDiff() {
            graph.DisplayingMode = Graph.Displaying.Diff;
            col1Text = DIFF_TITLE + COL1_TITLE + modeText;
            col2Text = DIFF_TITLE + COL2_TITLE + modeText;
			// multiple times?
            CreateGraph();
            //here?
            closeSpecterFileToolStripMenuItem.Enabled = true;
        }

        protected void ZedGraphRebirth(int zgcIndex, List<Graph.pListScaled> dataPoints, string title) {
            GraphPane myPane = graphs[zgcIndex].GraphPane;


            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = "Интенсивность";


            switch (graph.AxisDisplayMode) {
                case Graph.pListScaled.DisplayValue.Step:
                    myPane.XAxis.Title.Text = "Ступени";
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = minX[zgcIndex];
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = maxX[zgcIndex];
                    break;
                case Graph.pListScaled.DisplayValue.Voltage:
                    myPane.XAxis.Title.Text = "Напряжение (В)";
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = Config.CommonOptions.scanVoltageReal(minX[zgcIndex]);
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = Config.CommonOptions.scanVoltageReal(maxX[zgcIndex]);
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    myPane.XAxis.Title.Text = "Масса (а.е.м.)";
                    //limits inverted due to point-to-mass law
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = Config.pointToMass(maxX[zgcIndex], (zgcIndex == 0));
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = Config.pointToMass(minX[zgcIndex], (zgcIndex == 0));
                    break;
            }

            myPane.CurveList.Clear();

            specterSavingEnabled = false;

            if (preciseSpecterDisplayed) {
                for (int i = 1; i < dataPoints.Count; ++i) {
                    if (dataPoints[i].Step.Count > 0)
                        specterSavingEnabled = true;
                    LineItem temp = myPane.AddCurve(dataPoints[i].PeakSum.ToString(), dataPoints[i].Points(graph.AxisDisplayMode), rowsColors[i % rowsColors.Length], SymbolType.None);
                    temp.Symbol.Fill = new Fill(Color.White);
                }
            } else {
                if (dataPoints[0].Step.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp = myPane.AddCurve("My Curve", dataPoints[0].Points(graph.AxisDisplayMode), Color.Blue, SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }
            myPane.Legend.IsShowLegendSymbols = false;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            // Y-scale needs to be computed more properly!
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 10000;
            //autoscaling needs review. not working now. RefreshGraph or AxisChange anywhere?
            myPane.YAxis.Scale.MaxAuto = true;
            // Calculate the Axis Scale Ranges
            graphs[zgcIndex].AxisChange();
        }
        private void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e) {
            GraphForm_OnDiffOnPoint(0, null, null);
        }

        private void InvokeRefreshGraph(bool recreate) {
            if (this.InvokeRequired) {
                // TODO: NullPointerException here..
                this.Invoke(new Graph.GraphEventHandler(_refreshGraph), recreate);
                return;
            }
            _refreshGraph(recreate);
        }
        private void _refreshGraph(bool recreate) {
            refreshGraph(recreate);
        }
        protected virtual bool refreshGraph(bool recreate) {
            if (graph.DisplayingMode == Graph.Displaying.Diff) {
                if (recreate) {
                    DisplayDiff();
                } else {
                    RefreshGraph();
                }
                return true;
            }
            return false;
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraph.ZedGraphControl.ContextMenuObjectState objState) {
            ZedGraphControlPlus sender = control as ZedGraphControlPlus;
            if (sender == null)
                return;
            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            int iNearest;
            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out iNearest)) {
                /*foreach (ToolStripMenuItem it in menuStrip.Items)
                {
                    if ((string)it.Tag == "set_default")
                    {
                        // remove the menu item
                        menuStrip.Items.Remove(it);
                        // or, just disable the item with this
                        //item.Enabled = false; 

                        break;
                    }
                }*/
                sender.CurveReference = nearestCurve;
                sender.PointIndex = iNearest;

                // create a new menu item for point
                ToolStripMenuItem item = new ToolStripMenuItem();
                // This is the user-defined Tag so you can find this menu item later if necessary
                item.Name = "point_add";
                item.Tag = "point_add";
                // This is the text that will show up in the menu
                item.Text = "Добавить точку в редактор";

                // Add a handler that will respond when that menu item is selected
                item.Click += new System.EventHandler(sender.AddPointToPreciseEditor);
                // Add the menu item to the menu
                menuStrip.Items.Add(item);

                ToolStripMenuItem item1 = new ToolStripMenuItem();

                if (sender.IsFirstCollector) {
                    //sender.CurveIndex = graph.Displayed1.IndexOf((PointPairListPlus)(nearestCurve.Points));
                    item1.Name = "axis_rescale_coeff1";
                    item1.Tag = "axis_rescale_coeff1";
                    item1.Text = "Коэффициент коллектора 1";
                } else {
                    //sender.CurveIndex = graph.Displayed2.IndexOf((PointPairListPlus)(nearestCurve.Points));
                    item1.Name = "axis_rescale_coeff2";
                    item1.Tag = "axis_rescale_coeff2";
                    item1.Text = "Коэффициент коллектора 2";
                }

                item1.Click += new System.EventHandler(sender.SetScalingCoeff);
                menuStrip.Items.Add(item1);

                if (specterDiffEnabled) {
                    ToolStripMenuItem item2 = new ToolStripMenuItem();
                    item2.Name = "custom_diff";
                    item2.Tag = "custom_diff";
                    item2.Text = "Вычесть из текущего с перенормировкой на точку";
                    item2.Click += new System.EventHandler(sender.DiffWithCoeff);
                    menuStrip.Items.Add(item2);
                    if (graph.isPreciseSpectrum) {
                        ToolStripMenuItem item3 = new ToolStripMenuItem();
                        item3.Name = "custom_diff_peak";
                        item3.Tag = "custom_diff_peak";
                        item3.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                        item3.Click += new System.EventHandler(sender.DiffWithCoeff);
                        menuStrip.Items.Add(item3);
                    }
                }
            } else {
                ToolStripMenuItem stepViewItem = new ToolStripMenuItem();
                ToolStripMenuItem voltageViewItem = new ToolStripMenuItem();
                ToolStripMenuItem massViewItem = new ToolStripMenuItem();

                switch (graph.AxisDisplayMode) {
                    case Graph.pListScaled.DisplayValue.Step:
                        stepViewItem.Checked = true;
                        break;
                    case Graph.pListScaled.DisplayValue.Voltage:
                        voltageViewItem.Checked = true;
                        break;
                    case Graph.pListScaled.DisplayValue.Mass:
                        massViewItem.Checked = true;
                        break;
                }

                stepViewItem.Name = "axis_mode_step";
                stepViewItem.Tag = "axis_mode_step";
                stepViewItem.Text = "Ступени";
                stepViewItem.CheckOnClick = true;
                stepViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);

                voltageViewItem.Name = "axis_mode_voltage";
                voltageViewItem.Tag = "axis_mode_voltage";
                voltageViewItem.Text = "Напряжение";
                voltageViewItem.CheckOnClick = true;
                voltageViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);

                massViewItem.Name = "axis_mode_mass";
                massViewItem.Tag = "axis_mode_mass";
                massViewItem.Text = "Масса";
                massViewItem.CheckOnClick = true;
                massViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);

                // create a new general menu item
                ToolStripMenuItem item = new ToolStripMenuItem("", null, stepViewItem, voltageViewItem, massViewItem);
                // This is the user-defined Tag so you can find this menu item later if necessary
                item.Name = "axis_mode";
                item.Tag = "axis_mode";
                // This is the text that will show up in the menu
                item.Text = "Выбрать шкалу";

                // Add a handler that will respond when that menu item is selected
                //item.Click += new System.EventHandler(CustomZoom);
                // Add the menu item to the menu
                menuStrip.Items.Add(item);
            }
        }

        private void ViewItemCheckStateChanged(object sender, EventArgs e) {
            // TODO: move logic to subclasses..
            switch ((sender as ToolStripMenuItem).Name) {
                case "axis_mode_step":
                    graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Step;
                    break;
                case "axis_mode_voltage":
                    graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Voltage;
                    break;
                case "axis_mode_mass":
                    graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Mass;
                    break;
            }
        }

        private string ZedGraphControlPlus_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
            string tooltipData = "";
            PointPair pp = curve[iPt];
            switch (graph.AxisDisplayMode) {
                case Graph.pListScaled.DisplayValue.Step:
                    tooltipData = string.Format("ступень={0:G},счеты={1:F0}", pp.X, pp.Y);
                    break;
                case Graph.pListScaled.DisplayValue.Voltage:
                    tooltipData = string.Format("напряжение={0:####.#},ступень={1:G},счеты={2:F0}", pp.X, pp.Z, pp.Y);
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    tooltipData = string.Format("масса={0:###.##},ступень={1:G},счеты={2:F0}", pp.X, pp.Z, pp.Y);
                    break;
            }
            if (graph.isPreciseSpectrum) {
                int curveIndex1 = graph.Displayed1.IndexOf((PointPairListPlus)(curve.Points));
                int curveIndex2 = graph.Displayed2.IndexOf((PointPairListPlus)(curve.Points));
                long peakSum = -1;
                if (-1 != curveIndex1) {
                    peakSum = graph.DisplayedRows1[curveIndex1].PeakSum;
                } else if (-1 != curveIndex2) {
                    peakSum = graph.DisplayedRows2[curveIndex2].PeakSum;
                }
                if (peakSum != -1)
                    tooltipData += string.Format("\nИнтеграл пика: {0:G}", peakSum);
                else
                    tooltipData += "\nНе удалось идентифицировать пик";
            }
            return tooltipData;
        }
        private void GraphForm_OnDiffOnPoint(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference) {
            if (preciseSpecterDisplayed) {
                openSpecterFileDialog.Filter = Config.preciseSpectrumFileDialogFilter;
            } else {
                openSpecterFileDialog.Filter = Config.spectrumFileDialogFilter;
            }
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                try {
					Config.DistractSpectra(openSpecterFileDialog.FileName, step, plsReference, pedReference, graph);
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }
        protected override void saveData() {
            if (saveSpecterFileDialog.FileName != "") {
                saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(saveSpecterFileDialog.FileName);
                if (Graph.Displaying.Diff == graph.DisplayingMode)
					saveSpecterFileDialog.FileName += Config.DIFF_FILE_SUFFIX;
			}
			if (preciseSpecterDisplayed) {
                saveSpecterFileDialog.Filter = Config.preciseSpectrumFileDialogFilter;
                saveSpecterFileDialog.DefaultExt = Config.PRECISE_SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.SavePreciseSpecterFile(saveSpecterFileDialog.FileName, graph);
                }
            } else {
                saveSpecterFileDialog.Filter = Config.spectrumFileDialogFilter;
                saveSpecterFileDialog.DefaultExt = Config.SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
					Config.SaveSpecterFile(saveSpecterFileDialog.FileName, graph);
                }
            }
        }
	}
}