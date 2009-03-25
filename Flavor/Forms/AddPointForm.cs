using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public partial class AddPointForm : Form
    {
        private Utility.PreciseEditorRow oneRow;
        
        public AddPointForm(ushort step, byte col): base()
        {
            InitializeComponent();

            this.oneRow = new Utility.PreciseEditorRow(13, 50);
            this.oneRow.StepText = step.ToString();
            this.oneRow.ColText = col.ToString();
            this.Controls.AddRange(oneRow.getControls());
        }
    }
}