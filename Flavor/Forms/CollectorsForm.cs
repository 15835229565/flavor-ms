using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using ZedGraph;
using Graph = Flavor.Common.Graph;
using PreciseEditorData = Flavor.Common.Utility.PreciseEditorData;
using Collector = Flavor.Common.Collector;
using CommonOptions = Flavor.Common.CommonOptions;
using PointPairListPlus = Flavor.Common.PointPairListPlus;
// be careful with Config data. use constants only!
using Config = Flavor.Common.Config;

namespace Flavor.Forms {
    internal partial class CollectorsForm: GraphForm {
        private readonly string COL1_TITLE = Resources.CollectorsForm_Col1Title;
        private readonly string COL2_TITLE = Resources.CollectorsForm_Col2Title;
        private readonly string DIFF_TITLE = Resources.CollectorsForm_DiffTitle;
        private readonly string PREC_TITLE = Resources.CollectorsForm_PreciseTitle;
        private readonly string SCAN_TITLE = Resources.CollectorsForm_ScanTitle;

        private readonly string X_AXIS_TITLE_STEP = Resources.CollectorsForm_XAxisTitleStep;
        private readonly string X_AXIS_TITLE_MASS = Resources.CollectorsForm_XAxisTitleMass;
        private readonly string X_AXIS_TITLE_VOLT = Resources.CollectorsForm_XAxisTitleVoltage;

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
                //BAD: only if MeasureCollectorsForm!
                if (preciseSpectrumDisplayed == value)
                    return;
                preciseSpectrumDisplayed = value;
                // actually Graph.Instance; invokes on initMeasure already
                //graph.ResetPointLists();
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
        [Obsolete]
        protected CollectorsForm()
            : base() {
            // do not use! for designer only!
            InitializeComponent();
            Panel = new GraphPanel();
        }
        protected CollectorsForm(Graph graph, bool hint)
            : base() {
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

            this.graph = graph;
            
            preciseSpectrumDisplayed = hint;
            setTitles();

            graph.OnNewGraphData += InvokeRefreshGraph;
            graph.OnAxisModeChanged += InvokeAxisModeChange;
            graph.OnDisplayModeChanged += InvokeGraphModified;
        }

