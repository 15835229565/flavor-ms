using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm, ILoaded {
        private string displayedFileName;
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(graph, hint) {
            InitializeComponent();
            this.Text = displayedFileName = fileName;

            Panel.Enable();

            if (PreciseSpectrumDisplayed) {
                setXScaleLimits(graph.PreciseData);
            } else {
                ushort minX = (ushort)(graph.Displayed1Steps[0][0].X);
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX, minX, maxX);
            }
        }
        /*protected override MeasurePanel initPanel() {
            MeasurePanel panel = new MeasurePanel();
			return panel;
		}*/
        protected sealed override void updateOnModification() {
            this.Text = displayedFileName + (Modified ? "*" : "");
            base.updateOnModification();
        }
        protected sealed override bool saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
            bool res = base.saveData();
            if (res) {
                displayedFileName = saveSpecterFileDialog.FileName;
                this.Text = displayedFileName;
            }
            return res;
		}

        void LoadedCollectorsForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            if (Modified) {
                Activate();
                DialogResult res = MessageBox.Show(this.MdiParent, "Спектр изменен и не сохранен. Сохранить?", displayedFileName, MessageBoxButtons.YesNoCancel);
                switch (res) {
                    case DialogResult.Yes:
                        e.Cancel = !saveData();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        #region ILoaded Members

        public string FileName {
            get { return displayedFileName; }
        }

        #endregion
    }
}

