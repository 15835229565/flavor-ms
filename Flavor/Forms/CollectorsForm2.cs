using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using ZedGraph;
using Graph = Flavor.Common.Data.Measure.Graph;
using Collector = Flavor.Common.Data.Measure.Collector;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

using PointPairListPlus = Flavor.Common.Data.Measure.PointPairListPlus;
// be careful with Config data. use constants only!
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class CollectorsForm2: GraphForm {
        readonly string COL1_TITLE = Resources.CollectorsForm_Col1Title;
        readonly string COL2_TITLE = Resources.CollectorsForm_Col2Title;
        readonly string DIFF_TITLE = Resources.CollectorsForm_DiffTitle;
        readonly string PREC_TITLE = Resources.CollectorsForm_PreciseTitle;
        readonly string SCAN_TITLE = Resources.CollectorsForm_ScanTitle;

        readonly string X_AXIS_TITLE_STEP = Resources.CollectorsForm_XAxisTitleStep;
        readonly string X_AXIS_TITLE_MASS = Resources.CollectorsForm_XAxisTitleMass;
        readonly string X_AXIS_TITLE_VOLT = Resources.CollectorsForm_XAxisTitleVoltage;

        string col1Text;
        string col2Text;
        string modeText;

        Graph graph;
        bool modified = false;

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

        ZedGraphControlPlus[] graphs = null;
        bool preciseSpectrumDisplayed;
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
        ushort[] minX, maxX;
        protected bool specterSavingEnabled {
            private get {
                return saveToolStripMenuItem.Enabled;
            }
            set {
                saveToolStripMenuItem.Enabled = value;
                //!!!???
                distractFromCurrentToolStripMenuItem.Enabled = value && (graph.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        public bool specterDiffEnabled {
            get { return distractFromCurrentToolStripMenuItem.Enabled; }
            //set { distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled && value && (graph.DisplayingMode != Graph.Displaying.Diff); }
        }
        [Obsolete]
        protected CollectorsForm2()
            : base() {
            // do not use! for designer only!
            // Init panel before ApplyResources
            Panel = new GraphPanel();
            InitializeComponent();
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

                        zgc.Tag = (byte)(i + 1);
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

            graph.NewGraphData += InvokeRefreshGraph;
            graph.OnAxisModeChanged += InvokeAxisModeChange;
            graph.OnDisplayModeChanged += InvokeGraphModified;
        }

        [Obsolete]
        void setTitles() {
            modeText = PreciseSpectrumDisplayed ? PREC_TITLE : SCAN_TITLE;
            string prefix = (graph.DisplayingMode == Graph.Displaying.Diff) ? DIFF_TITLE : "";
            col1Text = prefix + COL1_TITLE + modeText;
            col2Text = prefix + COL2_TITLE + modeText;
        }

        void InvokeAxisModeChange() {
            if (InvokeRequired) {
                Invoke(new Graph.AxisModeEventHandler(CreateGraph));
                return;
            }
            CreateGraph();
        }
        void InvokeGraphModified(Graph.Displaying mode) {
            if (InvokeRequired) {
                Invoke(new Graph.DisplayModeEventHandler(GraphModified), mode);
                return;
            }
            GraphModified(mode);
        }
        void GraphModified(Graph.Displaying mode) {
            if (mode == Graph.Displaying.Diff) {
                setTitles();
                Modified = true;
            }
        }

        protected override sealed void CreateGraph() {
            CreateGraph(true);
        }
        void CreateGraph(bool recreate) {
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
            if (recreate && graphs != null)
                foreach (var zedgraphcontrol in graphs) {
                    if (zedgraphcontrol != null)
                        zedgraphcontrol.Refresh();
                }
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
                    //myPane.XAxis.Scale.Min = graph.CommonOptions.scanVoltageNew(minX[zgcIndex]);
                    //myPane.XAxis.Scale.Max = graph.CommonOptions.scanVoltageNew(maxX[zgcIndex]);
                    myPane.XAxis.Scale.Min = Flavor.Common.Settings.CommonOptions.scanVoltageReal(minX[zgcIndex]);
                    myPane.XAxis.Scale.Max = Flavor.Common.Settings.CommonOptions.scanVoltageReal(maxX[zgcIndex]);
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
        void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e) {
            GraphForm_OnDiffOnPoint(0, null, null);
        }

        void InvokeRefreshGraph(uint[] counts, params int[] recreate) {
            if (InvokeRequired) {
                // TODO: NullPointerException here..
                Invoke(new Graph.GraphEventHandler(refreshGraph), counts, recreate);
                return;
            }
            refreshGraph(counts, recreate);
        }
        void refreshGraph(uint[] counts, params int[] recreate) {
            if (recreate.Length != 0) {
                CreateGraph();
                return;
            }
            RefreshGraph();
        }

        void ZedGraphControlPlus_ContextMenuBuilder(object sender, ZedGraphControlPlus.ContextMenuBuilderEventArgs args) {
            ToolStripItemCollection items = args.MenuStrip.Items;
            ToolStripItem item = new ToolStripSeparator();
            items.Add(item);

            {
                PointPairListPlus ppl = args.Row;
                Graph.pListScaled pls;
                int index = args.Index;
                if (ppl != null && index > 0 && index < ppl.Count && (pls = ppl.PLSreference) != null) {
                    byte collectorNumber = (byte)(sender as ZedGraphControlPlus).Tag;

                    ushort step = (ushort)pls.Step[index].X;

                    item = new ToolStripMenuItem();
                    item.Text = "Добавить точку в редактор";
                    item.Click += (s, e) => {
                        // TODO: raise event here and move code below to mainForm
                        new AddPointForm(step, collectorNumber).ShowDialog();
                    };
                    items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Коэффициент коллектора " + collectorNumber;
                    item.Click += (s, e) => {
                        if (new SetScalingCoeffForm(step, collectorNumber, graph).ShowDialog() == DialogResult.Yes)
                            Modified = true;
                    };
                    items.Add(item);
                    {
                        PreciseEditorData ped = pls.PEDreference;

                        item = new ToolStripMenuItem();
                        item.Text = "Вычесть из текущего с перенормировкой на точку";
                        item.Click += (s, e) => GraphForm_OnDiffOnPoint(step, collectorNumber, ped);
                        items.Add(item);

                        if (ped != null) {
                            item = new ToolStripMenuItem();
                            item.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                            item.Click += (s, e) => GraphForm_OnDiffOnPoint(ushort.MaxValue, null, ped);
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
            stepViewItem.CheckedChanged += (s, e) => graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Step;

            voltageViewItem.Text = "Напряжение";
            voltageViewItem.CheckOnClick = true;
            voltageViewItem.CheckedChanged += (s, e) => graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Voltage;

            massViewItem.Text = "Масса";
            massViewItem.CheckOnClick = true;
            massViewItem.CheckedChanged += (s, e) => graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Mass;

            item = new ToolStripMenuItem("", null, stepViewItem, voltageViewItem, massViewItem);
            item.Text = "Выбрать шкалу";

            items.Add(item);
        }

        string ZedGraphControlPlus_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) {
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

                var collector = graph.Collectors[(byte)sender.Tag - 1];
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
                if (peakSum != -1) {
                    tooltipData += string.Format("\nИнтеграл пика: {0:G}", peakSum);
                    if (comment != null)
                        tooltipData += string.Format("\n{0}", comment);
                } else
                    tooltipData += "\nНе удалось идентифицировать пик";
            }
            return tooltipData;
        }
        void GraphForm_OnDiffOnPoint(ushort step, byte? collectorNumber, PreciseEditorData pedReference) {
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
            graph.NewGraphData -= InvokeRefreshGraph;
            graph.OnAxisModeChanged -= InvokeAxisModeChange;
            graph.OnDisplayModeChanged -= InvokeGraphModified;
            base.OnFormClosing(e);
        }
    }
}
