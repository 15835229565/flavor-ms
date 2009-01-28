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

using System.Runtime.InteropServices;
using ZedGraph;

namespace Flavor
{
    public partial class ZedGraphControlPlus : ZedGraph.ZedGraphControl
    {
        private CurveItem curveReference;
        private int pointIndex;
        //private bool addToPE = false;
        
        public ZedGraphControlPlus(): base()
        {
            InitializeComponent();
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState)
        {
            GraphPane pane = this.MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            int iNearest;
            if (pane.FindNearestPoint(mousePt, out nearestCurve, out iNearest))
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
                // create a new menu item for point
                ToolStripMenuItem item = new ToolStripMenuItem();
                // This is the user-defined Tag so you can find this menu item later if necessary
                item.Name = "point_add";
                item.Tag = "point_add";
                // This is the text that will show up in the menu
                item.Text = "Add point to precise editor";
                
                curveReference = nearestCurve;
                pointIndex = iNearest;
                // Add a handler that will respond when that menu item is selected
                item.Click += new System.EventHandler(AddPointToPreciseEditor);
                // Add the menu item to the menu
                menuStrip.Items.Add(item);
            }
            else
            {
                // create a new general menu item
                ToolStripMenuItem item = new ToolStripMenuItem();
                // This is the user-defined Tag so you can find this menu item later if necessary
                item.Name = "custom_zoom";
                item.Tag = "custom_zoom";
                // This is the text that will show up in the menu
                item.Text = "Do Something Special";
                // Add a handler that will respond when that menu item is selected
                item.Click += new System.EventHandler(CustomZoom);
                // Add the menu item to the menu
                menuStrip.Items.Add(item);
            }
        }

        private void CustomZoom(object sender, EventArgs e)
        {
            //ContextMenuStrip temp1 = (ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent();
            //object temp2 = temp1.GetContainerControl();
            //this.GraphPane.Title = "Changed!";
            (new CustomZoomOptions(this)).ShowDialog();
            if (this.MasterPane != null)
            {
                GraphPane gpane = this.GraphPane;
                if (gpane != null && !gpane.ZoomStack.IsEmpty)
                {
                    ZoomState.StateType type = gpane.ZoomStack.Top.Type;

                    ZoomState oldState = new ZoomState(gpane, type);
                    ZoomState newState = null;
                    if (this.IsSynchronizeXAxes || this.IsSynchronizeYAxes)
                    {
                        foreach (GraphPane pane in this.MasterPane.PaneList)
                        {
                            ZoomState state = pane.ZoomStack.Pop(pane);
                            if (pane == gpane)
                                newState = state;
                        }
                    }
                    else
                        newState = gpane.ZoomStack.Pop(gpane);

                    // Provide Callback to notify the user of zoom events
                    //if (this.ZoomEvent != null)
                    //    this.ZoomEvent(this, oldState, newState);

                    Refresh();
                }
            }
        }
        private void AddPointToPreciseEditor(object sender, EventArgs e) 
        {
            MessageBox.Show(pointIndex.ToString(), curveReference.NPts.ToString());
        }
    }
}

