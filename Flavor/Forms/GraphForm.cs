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
    delegate void CustomZoomEventHandler(ZedGraphControl zgc);
    
    public partial class GraphForm : Form
    {
        private bool isFromFile = false;
        private string displayedFileName = "";

        private bool preciseSpecterDisplayed = false;

        private bool prevPreciseSpecterDisplayed = false;

        public bool specterOpeningEnabled
        {
            set 
            {
                openSpecterFileToolStripMenuItem.Enabled = value;
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
            Graph.OnAxisModeChanged += new AxisModeEventHandler(Graph_OnAxisModeChanged);
            //Commander.OnProgramStateChanged += new ProgramEventHandler(ChangeSpecterType);
            this.collect1_graph.GraphPane.XAxis.Scale.Min = 0;
            this.collect1_graph.GraphPane.XAxis.Scale.Max = 1056;
            this.collect2_graph.GraphPane.XAxis.Scale.Min = 0;
            this.collect2_graph.GraphPane.XAxis.Scale.Max = 1056;
        }

        private void Graph_OnAxisModeChanged()
        {
            if (isFromFile)
            {
                DisplayLoadedSpectrum(this.collect1_graph, this.collect2_graph, displayedFileName);
            }
            else
            {
                CreateGraph(this.collect1_graph, this.collect2_graph);
            }
        }
/*
        private void ChangeSpecterType()
        {
            if (Commander.pState == Commander.programStates.Measure) 
                preciseSpecterDisplayed = Commander.isSenseMeasure;
        }
*/        
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
            isFromFile = false;
            closeSpecterFileToolStripMenuItem.Enabled = false;
            ZedGraphRebirth(zgc1, Graph.Collector1, "Первый коллектор");
            ZedGraphRebirth(zgc2, Graph.Collector2, "Второй коллектор");
            /*
            specterSavingEnabled = false;
            isFromFile = false;
            string modeText = "(скан.)";
            preciseSpecterDisplayed = false;
            if (Commander.isSenseMeasure)
            {
                modeText = "(прециз.)";
                preciseSpecterDisplayed = true;
            }
            prevPreciseSpecterDisplayed = preciseSpecterDisplayed;
            closeSpecterFileToolStripMenuItem.Enabled = false;

            GraphPane myPane1 = zgc1.GraphPane;
            GraphPane myPane2 = zgc2.GraphPane;

            myPane1.Title.Text = "Первый коллектор " + modeText;
            myPane1.XAxis.Title.Text = "Масса";
            myPane1.YAxis.Title.Text = "Интенсивность";

            myPane2.Title.Text = "Второй коллектор " + modeText;
            myPane2.XAxis.Title.Text = "Масса";
            myPane2.YAxis.Title.Text = "Интенсивность";

            myPane1.CurveList.Clear();
            myPane2.CurveList.Clear();

            foreach (PointPairList ppl in Graph.pointLists1)
            {
                if (ppl.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp1 = myPane1.AddCurve("My Curve", ppl, Color.Blue, SymbolType.None);
                temp1.Symbol.Fill = new Fill(Color.White);
            }
            foreach (PointPairList ppl in Graph.pointLists2)
            {
                if (ppl.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp2 = myPane2.AddCurve("My Curve", ppl, Color.Red, SymbolType.None);
                temp2.Symbol.Fill = new Fill(Color.White);
            }

            if (Graph.pointList1.Count > 0 || Graph.pointList2.Count > 0)
                specterSavingEnabled = true;
            LineItem myCurve1 = myPane1.AddCurve("My Curve", Graph.pointList1, Color.Blue, SymbolType.None);
            LineItem myCurve2 = myPane2.AddCurve("My Curve", Graph.pointList2, Color.Red, SymbolType.None);

            myCurve1.Symbol.Fill = new Fill(Color.White);
            myCurve2.Symbol.Fill = new Fill(Color.White);
            
            myPane1.Legend.IsVisible = false;
            myPane2.Legend.IsVisible = false;

            // Fill the axis background with a color gradient
            myPane1.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            myPane2.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane1.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            myPane2.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            myPane1.YAxis.Scale.Min = 0;
            myPane1.YAxis.Scale.Max = 10000;

            myPane2.YAxis.Scale.Min = 0;
            myPane2.YAxis.Scale.Max = 10000;

            //myPane1.XAxis.Scale.Min = 0;
            //myPane1.XAxis.Scale.Max = 1056;

            //myPane2.XAxis.Scale.Min = 0;
            //myPane2.XAxis.Scale.Max = 1056;


            // Calculate the Axis Scale Ranges
            zgc1.AxisChange();
            zgc2.AxisChange();

            RefreshGraph();
            //if (myPane1.CurveList.Count > 0 || myPane2.CurveList.Count > 0)
            //    specterSavingEnabled = true;
            */
        }

        public void RefreshGraph()
        {
            collect1_graph.Refresh();
            collect2_graph.Refresh();
        }

        public void DisplayLoadedSpectrum(ZedGraphControlPlus zgc1, ZedGraphControlPlus zgc2, string fileName) 
        {
            displayedFileName = fileName;
            isFromFile = true;
            ZedGraphRebirth(zgc1, Graph.LoadedSpectra1, "Первый коллектор");
            ZedGraphRebirth(zgc2, Graph.LoadedSpectra2, "Второй коллектор");
            closeSpecterFileToolStripMenuItem.Enabled = true;
            /*
            specterSavingEnabled = false;
            isFromFile = true;
            string modeText = "(скан.)";
            if (preciseSpecterDisplayed) modeText = "(прециз.)"; 
            GraphPane myPane1 = zgc1.GraphPane;
            GraphPane myPane2 = zgc2.GraphPane;

            myPane1.Title.Text = "Первый коллектор " + modeText +":\n" + fileName;
            myPane1.XAxis.Title.Text = "Масса";
            myPane1.YAxis.Title.Text = "Интенсивность";

            myPane2.Title.Text = "Второй коллектор " + modeText + ":\n" + fileName;
            myPane2.XAxis.Title.Text = "Масса";
            myPane2.YAxis.Title.Text = "Интенсивность";

            myPane1.CurveList.Clear();
            myPane2.CurveList.Clear();

            foreach (PointPairList ppl in Graph.pointListsLoaded1)
            {
                if (ppl.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp1 = myPane1.AddCurve("My Curve", ppl, Color.Blue, SymbolType.None);
                temp1.Symbol.Fill = new Fill(Color.White);
            }
            foreach (PointPairList ppl in Graph.pointListsLoaded2)
            {
                if (ppl.Count > 0)
                    specterSavingEnabled = true;
                LineItem temp2 = myPane2.AddCurve("My Curve", ppl, Color.Red, SymbolType.None);
                temp2.Symbol.Fill = new Fill(Color.White);
            }

            if (Graph.pointListLoaded1.Count > 0 || Graph.pointListLoaded2.Count > 0)
                specterSavingEnabled = true;
            LineItem myCurve1 = myPane1.AddCurve("My Curve", Graph.pointListLoaded1, Color.Blue, SymbolType.None);
            LineItem myCurve2 = myPane2.AddCurve("My Curve", Graph.pointListLoaded2, Color.Red, SymbolType.None);

            myCurve1.Symbol.Fill = new Fill(Color.White);
            myCurve2.Symbol.Fill = new Fill(Color.White);

            //myBar1.Symbol.Fill = new Fill(Color.Blue);
            //myBar2.Symbol.Fill = new Fill(Color.Red);

            myPane1.Legend.IsVisible = false;
            myPane2.Legend.IsVisible = false;

            // Fill the axis background with a color gradient
            myPane1.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            myPane2.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane1.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            myPane2.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            myPane1.YAxis.Scale.Min = 0;
            myPane1.YAxis.Scale.Max = 10000;

            myPane2.YAxis.Scale.Min = 0;
            myPane2.YAxis.Scale.Max = 10000;

            myPane1.XAxis.Scale.Min = 0;
            myPane1.XAxis.Scale.Max = 1056;

            myPane2.XAxis.Scale.Min = 0;
            myPane2.XAxis.Scale.Max = 1056;


            // Calculate the Axis Scale Ranges
            zgc1.AxisChange();
            zgc2.AxisChange();

            RefreshGraph();
            //if (myPane1.CurveList.Count > 0 || myPane2.CurveList.Count > 0)
            //    specterSavingEnabled = true;
            */
        }
        
        private void GraphForm_Validating(object sender, CancelEventArgs e)
        {
            RefreshGraph();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
                // TODO: Add code here to save the current contents of the form to a file.
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (preciseSpecterDisplayed)
            {
                saveSpecterFileDialog.Filter = "Precise specter files (*.psf)|*.psf";
                saveSpecterFileDialog.DefaultExt = "psf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SavePreciseSpecterFile(saveSpecterFileDialog.FileName, isFromFile);
                }
            }
            else
            {
                saveSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf";
                saveSpecterFileDialog.DefaultExt = "sdf";
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SaveSpecterFile(saveSpecterFileDialog.FileName, isFromFile);
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
                case pListScaled.DisplayValue.Step:
                    myPane.XAxis.Title.Text = "Ступени";
                    break;
                case pListScaled.DisplayValue.Voltage:
                    myPane.XAxis.Title.Text = "Напряжение";
                    break;
                case pListScaled.DisplayValue.Mass:
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
            closeSpecterFileToolStripMenuItem.Enabled = false;
            CreateGraph(this.collect1_graph, this.collect2_graph);
            preciseSpecterDisplayed = prevPreciseSpecterDisplayed;
        }

        private void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                closeSpecterFileToolStripMenuItem.Enabled = true;
            }
        }
    }
}