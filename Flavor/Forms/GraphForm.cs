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
    //delegate void CustomZoomEventHandler(ZedGraphControl zgc);
    
    public partial class GraphForm : Form
    {
        private string displayedFileName = "";
        private bool preciseSpecterDisplayed = false;
        private bool prevPreciseSpecterDisplayed = false;

        public bool specterOpeningEnabled
        {
            set 
            {
                openSpecterFileToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = closeSpecterFileToolStripMenuItem.Enabled && value;
            }
        }
        public bool specterClosingEnabled
        {
            set
            {
                closeSpecterFileToolStripMenuItem.Enabled = value;
                //!!!
                distractFromCurrentToolStripMenuItem.Enabled = openSpecterFileToolStripMenuItem.Enabled && value;
            }
        }
        public bool specterSavingEnabled
        {
            set
            {
                saveToolStripMenuItem.Enabled = value;
            }
        }
        
        public GraphForm()
        {
            InitializeComponent();
            Graph.OnAxisModeChanged += new Graph.AxisModeEventHandler(Graph_OnAxisModeChanged);
            this.collect1_graph.GraphPane.XAxis.Scale.Min = 0;
            this.collect1_graph.GraphPane.XAxis.Scale.Max = 1056;
            this.collect2_graph.GraphPane.XAxis.Scale.Min = 0;
            this.collect2_graph.GraphPane.XAxis.Scale.Max = 1056;
        }

        private void Graph_OnAxisModeChanged()
        {
            if (Graph.IsFromFile)
            {
                DisplayLoadedSpectrum(this.collect1_graph, this.collect2_graph, displayedFileName);
            }
            else
            {
                CreateGraph(this.collect1_graph, this.collect2_graph);
            }
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            CreateGraph(collect1_graph, collect2_graph);
            SetSize();
        }

        private void GraphForm_Resize(object sender, EventArgs e)
        {
            SetSize();
        }
        
        public void SetSize()
        {
            collect1_graph.Location = new Point(12, 12);
            collect1_graph.Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);

            collect2_graph.Location = new Point(12, 12 + (this.ClientSize.Height - (12)) / 2);
            collect2_graph.Size = new Size(this.ClientSize.Width - (12 + 12), (this.ClientSize.Height - (12 + 12 + 12)) / 2);
        }

        public void CreateGraph(ZedGraphControlPlus zgc1, ZedGraphControlPlus zgc2)
        {
            displayedFileName = "";
            Graph.IsFromFile = false;
            specterClosingEnabled = false;
            ZedGraphRebirth(zgc1, Graph.Collector1, "Первый коллектор");
            ZedGraphRebirth(zgc2, Graph.Collector2, "Второй коллектор");
        }

        public void RefreshGraph()
        {
            collect1_graph.Refresh();
            collect2_graph.Refresh();
        }

        public void DisplayLoadedSpectrum(ZedGraphControlPlus zgc1, ZedGraphControlPlus zgc2)
        {
            DisplayLoadedSpectrum(zgc1, zgc2, displayedFileName);
        }
        public void DisplayLoadedSpectrum(ZedGraphControlPlus zgc1, ZedGraphControlPlus zgc2, string fileName) 
        {
            displayedFileName = fileName;
            Graph.IsFromFile = true;
            ZedGraphRebirth(zgc1, Graph.LoadedSpectra1, "Первый коллектор");
            ZedGraphRebirth(zgc2, Graph.LoadedSpectra2, "Второй коллектор");
            specterClosingEnabled = true;
        }
        
        private void GraphForm_Validating(object sender, CancelEventArgs e)
        {
            RefreshGraph();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (preciseSpecterDisplayed)
            {
                saveSpecterFileDialog.Filter = "Precise specter files (*.psf)|*.psf";
                saveSpecterFileDialog.DefaultExt = "psf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SavePreciseSpecterFile(saveSpecterFileDialog.FileName, Graph.IsFromFile);
                }
            }
            else
            {
                saveSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf";
                saveSpecterFileDialog.DefaultExt = "sdf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SaveSpecterFile(saveSpecterFileDialog.FileName, Graph.IsFromFile);
                }
            }
        }

        private void ZedGraphRebirth(ZedGraphControlPlus zgc, List<PointPairList> dataPoints, string title)
        {
            GraphPane myPane = zgc.GraphPane;
            
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
                    break;
                case Graph.pListScaled.DisplayValue.Voltage:
                    myPane.XAxis.Title.Text = "Напряжение";
                    break;
                case Graph.pListScaled.DisplayValue.Mass:
                    myPane.XAxis.Title.Text = "Масса";
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

            myPane.Legend.IsVisible = false;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 10000;

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            RefreshGraph();
        }

        private void closeSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            specterClosingEnabled = false;
            CreateGraph(this.collect1_graph, this.collect2_graph);
            preciseSpecterDisplayed = prevPreciseSpecterDisplayed;
        }

        private void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf|Precise specter files (*.psf)|*.psf";
            //openSpecterFileDialog.DefaultExt = "";
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
            {
                Graph.ResetLoadedPointLists();
                if (openSpecterFileDialog.FilterIndex == 1)
                {
                    prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
                    preciseSpecterDisplayed = false;
                    Config.OpenSpecterFile(openSpecterFileDialog.FileName);
                }
                else 
                {
                    prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
                    preciseSpecterDisplayed = true;
                    Config.OpenPreciseSpecterFile(openSpecterFileDialog.FileName);
                }
                DisplayLoadedSpectrum(collect1_graph, collect2_graph, openSpecterFileDialog.FileName);
                specterClosingEnabled = true;
            }
        }

        private void distractFromCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (preciseSpecterDisplayed)
            {
                System.Windows.Forms.MessageBox.Show("Вычитание прецизионных спектров еще не реализовано", "Внутренняя ошибка программы");
                return;
                openSpecterFileDialog.Filter = "Precise specter files (*.psf)|*.psf";
                //openSpecterFileDialog.DefaultExt = "psf";
                if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                }
            }
            else
            {
                openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf";
                //openSpecterFileDialog.DefaultExt = "sdf";
                if (openSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.DistractSpectra(openSpecterFileDialog.FileName);
                }
            }
        }
    }
}