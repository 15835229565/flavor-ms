using System;
using System.Linq;
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
    internal partial class CollectorsForm2: GraphForm {
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
        private ushort[] minX, maxX;
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
        protected CollectorsForm2()
            : base() {
            // do not use! for designer only!
            InitializeComponent();
            Panel = new GraphPanel();
        }
        protected CollectorsForm2(Graph graph, bool hint)
            : base() {
            InitializeComponent();

            ToolStripItemCollection items = this.MainMenuStrip.Items;
            (items[items.IndexOfKey("FileMenu")] as ToolStripMenuItem).DropDownItems.Add(distractFromCurrentToolStripMenuItem);

            this.graph = graph;

            preciseSpectrumDisplayed = hint;
            setTitles();
            string prefix = (graph.DisplayingMode == Graph.Displaying.Diff) ? DIFF_TITLE : "";

            {
                int count = graph.Collectors.Count;
                graphs = new ZedGraphControlPlus[count];
                minX = new ushort[count];
                maxX = new ushort[count];
                setXScaleLimits();
            }

            tabControl.SuspendLayout();
            SuspendLayout();
            int i = 0;
            foreach (var collector in graph.Collectors) {
                //if (collector.TrueForAll(pls => { return pls.isEmpty; })) {
                //    graphs[i] = null;
                //} else {
                    var tabPage = new System.Windows.Forms.TabPage();
                    tabPage.SuspendLayout();
                    tabControl.Controls.Add(tabPage);
                    {
                        var zgc = new Flavor.Controls.ZedGraphControlPlus();
                        tabPage.Controls.Add(zgc);
                        zgc.PointValueEvent += ZedGraphControlPlus_PointValueEvent;
                        zgc.ContextMenuBuilder += ZedGraphControlPlus_ContextMenuBuilder;
                        zgc.ScrollMaxX = maxX[i];
                        zgc.ScrollMinX = minX[i];

                        zgc.GraphPane.Legend.IsVisible = false;
                        zgc.GraphPane.Title.IsVisible = false;
                        zgc.GraphPane.Margin.All = 0;
                        zgc.GraphPane.Margin.Top = 10;
                        zgc.GraphPane.XAxis.Title.FontSpec.Size = 12;
                        zgc.GraphPane.YAxis.Title.FontSpec.Size = 12;
                        zgc.Dock = DockStyle.Fill;

                        zgc.Tag = i + 1;
                        graphs[i] = zgc;
                    }
                    tabPage.UseVisualStyleBackColor = true;
                    tabPage.Text = prefix + i + modeText;
                    tabPage.ResumeLayout(false);
                //}
                ++i;
            }
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

            graph.OnNewGraphData += InvokeRefreshGraph;
            graph.OnAxisModeChanged += InvokeAxisModeChange;
            graph.OnDisplayModeChanged += InvokeGraphModified;
        }

        [Obsolete]
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
            CreateGraph(true);
        }
        private  void CreateGraph(bool recreate) {
            if (graph != null) {
                specterSavingEnabled = false;
                string prefix = (graph.DisplayingMode == Graph.Displaying.Diff) ? DIFF_TITLE : "";
                for (int i = 0; i < graphs.Length; ++i) {
                    if (graphs[i] != null)
                        ZedGraphRebirth(i, graph.Collectors[i], prefix);
                }
            }
            RefreshGraph(recreate);
        }

        protected void setXScaleLimits() {
            setXScaleLimits(Config.sPoint, Config.ePoint);
        }
        protected void setXScaleLimits(ushort min, ushort max) {
            if (min > max) {
                min = Config.MIN_STEP;
                max = Config.MAX_STEP;
            }
            for (int i = 0; i < minX.Length; ++i) {
                minX[i] = min;
                maxX[i] = max;
            }
        }
        protected void setXScaleLimits(List<PreciseEditorData> peds) {
            int count = graphs.Length;
            ushort[] maxX = new ushort[count], minX = new ushort[count];
            for (int i = 0; i < minX.Length; ++i) {
                minX[i] = Config.MAX_STEP;
                maxX[i] = Config.MIN_STEP;
            }
            foreach (PreciseEditorData ped in peds) {
                int col = ped.Collector - 1;
                if (col > count) {
                    // error! wrong collectors number
                }
                ushort lowBound = (ushort)(ped.Step - ped.Width);
                if (lowBound < Config.MIN_STEP) {
                    // error! wrong step
                }
                ushort upBound = (ushort)(ped.Step + ped.Width);
                if (upBound > Config.MAX_STEP) {
                    // error! wrong step
                }
                if (minX[col] > lowBound)
                    minX[col] = lowBound;
                if (maxX[col] < upBound)
                    maxX[col] = upBound;
            }
            this.minX = minX;
            this.maxX = maxX;
        }

        protected override sealed void RefreshGraph() {
            RefreshGraph(true);
        }
        protected void RefreshGraph(bool recreate) {
            if (recreate)
                foreach (var zedgraphcontrol in graphs) {
                    if (zedgraphcontrol != null)
                        zedgraphcontrol.Refresh();
                }
            /*if ((recreate & Graph.Recreate.Col1) == Graph.Recreate.Col1)
                collect1_graph.Refresh();
            if ((recreate & Graph.Recreate.Col2) == Graph.Recreate.Col2)
                collect2_graph.Refresh();*/
        }
        protected void yAxisChange() {
            foreach (var zedgraphcontrol in graphs) {
                if (zedgraphcontrol != null)
                    zedgraphcontrol.AxisChange();
            }
        }

        protected void ZedGraphRebirth(int zgcIndex, Collector dataPoints, string prefix) {
            var zgc = graphs[zgcIndex];
            GraphPane myPane = zgc.GraphPane;

            graphs[zgcIndex].Parent.Text = prefix + zgc.Tag + modeText;
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
                    Collector col = graph.Collectors[zgcIndex];
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
                CreateGraph();
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
                    // TODO: proper collector number here
                    //byte collectorNumber = (byte)((sender == graphs[0]) ? 1 : 2);
                    byte collectorNumber = (byte)(sender as ZedGraphControlPlus).Tag;

                    ushort step = (ushort)pls.Step[index].X;

                    item = new ToolStripMenuItem();
                    item.Text = "Добавить точку в редактор";
                    item.Click += new System.EventHandler((s, e) => {
                        // TODO: raise event here and move code below to mainForm
                        new AddPointForm(step, collectorNumber).ShowDialog();
                    });
                    items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Коэффициент коллектора " + collectorNumber;
                    item.Click += new System.EventHandler((s, e) => {
                        if (new SetScalingCoeffForm(step, collectorNumber, graph).ShowDialog() == DialogResult.Yes)
                            Modified = true;
                    });
                    items.Add(item);
                    {
                        PreciseEditorData ped = pls.PEDreference;

                        item = new ToolStripMenuItem();
                        item.Text = "Вычесть из текущего с перенормировкой на точку";
                        item.Click += new System.EventHandler((s, e) => { GraphForm_OnDiffOnPoint(step, collectorNumber, ped); });
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

                var collector = graph.Collectors[(int)sender.Tag - 1];
                string comment = null;
                try {
                    Graph.pListScaled row = null;
                    switch (graph.AxisDisplayMode) {
                        case Graph.pListScaled.DisplayValue.Step:
                            row = collector.First(pls => curve.Points.Equals(pls.Step));
                            break;
                        case Graph.pListScaled.DisplayValue.Voltage:
                            row = collector.First(pls => curve.Points.Equals(pls.Voltage));
                            break;
                        case Graph.pListScaled.DisplayValue.Mass:
                            row = collector.First(pls => curve.Points.Equals(pls.Mass));
                            break;
                    }
                    peakSum = row.PeakSum;
                    comment = row.PEDreference.Comment;
                } catch (InvalidOperationException) {
                    // strangely no match
                } catch (NullReferenceException) {
                    // strangely null PEDreference
                }
                /*int curveIndex1 = graph.Displayed1.IndexOf((PointPairListPlus)(curve.Points));
                int curveIndex2 = graph.Displayed2.IndexOf((PointPairListPlus)(curve.Points));
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
                }*/
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
