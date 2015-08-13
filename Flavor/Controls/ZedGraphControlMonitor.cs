using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using ZedGraph;

namespace Flavor.Controls {
    public partial class ZedGraphControlMonitor: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            public ContextMenuStrip MenuStrip { get; private set; }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip) {
                MenuStrip = menuStrip;
            }
        }
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;
        protected virtual void OnContextMenuBuilder(ContextMenuBuilderEventArgs e) {
            ContextMenuBuilder.Raise(this, e);
        }
        
        const string LOG_ITEM_TEXT = "Логарифмическая шкала";
        public ZedGraphControlMonitor()
            : base() {
            InitializeComponent();
            var pane = GraphPane;
            pane.IsFontsScaled = false;
            pane.Legend.IsVisible = false;
            pane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);
            var xAxis = pane.XAxis;
            var yAxis = pane.YAxis;
            var grid = xAxis.MinorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 1;
            grid.DashOff = 2;

            grid = yAxis.MinorGrid;
            grid.Color = Color.Gray;
            grid.IsVisible = true;
            grid.DashOn = 1;
            grid.DashOff = 2;

            grid = xAxis.MajorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 10;
            grid.DashOff = 5;

            grid = yAxis.MajorGrid;
            grid.IsVisible = true;
            grid.Color = Color.Gray;
            grid.DashOn = 10;
            grid.DashOff = 5;
            
            base.ContextMenuBuilder += ZedGraphControlMonitor_ContextMenuBuilder;
        }
        void ZedGraphControlMonitor_ContextMenuBuilder(object sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            var saveAsItems = menuStrip.Items.Find("save_as", false);
            if (saveAsItems.Length > 0) {
                var saveAsItem = saveAsItems[0];
                saveAsItem.Click -= base.MenuClick_SaveAs;
                saveAsItem.Click += this.MenuClick_SaveAs;
            }

            var yAxis = GraphPane.YAxis;
            var item = new ToolStripMenuItem(LOG_ITEM_TEXT) {
                Checked = yAxis.Type == AxisType.Log,
                CheckOnClick = true
            };
            item.CheckedChanged += (s, e) => {
                yAxis.Type = ((ToolStripMenuItem)s).Checked ? AxisType.Log : AxisType.Linear;
                Refresh();
            };

            menuStrip.Items.Add(item);

            OnContextMenuBuilder(new ContextMenuBuilderEventArgs(menuStrip));
        }
        protected new void MenuClick_SaveAs(object sender, EventArgs e) {
            if (MasterPane != null) {
                SaveFileDialog.Filter =
                    "Bmp Format (*.bmp)|*.bmp|" +
                    "PNG Format (*.png)|*.png|" +
                    "Gif Format (*.gif)|*.gif|" +
                    "Jpeg Format (*.jpg)|*.jpg|" +
                    "Tiff Format (*.tif)|*.tif|" +
                    "Emf Format (*.emf)|*.emf";

                if (SaveFileDialog.FileName != null && SaveFileDialog.FileName.Length > 0) {
                    string ext = Path.GetExtension(SaveFileDialog.FileName).ToLower();
                    switch (ext) {
                        case ".emf": SaveFileDialog.FilterIndex = 6; break;
                        case ".png": SaveFileDialog.FilterIndex = 2; break;
                        case ".gif": SaveFileDialog.FilterIndex = 3; break;
                        case ".jpeg":
                        case ".jpg": SaveFileDialog.FilterIndex = 4; break;
                        case ".tiff":
                        case ".tif": SaveFileDialog.FilterIndex = 5; break;
                        case ".bmp": SaveFileDialog.FilterIndex = 1; break;
                    }
                }

                if (SaveFileDialog.ShowDialog() == DialogResult.OK) {
                    Stream myStream = SaveFileDialog.OpenFile();
                    if (myStream != null) {
                        if (SaveFileDialog.FilterIndex == 6) {
                            myStream.Close();
                            //base.SaveEmfFile(SaveFileDialog.FileName);
                        } else {
                            ImageFormat format = ImageFormat.Png;
                            switch (SaveFileDialog.FilterIndex) {
                                case 2: format = ImageFormat.Png; break;
                                case 3: format = ImageFormat.Gif; break;
                                case 4: format = ImageFormat.Jpeg; break;
                                case 5: format = ImageFormat.Tiff; break;
                                case 1: format = ImageFormat.Bmp; break;
                            }

                            MasterPane.GetImage(MasterPane.IsAntiAlias).Save(myStream, format);
                            myStream.Close();
                        }
                    }
                }
            }
        }
    }
}

