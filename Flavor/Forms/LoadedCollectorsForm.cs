using System;
using System.Windows.Forms;
using Flavor.Controls;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
using Graph = Flavor.Common.Data.Measure.Graph;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm2, ILoaded {
        private string displayedFileName;
        private string DisplayedFileName {
            get { return System.IO.Path.GetFileName(displayedFileName); }
        }
        [Obsolete]
        protected LoadedCollectorsForm(): base() {
            // do not use! for designer only!
            InitializeComponent();
            Panel = new GraphPanel();
        }
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(graph, hint) {
            InitializeComponent();
            // TODO: different for precise & scan
            Panel = new GraphPanel();
            Panel.Graph = graph;
            Panel.Enable();

            displayedFileName = fileName;
            this.Text = DisplayedFileName;

            if (PreciseSpectrumDisplayed) {
                // search temporary here
                // TODO: use extension method getUsed()
                setXScaleLimits(graph.PreciseData.FindAll(PreciseEditorData.PeakIsUsed));
            } else {
                ushort minX = (ushort)graph.Displayed1Steps[0][0].X;
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX);
            }
        }
        protected sealed override void updateOnModification() {
            this.Text = DisplayedFileName + (Modified ? "*" : "");
            base.updateOnModification();
        }
        protected sealed override bool saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
            bool res = base.saveData();
            if (res) {
                displayedFileName = saveSpecterFileDialog.FileName;
                this.Text = DisplayedFileName;
            }
            return res;
		}

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (Modified) {
                Activate();
                switch (MessageBox.Show(this.MdiParent, "Спектр изменен и не сохранен. Сохранить?", displayedFileName, MessageBoxButtons.YesNoCancel)) {
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

