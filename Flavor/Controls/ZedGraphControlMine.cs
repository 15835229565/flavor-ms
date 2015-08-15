using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using ZedGraph;

namespace Flavor.Controls {
    public partial class ZedGraphControlMine: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            public ContextMenuStrip MenuStrip { get; private set; }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip) {
                MenuStrip = menuStrip;
            }
        }
        public ZedGraphControlMine() {
            InitializeComponent();
            var pane = GraphPane;
            pane.IsFontsScaled = false;
            pane.Legend.IsVisible = false;
            // Fill the axis background with a color gradient
            pane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45f);
            // Fill the pane background with a color gradient
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45f);

            base.ContextMenuBuilder += ZedGraphControlMine_ContextMenuBuilder;
        }
        void ZedGraphControlMine_ContextMenuBuilder(object sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            var saveAsItems = menuStrip.Items.Find("save_as", false);
            if (saveAsItems.Length > 0) {
                var saveAsItem = saveAsItems[0];
                saveAsItem.Click -= base.MenuClick_SaveAs;
                saveAsItem.Click += this.MenuClick_SaveAs;
            }
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
