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
        
        internal bool specterOpeningEnabled
        {
            set 
            {
                openSpecterFileToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled && value;
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
                distractFromCurrentToolStripMenuItem.Enabled = openSpecterFileToolStripMenuItem.Enabled && value;
            }
        }

        internal GraphForm()
        {
            InitializeComponent();
            graphs = new ZedGraphControlPlus[] {collect1_graph, collect2_graph};
            Graph.OnAxisModeChanged += new Graph.AxisModeEventHandler(Graph_OnAxisModeChanged);
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
            minX[0] = minX[1] = Config.sPoint;
            maxX[0] = maxX[1] = Config.ePoint;
        }
        internal void setXScaleLimits(ushort minX1, ushort maxX1, ushort minX2, ushort maxX2)
        {
            minX[0] = minX1;
            minX[1] = minX2;
            maxX[0] = maxX1;
            maxX[1] = maxX2;
        }

        internal void RefreshGraph()
        {
            graphs[0].Refresh();
            graphs[1].Refresh();
        }

        private void setAutoScales()
        {
            graphs[0].RestoreScale(graphs[0].GraphPane);
            graphs[1].RestoreScale(graphs[1].GraphPane);
            graphs[0].GraphPane.ZoomStack.Clear();
            graphs[1].GraphPane.ZoomStack.Clear();
        }
        private void setLegendAndScales()
        {
            if (preciseSpecterDisplayed)
            {
                graphs[0].GraphPane.Legend.IsVisible = true;
                graphs[1].GraphPane.Legend.IsVisible = true;
                setAutoScales();
            }
            else
            {
                graphs[0].GraphPane.Legend.IsVisible = false;
                graphs[1].GraphPane.Legend.IsVisible = false;
            }
            RefreshGraph();
        }
        
        internal void CreateGraph()
        {
            displayedFileName = "";
            Graph.DisplayingMode = Graph.Displaying.Measured;
            specterClosingEnabled = false;
            ZedGraphRebirth(0, Graph.Displayed1, "������ ���������");
            ZedGraphRebirth(1, Graph.Displayed2, "������ ���������");
            setLegendAndScales();
        }

        internal void DisplayLoadedSpectrum()
        {
            DisplayLoadedSpectrum(displayedFileName);
        }
        internal void DisplayLoadedSpectrum(string fileName) 
        {
            displayedFileName = fileName;
            Graph.DisplayingMode = Graph.Displaying.Loaded;
            ZedGraphRebirth(0, Graph.Displayed1, "������ ���������");
            ZedGraphRebirth(1, Graph.Displayed2, "������ ���������");
            setLegendAndScales();
            specterClosingEnabled = true;
        }
        internal void DisplayDiff()
        {
            Graph.DisplayingMode = Graph.Displaying.Diff;
            ZedGraphRebirth(0, Graph.Displayed1, "Diff - ������ ���������");
            ZedGraphRebirth(1, Graph.Displayed2, "Diff - ������ ���������");
            // ?
            setLegendAndScales();
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

        private void ZedGraphRebirth(int zgcIndex, List<PointPairList> dataPoints, string title)
        {
            GraphPane myPane = graphs[zgcIndex].GraphPane;
            
            string modeText = " (������.)";
            preciseSpecterDisplayed = true;
            if (dataPoints.Count == 1)
            {
                modeText = " (����.)";
                preciseSpecterDisplayed = false;
            }
            prevPreciseSpecterDisplayed = preciseSpecterDisplayed;

            myPane.Title.Text = title + modeText;
            myPane.YAxis.Title.Text = "�������������";

            if (!(displayedFileName == ""))
                myPane.Title.Text += ":\n" + displayedFileName;

            switch (Graph.AxisDisplayMode)
            {
                case Graph.pListScaled.DisplayValue.Step:
                    myPane.XAxis.Title.Text = "�������";
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = minX[zgcIndex];
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = maxX[zgcIndex];
                    break;
                case Graph.pListScaled.DisplayValue.Voltage:
                    myPane.XAxis.Title.Text = "���������� (�)";
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = Config.scanVoltageReal(minX[zgcIndex]);
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = Config.scanVoltageReal(maxX[zgcIndex]);
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    myPane.XAxis.Title.Text = "����� (�.�.�.)";
                    //limits inverted due to point-to-mass law
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Min = Config.pointToMass(maxX[zgcIndex], (zgcIndex == 0));
                    graphs[zgcIndex].GraphPane.XAxis.Scale.Max = Config.pointToMass(minX[zgcIndex], (zgcIndex == 0));
                    break;
            }

            myPane.CurveList.Clear();
            
            specterSavingEnabled = false;
            foreach (PointPairList ppl in dataPoints)
            {
                if (ppl.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp = myPane.AddCurve("My Curve", ppl, Color.Blue, SymbolType.None);
                temp.Symbol.Fill = new Fill(Color.White);
            }

            //myPane.Legend.IsVisible = false;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            // Y-scale needs to be computed more properly!
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 10000;

            myPane.YAxis.Scale.MaxAuto = true;
            // Calculate the Axis Scale Ranges
            graphs[zgcIndex].AxisChange();

            //RefreshGraph();
        }

        private void closeSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setXScaleLimits();
            //!!!
            CreateGraph();
            preciseSpecterDisplayed = prevPreciseSpecterDisplayed;
        }

        private void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf|Precise specter files (*.psf)|*.psf";
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openSpecterFileDialog.FilterIndex == 1)
                {
                    Config.OpenSpecterFile(openSpecterFileDialog.FileName);
                    prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
                    preciseSpecterDisplayed = false;
                    // setXScaleLimits
                    ushort minX = (ushort)(Graph.Displayed1Steps[0][0].X);
                    ushort maxX = (ushort)(minX - 1 + Graph.Displayed1Steps[0].Count);
                    setXScaleLimits(minX, maxX, minX, maxX);
                    // YScaleMax - auto!
                    DisplayLoadedSpectrum(openSpecterFileDialog.FileName);
                }
                else 
                {
                    Config.OpenPreciseSpecterFile(openSpecterFileDialog.FileName);
                    prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
                    preciseSpecterDisplayed = true;
                    DisplayLoadedSpectrum(openSpecterFileDialog.FileName);
                    // Default scale
                    setAutoScales();
                }

                specterClosingEnabled = true;
            }
        }

        private void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e)
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
                Config.DistractSpectra(openSpecterFileDialog.FileName);
            }
        }
    }
}