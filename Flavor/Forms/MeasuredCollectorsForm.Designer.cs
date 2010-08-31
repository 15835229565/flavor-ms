namespace Flavor.Forms {
    partial class MeasuredCollectorsForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.closeSpecterFileToolStripMenuItem.Enabled = false;
            // 
            // MeasuredCollectorsForm
            // 
            this.Text = "Коллекторы";
            this.ControlBox = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MeasuredCollectorsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