        private void setTitles() {
            modeText = PreciseSpectrumDisplayed ? PREC_TITLE : SCAN_TITLE;
            string prefix = (graph.DisplayingMode == Graph.Displaying.Diff) ? DIFF_TITLE : "";
            col1Text = prefix + COL1_TITLE + modeText;
            col2Text = prefix + COL2_TITLE + modeText;
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
            CreateGraph(Graph.Recreate.Both);
        }
        private void CreateGraph(Graph.Recreate recreate) {
            if (graph != null) {
                specterSavingEnabled = false;
                if ((recreate & Graph.Recreate.Col1) == Graph.Recreate.Col1)
                    ZedGraphRebirth(0, graph.DisplayedRows1, col1Text);
                if ((recreate & Graph.Recreate.Col2) == Graph.Recreate.Col2)
                    ZedGraphRebirth(1, graph.DisplayedRows2, col2Text);
            }
            RefreshGraph(recreate);
        }
        protected override sealed void SetSize() {
            if (graphs == null)
                return;
            Size size = new Size(ClientSize.Width - (2 * HORIZ_GRAPH_INDENT) - (Panel != null && Panel.Visible ? Panel.Width : 0), (ClientSize.Height - (3 * VERT_GRAPH_INDENT)) / 2);
            collect1_graph.Dock = DockStyle.None;
            collect1_graph.Location = new Point(HORIZ_GRAPH_INDENT, VERT_GRAPH_INDENT);
            collect1_graph.Size = size;

            collect2_graph.Dock = DockStyle.None;
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
        protected void setXScaleLimits(List<PreciseEditorData> peds) {
            ushort[] maxX = { Config.MIN_STEP, Config.MIN_STEP }, minX = { Config.MAX_STEP, Config.MAX_STEP };
            foreach (PreciseEditorData ped in peds) {
                int col = ped.Collector - 1;
                ushort lowBound = (ushort)(ped.Step - ped.Width);
                ushort upBound = (ushort)(ped.Step + ped.Width);
                if (minX[col] > lowBound)
                    minX[col] = lowBound;
                if (maxX[col] < upBound)
                    maxX[col] = upBound;
            }
            setXScaleLimits(minX[0], maxX[0], minX[1], maxX[1]);
        }

        protected override sealed void RefreshGraph() {
            RefreshGraph(Graph.Recreate.Both);
        }
        private void RefreshGraph(Graph.Recreate recreate) {
            if ((recreate & Graph.Recreate.Col1) == Graph.Recreate.Col1)
                collect1_graph.Refresh();
            if ((recreate & Graph.Recreate.Col2) == Graph.Recreate.Col2)
                collect2_graph.Refresh();
        }
        protected void yAxisChange() {
            collect1_graph.AxisChange();
            collect2_graph.AxisChange();
        }

        protected void ZedGraphRebirth(int zgcIndex, Collector dataPoints, string title) {
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
                    myPane.XAxis.Scale.Min = CommonOptions.scanVoltageReal(minX[zgcIndex]);
                    myPane.XAxis.Scale.Max = CommonOptions.scanVoltageReal(maxX[zgcIndex]);
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    myPane.XAxis.Title.Text = X_AXIS_TITLE_MASS;
                    //limits inverted due to point-to-mass law
                    Collector col = zgcIndex == 0 ? graph.DisplayedRows1 : graph.DisplayedRows2;
                    myPane.XAxis.Scale.Min = col.pointToMass(maxX[zgcIndex]);
                    myPane.XAxis.Scale.Max = col.pointToMass(minX[zgcIndex]);
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
                CreateGraph(recreate);
                return;
            }
            RefreshGraph();
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(object sender, ZedGraphControlPlus.ContextMenuBuilderEventArgs args) {
            ToolStripItemCollection items = args.MenuStrip.Items;
            ToolStripItem item = new ToolStripSeparator();
            items.Add(item);

            {
                PointPairListPlus ppl = args.Row;
                Graph.pListScaled pls;
                int index = args.Index;
                if (ppl != null && index > 0 && index < ppl.Count && (pls = ppl.PLSreference) != null) {
                    // TODO: collector number here
                    bool isFirstCollector = sender == collect1_graph;
                    byte isFirst = isFirstCollector ? (byte)1 : (byte)2;

                    ushort step = (ushort)pls.Step[index].X;

                    item = new ToolStripMenuItem();
                    item.Text = "Добавить точку в редактор";
                    item.Click += new System.EventHandler((s, e) => {
                        // TODO: raise event here and move code below to mainForm
                        new AddPointForm(step, isFirst).ShowDialog();
                    });
                    items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Коэффициент коллектора" + (isFirstCollector ? " 1" : " 2");
                    item.Click += new System.EventHandler((s, e) => {
                        if (new SetScalingCoeffForm(step, isFirst, graph).ShowDialog() == DialogResult.Yes)
                            Modified = true;
                    });
                    items.Add(item);
                    {
                        PreciseEditorData ped = pls.PEDreference;

                        item = new ToolStripMenuItem();
                        item.Text = "Вычесть из текущего с перенормировкой на точку";
                        item.Click += new System.EventHandler((s, e) => { GraphForm_OnDiffOnPoint(step, isFirst, ped); });
                        items.Add(item);

                        if (ped != null) {
                            item = new ToolStripMenuItem();
                            item.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                            item.Click += new System.EventHandler((s, e) => { GraphForm_OnDiffOnPoint(ushort.MaxValue, null, ped); });
                            items.Add(item);
                        }
                    }
                    item = new ToolStripSeparator();
                    items.Add(item);
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

            stepViewItem.Text = "Ступени";
            stepViewItem.CheckOnClick = true;
            stepViewItem.CheckedChanged += new System.EventHandler((s, e) => {
                graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Step;
            });

            voltageViewItem.Text = "Напряжение";
            voltageViewItem.CheckOnClick = true;
            voltageViewItem.CheckedChanged += new System.EventHandler((s, e) => {
                graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Voltage;
            });

            massViewItem.Text = "Масса";
            massViewItem.CheckOnClick = true;
            massViewItem.CheckedChanged += new System.EventHandler((s, e) => {
                graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Mass;
            });

            item = new ToolStripMenuItem("", null, stepViewItem, voltageViewItem, massViewItem);
            item.Text = "Выбрать шкалу";

            items.Add(item);
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
                string comment = null;
                if (-1 != curveIndex1) {
                    var row = graph.DisplayedRows1[curveIndex1];
                    peakSum = row.PeakSum;
                    try {
                        comment = row.PEDreference.Comment;
                    } catch { }
                } else if (-1 != curveIndex2) {
                    var row = graph.DisplayedRows2[curveIndex2];
                    peakSum = row.PeakSum;
                    try {
                        comment = row.PEDreference.Comment;
                    } catch { }
                }
                if (peakSum != -1) {
                    tooltipData += string.Format("\nИнтеграл пика: {0:G}", peakSum);
                    if (comment != null)
                        tooltipData += string.Format("\n{0}", comment);
                } else
                    tooltipData += "\nНе удалось идентифицировать пик";
            }
            return tooltipData;
        }
        private void GraphForm_OnDiffOnPoint(ushort step, byte? collectorNumber, PreciseEditorData pedReference) {
            if (PreciseSpectrumDisplayed) {
                openSpecterFileDialog.Filter = Config.PRECISE_SPECTRUM_FILE_DIALOG_FILTER;
            } else {
                openSpecterFileDialog.Filter = Config.SPECTRUM_FILE_DIALOG_FILTER;
            }
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    Config.distractSpectra(openSpecterFileDialog.FileName, step, collectorNumber, pedReference, graph);
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
        
        protected override void OnFormClosing(FormClosingEventArgs e) {
            graph.OnNewGraphData -= InvokeRefreshGraph;
            graph.OnAxisModeChanged -= InvokeAxisModeChange;
            graph.OnDisplayModeChanged -= InvokeGraphModified;
            base.OnFormClosing(e);
        }
    }
}
