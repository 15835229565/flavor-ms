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
        protected const string COL1_TITLE = "Первый коллектор";
        protected const string COL2_TITLE = "Второй коллектор";

        private ZedGraphControlPlus[] graphs;
        protected bool preciseSpecterDisplayed;
        private ushort[] minX = { 0, 0 }, maxX = { 1056, 1056 };
        private ushort[] minXprev = { 0, 0 }, maxXprev = { 1056, 1056 };
        private Color[] rowsColors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.DarkViolet, Color.DeepPink,
        Color.Black, Color.Magenta,};
        protected bool specterClosingEnabled {
            set {
                closeSpecterFileToolStripMenuItem.Enabled = value;
            }
        }
        internal bool specterSavingEnabled {
            set {
                saveToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = value &&
                    (Graph.Instance.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        internal bool specterDiffEnabled {
            set {
                distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled &&
                    value && (Graph.Instance.DisplayingMode != Graph.Displaying.Diff);
            }
            get { return distractFromCurrentToolStripMenuItem.Enabled; }
        }
        public CollectorsForm(bool isPrecise) {
            InitializeComponent();
            preciseSpecterDisplayed = isPrecise;
            graphs = new ZedGraphControlPlus[] { collect1_graph, collect2_graph };
            graphs[0].GraphPane.Legend.IsVisible = false;
            graphs[1].GraphPane.Legend.IsVisible = false;
            Graph.Instance.OnAxisModeChanged += new Graph.AxisModeEventHandler(Graph_OnAxisModeChanged);
            graphs[0].OnDiffOnPoint += new ZedGraphControlPlus.DiffOnPointEventHandler(GraphForm_OnDiffOnPoint);
            graphs[1].OnDiffOnPoint += new ZedGraphControlPlus.DiffOnPointEventHandler(GraphForm_OnDiffOnPoint);

            ToolStripItemCollection items = this.MainMenuStrip.Items;
            (items[items.IndexOfKey("FileMenu")] as ToolStripMenuItem).DropDownItems.Add(distractFromCurrentToolStripMenuItem);
            // 
            // measurePanel
            // 
            Panel = new MeasurePanel();
            Panel.Dock = System.Windows.Forms.DockStyle.Right;
            Panel.Location = new System.Drawing.Point(712, 49);
            Panel.Name = "measurePanel";
            Panel.Size = new System.Drawing.Size(280, 895);
            Panel.TabIndex = 18;
            Panel.Visible = false;
        }
        protected abstract void Graph_OnAxisModeChanged();

        internal sealed override void CreateGraph() {
            ZedGraphRebirth(0, Graph.Instance.DisplayedRows1, COL1_TITLE);
            ZedGraphRebirth(1, Graph.Instance.DisplayedRows2, COL2_TITLE);
            RefreshGraph();
        }
        protected sealed override void SetSize() {
            collect1_graph.Location = new Point(12, 12);
            collect1_graph.Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);

            collect2_graph.Location = new Point(12, 12 + (this.ClientSize.Height - (12)) / 2);
            collect2_graph.Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);
        }

        internal void setXScaleLimits() {
            setXScaleLimits(Config.sPoint, Config.ePoint, Config.sPoint, Config.ePoint);
        }
        internal void setXScaleLimits(ushort minX1, ushort maxX1, ushort minX2, ushort maxX2) {
            storeXScaleLimits();
            minX[0] = minX1;
            minX[1] = minX2;
            maxX[0] = maxX1;
            maxX[1] = maxX2;
        }
        internal void setXScaleLimits(List<Utility.PreciseEditorData> peds) {
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
        internal void storeXScaleLimits() {
            minXprev = minX;
            maxXprev = maxX;
        }
        internal void restoreXScaleLimits() {
            minX = minXprev;
            maxX = maxXprev;
        }

        internal override void RefreshGraph() {
            graphs[0].Refresh();
            graphs[1].Refresh();
        }
        internal void yAxisChange() {
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
            Graph.Instance.DisplayingMode = Graph.Displaying.Diff;
            // TODO!
            string modeText = preciseSpecterDisplayed ? " (прециз.)" : " (скан.)"/* + ":\n" + displayedFileName*/;
            ZedGraphRebirth(0, Graph.Instance.DisplayedRows1, "Diff - Первый коллектор" + modeText);
            ZedGraphRebirth(1, Graph.Instance.DisplayedRows2, "Diff - Второй коллектор" + modeText);
            // ?
            RefreshGraph();
            specterClosingEnabled = true;
        }

        protected void ZedGraphRebirth(int zgcIndex, List<Graph.pListScaled> dataPoints, string title) {
            GraphPane myPane = graphs[zgcIndex].GraphPane;


            myPane.Title.Text = title;
            myPane.YAxis.Title.Text = "Интенсивность";


            switch (Graph.Instance.AxisDisplayMode) {
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
                    LineItem temp = myPane.AddCurve(dataPoints[i].PeakSum.ToString(), dataPoints[i].Points(Graph.Instance.AxisDisplayMode), rowsColors[i % rowsColors.Length], SymbolType.None);
                    temp.Symbol.Fill = new Fill(Color.White);
                }
            } else {
                if (dataPoints[0].Step.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp = myPane.AddCurve("My Curve", dataPoints[0].Points(Graph.Instance.AxisDisplayMode), Color.Blue, SymbolType.None);
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

            //RefreshGraph();
        }
        private void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e) {
            GraphForm_OnDiffOnPoint(0, null, null);
        }

        private void GraphForm_OnDiffOnPoint(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference) {
            if (preciseSpecterDisplayed) {
                openSpecterFileDialog.Filter = Config.preciseSpectrumFileDialogFilter;
            } else {
                openSpecterFileDialog.Filter = Config.spectrumFileDialogFilter;
            }
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    Config.DistractSpectra(openSpecterFileDialog.FileName, step, plsReference, pedReference);
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }
    }
}