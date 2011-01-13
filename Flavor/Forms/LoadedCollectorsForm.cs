using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

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
                setXScaleLimits(graph.PreciseData.FindAll(Utility.PreciseEditorData.PeakIsUsed));
            } else {
                ushort minX = (ushort)graph.Displayed1Steps[0][0].X;
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX, minX, maxX);
            }
        }
        /*protected override MeasurePanel initPanel() {
            MeasurePanel panel = new MeasurePanel();
			return panel;
		}*/
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
                        if (!saveData())
                            return;
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
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

