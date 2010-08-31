using System.Windows.Forms;
using System.Drawing;
using System;
namespace Flavor.Forms {
    partial class PlacePointForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlacePointForm));
            this.pNumComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            // 
            // pNumComboBox
            // 
            resources.ApplyResources(this.pNumComboBox, "pNumComboBox");
            this.pNumComboBox.Name = "pNumComboBox";
            this.pNumComboBox.SelectedIndexChanged += new System.EventHandler(this.pNumComboBox_SelectedIndexChanged);
            // 
            // PlacePointForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.pNumComboBox);
            this.Name = "PlacePointForm";
            this.Controls.SetChildIndex(this.pNumComboBox, 0);
            this.Controls.SetChildIndex(this.oneRow, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox pNumComboBox;
    }
}
