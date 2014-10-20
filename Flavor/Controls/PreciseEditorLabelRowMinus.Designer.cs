using System.Windows.Forms;
using System.ComponentModel;

namespace Flavor.Controls {
    partial class PreciseEditorLabelRowMinus {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            Label label10;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PreciseEditorLabelRowMinus));
            Label colNumLabel;
            this.peakCenterLabel = new Label();
            this.colToolTip = new ToolTip(this.components);
            label10 = new Label();
            colNumLabel = new Label();
            this.SuspendLayout();
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // colNumLabel
            // 
            resources.ApplyResources(colNumLabel, "colNumLabel");
            colNumLabel.Name = "colNumLabel";
            this.colToolTip.SetToolTip(colNumLabel, resources.GetString("colNumLabel.ToolTip"));
            // 
            // peakCenterLabel
            // 
            resources.ApplyResources(this.peakCenterLabel, "peakCenterLabel");
            this.peakCenterLabel.Name = "peakCenterLabel";
            // 
            // PreciseEditorLabelRowMinus
            // 
            this.Controls.Add(this.peakCenterLabel);
            this.Controls.Add(colNumLabel);
            this.Controls.Add(label10);
            this.Name = "PreciseEditorLabelRowMinus";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        protected Label peakCenterLabel;
        private ToolTip colToolTip;
    }
}
