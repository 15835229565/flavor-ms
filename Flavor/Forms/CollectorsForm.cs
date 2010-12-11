using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;
using ZedGraph;

namespace Flavor.Forms
{
    internal partial class CollectorsForm : GraphForm {
        private const string COL1_TITLE = "Первый коллектор";
        private const string COL2_TITLE = "Второй коллектор";
        private const string DIFF_TITLE = "Diff - ";
        private const string PREC_TITLE = " (прециз.)";
        private const string SCAN_TITLE = " (скан.)";

        private const string X_AXIS_TITLE_STEP = "Ступени";
        private const string X_AXIS_TITLE_MASS = "Масса (а.е.м.)";
        private const string X_AXIS_TITLE_VOLT = "Напряжение (В)";

        private const string DIFF_ON_POINT_TAG = "custom_diff";
        public const string DIFF_ON_PEAK_TAG = "custom_diff_peak";
        private const string VIEW_MODE_TAG = "view_mode";
        private const string STEP_VIEW_MODE_TAG = "axis_mode_step";
        private const string MASS_VIEW_MODE_TAG = "axis_mode_voltage";
        private const string VOLT_VIEW_MODE_TAG = "axis_mode_mass";

        private string col1Text;
        private string col2Text;
        private string modeText;

        private Graph graph;
        private bool modified = false;

        protected bool Modified {
            get { return modified; }
            private set {
                if (modified == value)
                    return;
                modified = value;
                updateOnModification();
            }
        }
        protected virtual void updateOnModification() {
            Activate();
        }

