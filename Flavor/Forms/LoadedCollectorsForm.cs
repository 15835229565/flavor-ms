using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Controls;
using PreciseEditorData = Flavor.Common.Utility.PreciseEditorData;
using Graph = Flavor.Common.Graph;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm, ILoaded {
        private string displayedFileName;
        private string DisplayedFileName {
            get { return System.IO.Path.GetFileName(displayedFileName); }
        }
        protected LoadedCollectorsForm(): base() {
            // do not use! for designer only!
            InitializeComponent();
            Panel.Enable();
        }
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(graph, hint) {
            InitializeComponent();
            displayedFileName = fileName;
            this.Text = DisplayedFileName;

            Panel.Enable();

            if (PreciseSpectrumDisplayed) {
                // search temporary here
                // TODO: use extension method getUsed()
                setXScaleLimits(graph.PreciseData.FindAll(PreciseEditorData.PeakIsUsed));
            } else {
                ushort minX = (ushort)graph.Displayed1Steps[0][0].X;
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX, minX, maxX);
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

