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

namespace Flavor
{
    public partial class ZedGraphControlPlus : ZedGraph.ZedGraphControl
    {
        private CurveItem curveReference;
        private int curveIndex = -1;
        private int pointIndex;
        
        public ZedGraphControlPlus(): base()
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

                int curveIndex1 = Graph.Displayed1.IndexOf((PointPairList)(nearestCurve.Points));
                int curveIndex2 = Graph.Displayed2.IndexOf((PointPairList)(nearestCurve.Points));
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
            int curveIndex1 = Graph.Displayed1.IndexOf((PointPairList)(curveReference.Points));
            //List<PointPairList> l1 = Graph.Displayed1Steps;
            int curveIndex2 = Graph.Displayed2.IndexOf((PointPairList)(curveReference.Points));
            //List<PointPairList> l2 = Graph.Displayed2Steps;
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
    }
}

