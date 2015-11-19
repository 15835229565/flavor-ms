using System;
using System.IO;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common.Data.Measure;

namespace Flavor.Forms {
    partial class LoadedCollectorsForm: CollectorsForm2, ILoaded {
        string _fileName;
        string DisplayedFileName {
            get { return Path.GetFileName(_fileName); }
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
            Panel = new GraphPanel();
            InitializeComponent();
            // TODO: different for precise & scan
            Panel.Init(graph);

            _fileName = fileName;
            Text = DisplayedFileName;
        }
        protected override bool DisableTabPage(Collector collector) {
            if (PreciseSpectrumDisplayed)
                return collector.TrueForAll(list => list.isEmpty);
            return false;
        }
        protected sealed override void updateOnModification() {
            Text = DisplayedFileName + (Modified ? "*" : "");
            base.updateOnModification();
        }
        protected sealed override bool saveData() {
            saveSpecterFileDialog.InitialDirectory = Path.GetDirectoryName(_fileName);
            saveSpecterFileDialog.FileName = Path.GetFileNameWithoutExtension(_fileName);
            bool res = base.saveData();
            if (res) {
                _fileName = saveSpecterFileDialog.FileName;
                Text = DisplayedFileName;
            }
            return res;
		}

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (Modified) {
                Activate();
                switch (MessageBox.Show(MdiParent, "Спектр изменен и не сохранен. Сохранить?", _fileName, MessageBoxButtons.YesNoCancel)) {
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
            get { return _fileName; }
        }
        #endregion
    }
}