        private ZedGraphControlPlus[] graphs = null;
        private bool preciseSpectrumDisplayed;
        // TODO: make more clear here. now in Graph can be mistake
        protected bool PreciseSpectrumDisplayed {
            get { return preciseSpectrumDisplayed; }
            set {
                if (preciseSpectrumDisplayed == value)
                    return;
                preciseSpectrumDisplayed = value;
                graph.ResetPointLists();
                setTitles();
            }
        }
        private ushort[] minX = { Config.MIN_STEP, Config.MIN_STEP }, maxX = { Config.MAX_STEP, Config.MAX_STEP };
        internal protected bool specterSavingEnabled {
            private get {
                return saveToolStripMenuItem.Enabled;
            }
            set {
                saveToolStripMenuItem.Enabled = value;
                //!!!???
                distractFromCurrentToolStripMenuItem.Enabled = value && (graph.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        internal bool specterDiffEnabled {
            get { return distractFromCurrentToolStripMenuItem.Enabled; }
            //set { distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled && value && (graph.DisplayingMode != Graph.Displaying.Diff); }
        }
        protected CollectorsForm() {
            // do not use! for designer only!
            InitializeComponent();

            collect1_graph.GraphPane.Legend.IsVisible = false;
            collect2_graph.GraphPane.Legend.IsVisible = false;
            collect1_graph.ScrollMaxX = maxX[0];
            collect1_graph.ScrollMinX = minX[0];
            collect2_graph.ScrollMaxX = maxX[1];
            collect2_graph.ScrollMinX = minX[1];

            graphs = new ZedGraphControlPlus[] { collect1_graph, collect2_graph };

            ToolStripItemCollection items = this.MainMenuStrip.Items;
            (items[items.IndexOfKey("FileMenu")] as ToolStripMenuItem).DropDownItems.Add(distractFromCurrentToolStripMenuItem);
        }
        protected CollectorsForm(Graph graph, bool hint): this() {
            this.graph = graph;
            Panel.Graph = graph;
            
            preciseSpectrumDisplayed = hint;
            setTitles();
            
            graph.OnAxisModeChanged += new Graph.AxisModeEventHandler(InvokeAxisModeChange);
            graph.OnDisplayModeChanged += new Graph.DisplayModeEventHandler(InvokeGraphModified);
        }

        private void setTitles() {
            modeText = PreciseSpectrumDisplayed ? PREC_TITLE : SCAN_TITLE;
            string prefix = (graph.DisplayingMode == Graph.Displaying.Diff) ? DIFF_TITLE : "";
            col1Text = prefix + COL1_TITLE + modeText;
            col2Text = prefix + COL2_TITLE + modeText;
        }

        protected override GraphPanel initPanel() {
            GraphPanel panel = new GraphPanel();
            return panel;
        }

        private void InvokeAxisModeChange() {
            if (this.InvokeRequired) {
                this.Invoke(new Graph.AxisModeEventHandler(CreateGraph));
                return;
            }
            CreateGraph();
        }
        private void InvokeGraphModified(Graph.Displaying mode) {
            if (this.InvokeRequired) {
                this.Invoke(new Graph.DisplayModeEventHandler(GraphModified), mode);
                return;
            }
            GraphModified(mode);
        }
        private void GraphModified(Graph.Displaying mode) {
            if (mode == Graph.Displaying.Diff) {
                setTitles();
                Modified = true;
            }
        }

        protected override sealed void CreateGraph() {
            if (graph != null) {
                specterSavingEnabled = false;
                ZedGraphRebirth(0, graph.DisplayedRows1, col1Text);
                ZedGraphRebirth(1, graph.DisplayedRows2, col2Text);
            }
            RefreshGraph();
        }
        protected override sealed void SetSize() {
            if (graphs == null)
                return;
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
            if (minX1 < maxX1) {
                minX[0] = minX1;
                maxX[0] = maxX1;
            } else {
                minX[0] = Config.MIN_STEP;
                maxX[0] = Config.MAX_STEP;
            }
            if (minX2 < maxX2) {
                minX[1] = minX2;
                maxX[1] = maxX2;
            } else {
                minX[1] = Config.MIN_STEP;
                maxX[1] = Config.MAX_STEP;
            }
        }
        protected void setXScaleLimits(List<Utility.PreciseEditorData> peds) {
            ushort[] maxX = { Config.MIN_STEP, Config.MIN_STEP }, minX = { Config.MAX_STEP, Config.MAX_STEP };
            foreach (Utility.PreciseEditorData ped in peds) {
                if (minX[ped.Collector - 1] > ped.Step - ped.Width)
                    minX[ped.Collector - 1] = (ushort)(ped.Step - ped.Width);
                if (maxX[ped.Collector - 1] < ped.Step + ped.Width)
                    maxX[ped.Collector - 1] = (ushort)(ped.Step + ped.Width);
            }
            setXScaleLimits(minX[0], maxX[0], minX[1], maxX[1]);
        }

        protected override sealed void RefreshGraph() {
            collect1_graph.Refresh();
            collect2_graph.Refresh();
        }
        protected void yAxisChange() {
            collect1_graph.AxisChange();
            collect2_graph.AxisChange();
        }

        protected void ZedGraphRebirth(int zgcIndex, List<Graph.pListScaled> dataPoints, string title) {
            GraphPane myPane = graphs[zgcIndex].GraphPane;
            
            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = Y_AXIS_TITLE;
            
            switch (graph.AxisDisplayMode) {
                case Graph.pListScaled.DisplayValue.Step:
                    myPane.XAxis.Title.Text = X_AXIS_TITLE_STEP;
                    myPane.XAxis.Scale.Min = minX[zgcIndex];
                    myPane.XAxis.Scale.Max = maxX[zgcIndex];
                    break;
                case Graph.pListScaled.DisplayValue.Voltage:
                    myPane.XAxis.Title.Text = X_AXIS_TITLE_VOLT;
                    myPane.XAxis.Scale.Min = Config.CommonOptions.scanVoltageReal(minX[zgcIndex]);
                    myPane.XAxis.Scale.Max = Config.CommonOptions.scanVoltageReal(maxX[zgcIndex]);
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    myPane.XAxis.Title.Text = X_AXIS_TITLE_MASS;
                    //limits inverted due to point-to-mass law
                    bool isFirst = (zgcIndex == 0);
                    myPane.XAxis.Scale.Min = Config.pointToMass(maxX[zgcIndex], isFirst);
                    myPane.XAxis.Scale.Max = Config.pointToMass(minX[zgcIndex], isFirst);
                    break;
            }
            
            myPane.CurveList.Clear();

            bool savingDisabled = !specterSavingEnabled;
            if (PreciseSpectrumDisplayed) {
                for (int i = 1; i < dataPoints.Count; ++i) {
                    if (savingDisabled && dataPoints[i].Step.Count > 0)
                        savingDisabled = false;
                    LineItem temp = myPane.AddCurve(dataPoints[i].PeakSum.ToString(), dataPoints[i].Points(graph.AxisDisplayMode), rowsColors[i % rowsColors.Length], SymbolType.None);
                    temp.Symbol.Fill = new Fill(Color.White);
                }
            } else {
                if (savingDisabled && dataPoints[0].Step.Count > 0)
                    savingDisabled = false;
                LineItem temp = myPane.AddCurve("My Curve", dataPoints[0].Points(graph.AxisDisplayMode), Color.Blue, SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }
            specterSavingEnabled = !savingDisabled;
            myPane.Legend.IsVisible = false;
            
            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
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

        protected void InvokeRefreshGraph(bool recreate) {
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
                if (this is IMeasured)
                    (this as IMeasured).refreshGraphicsOnMeasureStep();
            }
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            ZedGraphControlPlus sender = control as ZedGraphControlPlus;
            if (sender == null)
                return;

            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            int iNearest;
            ToolStripMenuItem item;
            ToolStripItemCollection items = menuStrip.Items;
            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out iNearest)) {
                sender.CurveRef = (nearestCurve.Points as PointPairListPlus).PLSreference;
                sender.PointIndex = iNearest;
                
                item = new ToolStripMenuItem();
                item.Text = "Добавить точку в редактор";
                item.Click += new System.EventHandler(sender.AddPointToPreciseEditor);
                items.Add(item);

                item = new ToolStripMenuItem();
                item.Text = "Коэффициент коллектора" + (sender.IsFirstCollector ? " 1" : " 2");
                item.Click += new System.EventHandler(sender.SetScalingCoeff);
                items.Add(item);
                
                if (specterDiffEnabled) {
                    item = new ToolStripMenuItem();
                    item.Name = DIFF_ON_POINT_TAG;
                    item.Text = "Вычесть из текущего с перенормировкой на точку";
                    item.Click += new System.EventHandler(sender.DiffWithCoeff);
                    items.Add(item);
                    
                    if (graph.isPreciseSpectrum) {
                        item = new ToolStripMenuItem();
                        item.Name = DIFF_ON_PEAK_TAG;
                        item.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                        item.Click += new System.EventHandler(sender.DiffWithCoeff);
                        items.Add(item);
                    }
                }
            }
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
            
            stepViewItem.Name = STEP_VIEW_MODE_TAG;
            stepViewItem.Text = "Ступени";
            stepViewItem.CheckOnClick = true;
            stepViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);
            
            voltageViewItem.Name = VOLT_VIEW_MODE_TAG;
            voltageViewItem.Text = "Напряжение";
            voltageViewItem.CheckOnClick = true;
            voltageViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);
            
            massViewItem.Name = MASS_VIEW_MODE_TAG;
            massViewItem.Text = "Масса";
            massViewItem.CheckOnClick = true;
            massViewItem.CheckedChanged += new System.EventHandler(ViewItemCheckStateChanged);

            item = new ToolStripMenuItem("", null, stepViewItem, voltageViewItem, massViewItem);
            item.Name = VIEW_MODE_TAG;
            item.Text = "Выбрать шкалу";

            items.Add(item);
        }
        private void ViewItemCheckStateChanged(object sender, EventArgs e) {
            // TODO: move logic to subclasses..
            switch ((sender as ToolStripMenuItem).Name) {
                case STEP_VIEW_MODE_TAG:
                graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Step;
                    break;
                case VOLT_VIEW_MODE_TAG:
                graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Voltage;
                    break;
                case MASS_VIEW_MODE_TAG:
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
                long peakSum = -1;
                int curveIndex1 = graph.Displayed1.IndexOf((PointPairListPlus)(curve.Points));
                int curveIndex2 = graph.Displayed2.IndexOf((PointPairListPlus)(curve.Points));
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
            if (PreciseSpectrumDisplayed) {
                openSpecterFileDialog.Filter = Config.PRECISE_SPECTRUM_FILE_DIALOG_FILTER;
            } else {
                openSpecterFileDialog.Filter = Config.SPECTRUM_FILE_DIALOG_FILTER;
            }
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    Config.distractSpectra(openSpecterFileDialog.FileName, step, plsReference, pedReference, graph);
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }
        protected override bool saveData() {
            if (saveSpecterFileDialog.FileName != "") {
                saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(saveSpecterFileDialog.FileName);
                if (Graph.Displaying.Diff == graph.DisplayingMode)
                    saveSpecterFileDialog.FileName += Config.DIFF_FILE_SUFFIX;
            }
            if (PreciseSpectrumDisplayed) {
                saveSpecterFileDialog.Filter = Config.PRECISE_SPECTRUM_FILE_DIALOG_FILTER;
                saveSpecterFileDialog.DefaultExt = Config.PRECISE_SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.savePreciseSpectrumFile(saveSpecterFileDialog.FileName, graph);
                    Modified = false;
                    return true;
                }
            } else {
                saveSpecterFileDialog.Filter = Config.SPECTRUM_FILE_DIALOG_FILTER;
                saveSpecterFileDialog.DefaultExt = Config.SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.saveSpectrumFile(saveSpecterFileDialog.FileName, graph);
                    Modified = false;
                    return true;
                }
            }
            return false;
        }
        }
    }
