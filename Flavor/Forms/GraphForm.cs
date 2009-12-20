using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Flavor
{
    internal partial class GraphForm : Form
    {
        private ZedGraphControlPlus[] graphs;
        private string displayedFileName = "";
        private bool preciseSpecterDisplayed = false;
        private bool prevPreciseSpecterDisplayed = false;

        private ushort[] minX = { 0, 0 }, maxX = { 1056, 1056 };
        private ushort[] minXprev = { 0, 0 }, maxXprev = { 1056, 1056 };
        private Color[] rowsColors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.DarkViolet, Color.DeepPink,
        Color.Black, Color.Magenta,};
        internal bool specterOpeningEnabled
        {
            set 
            {
                openSpecterFileToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled && value &&
                    (Graph.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        internal bool specterClosingEnabled
        {
            set
            {
                closeSpecterFileToolStripMenuItem.Enabled = value;
            }
        }
        internal bool specterSavingEnabled
        {
            set
            {
                saveToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = openSpecterFileToolStripMenuItem.Enabled && value &&
                    (Graph.DisplayingMode != Graph.Displaying.Diff);
            }
        }
        internal bool specterDiffEnabled
        {
            set
            {
                distractFromCurrentToolStripMenuItem.Enabled = openSpecterFileToolStripMenuItem.Enabled && saveToolStripMenuItem.Enabled &&
                    value && (Graph.DisplayingMode != Graph.Displaying.Diff);
            }
            get { return distractFromCurrentToolStripMenuItem.Enabled; }
        }

        internal GraphForm()
        {
            InitializeComponent();
            graphs = new ZedGraphControlPlus[] {collect1_graph, collect2_graph};
            graphs[0].GraphPane.Legend.IsVisible = false;
            graphs[1].GraphPane.Legend.IsVisible = false;
            Graph.OnAxisModeChanged += new Graph.AxisModeEventHandler(Graph_OnAxisModeChanged);
            graphs[0].OnDiffOnPoint += new ZedGraphControlPlus.DiffOnPointEventHandler(GraphForm_OnDiffOnPoint);
            graphs[1].OnDiffOnPoint += new ZedGraphControlPlus.DiffOnPointEventHandler(GraphForm_OnDiffOnPoint);
        }

        private void Graph_OnAxisModeChanged()
        {
            switch (Graph.DisplayingMode)
            {
                case Graph.Displaying.Loaded:
                    DisplayLoadedSpectrum(displayedFileName);
                    break;
                case Graph.Displaying.Measured:
                    CreateGraph();
                    break;
                case Graph.Displaying.Diff:
                    DisplayDiff();
                    break;
            }
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            CreateGraph();
            SetSize();
        }

        private void GraphForm_Resize(object sender, EventArgs e)
        {
            SetSize();
        }
        
        private void SetSize()
        {
            graphs[0].Location = new Point(12, 12);
            graphs[0].Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);

            graphs[1].Location = new Point(12, 12 + (this.ClientSize.Height - (12)) / 2);
            graphs[1].Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);
        }

        internal void setXScaleLimits()
        {
            setXScaleLimits(Config.sPoint, Config.ePoint, Config.sPoint, Config.ePoint);
        }
        internal void setXScaleLimits(ushort minX1, ushort maxX1, ushort minX2, ushort maxX2)
        {
            storeXScaleLimits();
            minX[0] = minX1;
            minX[1] = minX2;
            maxX[0] = maxX1;
            maxX[1] = maxX2;
        }
        internal void setXScaleLimits(List<Utility.PreciseEditorData> peds)
        {
            storeXScaleLimits();
            ushort[] minX = { 1056, 1056 }, maxX = { 0, 0 };
            foreach (Utility.PreciseEditorData ped in peds)
            {
                if (minX[ped.Collector - 1] > ped.Step - ped.Width) 
                    minX[ped.Collector - 1] = (ushort)(ped.Step - ped.Width);
                if (maxX[ped.Collector - 1] < ped.Step + ped.Width)
                    maxX[ped.Collector - 1] = (ushort)(ped.Step + ped.Width);
            }
            this.minX = minX;
            this.maxX = maxX;
        }
        internal void storeXScaleLimits()
        {
            minXprev = minX;
            maxXprev = maxX;
        }
        internal void restoreXScaleLimits()
        {
            minX = minXprev;
            maxX = maxXprev;
        }

        internal void RefreshGraph()
        {
            graphs[0].Refresh();
            graphs[1].Refresh();
        }
        internal void yAxisChange()
        {
            graphs[0].AxisChange();
            graphs[1].AxisChange();
        }

        private void setAutoScales()
        {
            graphs[0].RestoreScale(graphs[0].GraphPane);
            graphs[1].RestoreScale(graphs[1].GraphPane);
            graphs[0].GraphPane.ZoomStack.Clear();
            graphs[1].GraphPane.ZoomStack.Clear();
        }
        
        internal void CreateGraph()
        {
            displayedFileName = "";
            Graph.DisplayingMode = Graph.Displaying.Measured;
            specterClosingEnabled = false;
            ZedGraphRebirth(0, Graph.DisplayedRows1, "Первый коллектор");
            ZedGraphRebirth(1, Graph.DisplayedRows2, "Второй коллектор");
            RefreshGraph();
        }

        internal void DisplayLoadedSpectrum()
        {
            DisplayLoadedSpectrum(displayedFileName);
        }
        internal void DisplayLoadedSpectrum(string fileName) 
        {
            displayedFileName = fileName;
            Graph.DisplayingMode = Graph.Displaying.Loaded;
            ZedGraphRebirth(0, Graph.DisplayedRows1, "Первый коллектор");
            ZedGraphRebirth(1, Graph.DisplayedRows2, "Второй коллектор");
            RefreshGraph();
            specterClosingEnabled = true;
        }
        internal void DisplayDiff()
        {
            Graph.DisplayingMode = Graph.Displaying.Diff;
            ZedGraphRebirth(0, Graph.DisplayedRows1, "Diff - Первый коллектор");
            ZedGraphRebirth(1, Graph.DisplayedRows2, "Diff - Второй коллектор");
            // ?
            RefreshGraph();
            specterClosingEnabled = true;
        }
        
        private void GraphForm_Validating(object sender, CancelEventArgs e)
        {
            RefreshGraph();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (Graph.DisplayingMode)
            {
                case Graph.Displaying.Loaded:
                    if (displayedFileName == "")
                    {
                        saveSpecterFileDialog.FileName = "";
                        break;
                    }
                    saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(displayedFileName);
                    saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
                    break;
                case Graph.Displaying.Measured:
                    saveSpecterFileDialog.FileName = "";
                    break;
                case Graph.Displaying.Diff:
                    if (displayedFileName == "")
                    {
                        saveSpecterFileDialog.FileName = "";
                        break;
                    }
                    saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(displayedFileName);
                    saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName) + "~diff";
                    break;
            }
            if (preciseSpecterDisplayed)
            {
                saveSpecterFileDialog.Filter = "Precise specter files (*.psf)|*.psf";
                saveSpecterFileDialog.DefaultExt = "psf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SavePreciseSpecterFile(saveSpecterFileDialog.FileName, Graph.DisplayingMode);
                }
            }
            else
            {
                saveSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf";
                saveSpecterFileDialog.DefaultExt = "sdf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SaveSpecterFile(saveSpecterFileDialog.FileName, Graph.DisplayingMode);
                }
            }
        }

        private void ZedGraphRebirth(int zgcIndex, List<Graph.pListScaled> dataPoints, string title)
        {
            GraphPane myPane = graphs[zgcIndex].GraphPane;
            
            string modeText = " (прециз.)";
            preciseSpecterDisplayed = true;
            if (dataPoints.Count == 1)
            {
                modeText = " (скан.)";
                preciseSpecterDisplayed = false;
            }
            prevPreciseSpecterDisplayed = preciseSpecterDisplayed;

            myPane.Title.Text = title + modeText;
            myPane.YAxis.Title.Text = "Интенсивность";

            if (!(displayedFileName == ""))
                myPane.Title.Text += ":\n" + displayedFileName;

            switch (Graph.AxisDisplayMode)
            {
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

            if (preciseSpecterDisplayed)
            {
                for (int i = 1; i < dataPoints.Count; ++i)
                {
                    if (dataPoints[i].Step.Count > 0)
                        specterSavingEnabled = true;
                    LineItem temp = myPane.AddCurve(dataPoints[i].PeakSum.ToString(), dataPoints[i].Points(Graph.AxisDisplayMode), rowsColors[i % rowsColors.Length], SymbolType.None);
                    temp.Symbol.Fill = new Fill(Color.White);
                }
            }
            else
            {
                if (dataPoints[0].Step.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp = myPane.AddCurve("My Curve", dataPoints[0].Points(Graph.AxisDisplayMode), Color.Blue, SymbolType.None);
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
        // move to mainForm
        private void closeSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreXScaleLimits();
            //!!!
            CreateGraph();
            preciseSpecterDisplayed = prevPreciseSpecterDisplayed;
        }
        // move to mainForm
        private void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf|Precise specter files (*.psf)|*.psf";
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
            {
                bool hint = (openSpecterFileDialog.FilterIndex == 1);
                try
                {
                    bool result = Config.openSpectrumFile(openSpecterFileDialog.FileName, hint);
                    prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
                    if (result)
                    {
                        preciseSpecterDisplayed = false;
                        ushort minX = (ushort)(Graph.Displayed1Steps[0][0].X);
                        ushort maxX = (ushort)(minX - 1 + Graph.Displayed1Steps[0].Count);
                        setXScaleLimits(minX, maxX, minX, maxX);
                    }
                    else
                    {
                        preciseSpecterDisplayed = true;
                        setXScaleLimits(Config.PreciseDataLoaded);
                    }
                    // YScaleLimits - auto!
                    DisplayLoadedSpectrum(openSpecterFileDialog.FileName);
                    specterClosingEnabled = true;
                }
                catch (Config.ConfigLoadException cle)
                {
                    cle.visualise();
                }
            }
        }

        private void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphForm_OnDiffOnPoint(0, null, null);
        }

        private void GraphForm_OnDiffOnPoint(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference)
        {
            if (preciseSpecterDisplayed)
            {
                openSpecterFileDialog.Filter = "Precise specter files (*.psf)|*.psf";
            }
            else
            {
                openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf";
            }
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Config.DistractSpectra(openSpecterFileDialog.FileName, step, plsReference, pedReference);
                }
                catch (Config.ConfigLoadException cle)
                {
                    cle.visualise();
                }
            }
        }
    }
}