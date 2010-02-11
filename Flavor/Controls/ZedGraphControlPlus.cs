using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls
{
    partial class ZedGraphControlPlus : ZedGraph.ZedGraphControl
    {
        internal delegate void DiffOnPointEventHandler(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference);
        internal event DiffOnPointEventHandler OnDiffOnPoint;
        private CurveItem curveReference;
        private int curveIndex = -1;
        private int pointIndex;
        
        internal ZedGraphControlPlus(): base()
        {
            InitializeComponent();
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState)
        {
            GraphPane pane = this.MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            int iNearest;
            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out iNearest))
            {
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
                curveReference = nearestCurve;
                pointIndex = iNearest;

                // create a new menu item for point
                ToolStripMenuItem item = new ToolStripMenuItem();
                // This is the user-defined Tag so you can find this menu item later if necessary
                item.Name = "point_add";
                item.Tag = "point_add";
                // This is the text that will show up in the menu
                item.Text = "Добавить точку в редактор";
                
                // Add a handler that will respond when that menu item is selected
                item.Click += new System.EventHandler(AddPointToPreciseEditor);
                // Add the menu item to the menu
                menuStrip.Items.Add(item);

                ToolStripMenuItem item1 = new ToolStripMenuItem();

                int curveIndex1 = Graph.Displayed1.IndexOf((PointPairListPlus)(nearestCurve.Points));
                int curveIndex2 = Graph.Displayed2.IndexOf((PointPairListPlus)(nearestCurve.Points));
                if (-1 != curveIndex1)
                {
                    curveIndex = curveIndex1;
                    item1.Name = "axis_rescale_coeff1";
                    item1.Tag = "axis_rescale_coeff1";
                    item1.Text = "Коэффициент коллектора 1";
                }
                else if (-1 != curveIndex2)
                {
                    curveIndex = curveIndex2;
                    item1.Name = "axis_rescale_coeff2";
                    item1.Tag = "axis_rescale_coeff2";
                    item1.Text = "Коэффициент коллектора 2";
                }
                else
                {
                    curveIndex = -1;
                    throw new Exception("Point not in any collector. Strange.");
                }

                item1.Click += new System.EventHandler(SetScalingCoeff);
                menuStrip.Items.Add(item1);

                if ((this.ParentForm as GraphForm).specterDiffEnabled)
                {
                    ToolStripMenuItem item2 = new ToolStripMenuItem();
                    item2.Name = "custom_diff";
                    item2.Tag = "custom_diff";
                    item2.Text = "Вычесть из текущего с перенормировкой на точку";
                    item2.Click += new System.EventHandler(DiffWithCoeff);
                    menuStrip.Items.Add(item2);
                    if (Graph.isPreciseSpectrum)
                    {
                        ToolStripMenuItem item3 = new ToolStripMenuItem();
                        item3.Name = "custom_diff_peak";
                        item3.Tag = "custom_diff_peak";
                        item3.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                        item3.Click += new System.EventHandler(DiffWithCoeff);
                        menuStrip.Items.Add(item3);
                    }
                }
            }
            else
            {
                ToolStripMenuItem stepViewItem = new ToolStripMenuItem();
                ToolStripMenuItem voltageViewItem = new ToolStripMenuItem();
                ToolStripMenuItem massViewItem = new ToolStripMenuItem();

                switch (Graph.AxisDisplayMode)
                {
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

        private void ViewItemCheckStateChanged(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem)sender).Name) 
            {
                case "axis_mode_step":
                    Graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Step;
                    break;
                case "axis_mode_voltage":
                    Graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Voltage;
                    break;
                case "axis_mode_mass":
                    Graph.AxisDisplayMode = Graph.pListScaled.DisplayValue.Mass;
                    break;
                default:
                    break;
            }
        }

        private void AddPointToPreciseEditor(object sender, EventArgs e) 
        {
            byte collector = 0;
            PointPair pp = null;
            int curveIndex1 = Graph.Displayed1.IndexOf((PointPairListPlus)(curveReference.Points));
            int curveIndex2 = Graph.Displayed2.IndexOf((PointPairListPlus)(curveReference.Points));
            if (-1 != curveIndex1)
            {
                collector = 1;
                pp = (Graph.Displayed1Steps[curveIndex1])[pointIndex];
            }
            else if (-1 != curveIndex2)
            {
                collector = 2;
                pp = (Graph.Displayed2Steps[curveIndex2])[pointIndex];
            }
            if ((pp != null) && (collector != 0))
            {
                if (new AddPointForm((ushort)(pp.X), collector).ShowDialog() == DialogResult.OK)
                {
                    PreciseOptionsForm pForm = PreciseOptionsForm.getInstance();
                    pForm.UpLevel = (mainForm)((GraphForm)(this.ParentForm)).MdiParent;
                    pForm.Show();
                    pForm.BringToFront();
                }
            }
            else
                MessageBox.Show("Не удалось корректно найти точку", "Ошибка");
        }
        private void DiffWithCoeff(object sender, EventArgs e)
        {
            Graph.pListScaled pls = ((PointPairListPlus)(curveReference.Points)).PLSreference;
            if (pls == null)
            {
                MessageBox.Show("Не удалось корректно найти точку", "Ошибка");
                return;
            }
            if (Graph.isPreciseSpectrum)
            {
                List<Utility.PreciseEditorData> peds = null;
                switch (Graph.DisplayingMode)
                {
                    case Graph.Displaying.Measured:
                        peds = Config.PreciseData;
                        break;
                    case Graph.Displaying.Loaded:
                        peds = Config.PreciseDataLoaded;
                        break;
                    case Graph.Displaying.Diff:
                        //peds = Config.PreciseDataDiff;
                        //break;
                        throw new ArgumentOutOfRangeException();
                }
                Utility.PreciseEditorData ped = pls.PEDreference;
                if (ped == null)
                {
                    MessageBox.Show("Не удалось корректно найти точку", "Ошибка");
                    return;
                }
                if (((ToolStripMenuItem)sender).Name == "custom_diff_peak")
                {
                    //!!!! modify!
                    OnDiffOnPoint(ushort.MaxValue, null, ped);
                }
                else
                {
                    OnDiffOnPoint((ushort)pls.Step[pointIndex].X, pls, ped);
                }
            }
            else
            {
                OnDiffOnPoint((ushort)pls.Step[pointIndex].X, pls, null);
            }
        }
        private void SetScalingCoeff(object sender, EventArgs e)
        {
            byte collector = 0;
            PointPair pp = null;
            if (((ToolStripMenuItem)sender).Name == "axis_rescale_coeff1")
            {
                collector = 1;
                pp = (Graph.Displayed1Steps[curveIndex])[pointIndex];
            }
            else if (((ToolStripMenuItem)sender).Name == "axis_rescale_coeff2")
            {
                collector = 2;
                pp = (Graph.Displayed2Steps[curveIndex])[pointIndex];
            }
            if ((pp != null) && (collector != 0))
            {
                if (new SetScalingCoeffForm((ushort)(pp.X), collector).ShowDialog() == DialogResult.OK)
                {
                    //Recompute of mass rows
                    //Repaint with new coeffs if needed (mass displaying mode)
                    //Implemented in Config & Graph respectively
                }
            }
            else
                MessageBox.Show("Не удалось корректно найти точку", "Ошибка");
        }

        private string ZedGraphControlPlus_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            string tooltipData = "";
            PointPair pp = curve[iPt];
            switch (Graph.AxisDisplayMode)
            {
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
            if (Graph.isPreciseSpectrum)
            {
                int curveIndex1 = Graph.Displayed1.IndexOf((PointPairListPlus)(curve.Points));
                int curveIndex2 = Graph.Displayed2.IndexOf((PointPairListPlus)(curve.Points));
                long peakSum = -1;
                if (-1 != curveIndex1)
                {
                    peakSum = Graph.DisplayedRows1[curveIndex1].PeakSum;
                }
                else if (-1 != curveIndex2)
                {
                    peakSum = Graph.DisplayedRows2[curveIndex2].PeakSum;
                }
                if (peakSum != -1)
                    tooltipData += string.Format("\nИнтеграл пика: {0:G}", peakSum);
                else
                    tooltipData += "\nНе удалось идентифицировать пик";
            }
            return tooltipData;
        }
    }
}

