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
            this.SuspendLayout();

            this.pNumComboBox = new ComboBox();
            this.pNumComboBox.Location = new Point(174, 64);
            this.pNumComboBox.Size = new Size(40, 23);
            this.pNumComboBox.SelectedIndexChanged += new EventHandler(pNumComboBox_SelectedIndexChanged);

            this.okButton.Enabled = false;

            this.Text = "Вставка точки";
            this.Controls.Add(this.pNumComboBox);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private ComboBox pNumComboBox;
    }
}
