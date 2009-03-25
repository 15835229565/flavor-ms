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
        
        public AddPointForm()
        {
            InitializeComponent();

            this.oneRow = new Utility.PreciseEditorRow(13, 50);
            this.Controls.AddRange(oneRow.getControls());
        }
    }
}