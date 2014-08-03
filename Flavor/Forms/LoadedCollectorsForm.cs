using System;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common.Data.Measure;

namespace Flavor.Forms {
    partial class LoadedCollectorsForm: CollectorsForm2, ILoaded {
        string displayedFileName;
        string DisplayedFileName {
            get { return System.IO.Path.GetFileName(displayedFileName); }
        }
        [Obsolete]
        protected LoadedCollectorsForm(): base() {
            // do not use! for designer only!
            // Init panel before ApplyResources
            Panel = new GraphPanel();
            InitializeComponent();
        }
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(graph, hint) {
            // Init panel before ApplyResources
            Panel = new GraphPanel { Graph = graph };
            InitializeComponent();
            // TODO: different for precise & scan
            Panel.Enable();

            displayedFileName = fileName;
            Text = DisplayedFileName;

            if (PreciseSpectrumDisplayed) {
                // search temporary here
                setXScaleLimits(graph.PreciseData.getUsed());
            } else {
                var data = graph.Collectors[0][0].Step;
                ushort minX = (ushort)data[0].X;
                ushort maxX = (ushort)(minX - 1 + data.Count);
                //ushort minX = (ushort)graph.Displayed1Steps[0][0].X;
                //ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX);
            }
        }
        protected sealed override void updateOnModification() {
            Text = DisplayedFileName + (Modified ? "*" : "");
            base.updateOnModification();
        }
        protected sealed override bool saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
            bool res = base.saveData();
            if (res) {
                displayedFileName = saveSpecterFileDialog.FileName;
                Text = DisplayedFileName;
            }
            return res;
		}

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (Modified) {
                Activate();
                switch (MessageBox.Show(MdiParent, "Спектр изменен и не сохранен. Сохранить?", displayedFileName, MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Yes:
                        if (!saveData()) {
                            e.Cancel = true;
                            return;
                        }
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            base.OnFormClosing(e);
        }
        #region ILoaded Members
        public string FileName {
            get { return displayedFileName; }
        }
        #endregion
    }
}

