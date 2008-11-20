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
        public ZedGraphControlPlus(): base()
        {
            InitializeComponent();
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState)
        {
            /*
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                if ((string)item.Tag == "set_default")
                {
                    // remove the menu item
                    menuStrip.Items.Remove(item);
                    // or, just disable the item with this
                    //item.Enabled = false; 

                    break;
                }
            }
            */
            // create a new menu item
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
    }
}

