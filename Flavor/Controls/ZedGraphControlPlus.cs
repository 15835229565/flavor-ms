using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls
{
    public partial class ZedGraphControlPlus : ZedGraphControl {
        public delegate void DiffOnPointEventHandler(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference);
        public event DiffOnPointEventHandler OnDiffOnPoint;

        private Graph.pListScaled pls;
        public Graph.pListScaled CurveRef {
            set { pls = value; }
        }
        private int pointIndex;
        public int PointIndex {
            set { pointIndex = value; }
        }
        private bool isFirstCollector;
        public bool IsFirstCollector {
            get { return isFirstCollector; }
            set { isFirstCollector = value; }
        }

        public ZedGraphControlPlus() : base() {
            InitializeComponent();
        }
        internal void AddPointToPreciseEditor(object sender, EventArgs e) {
            // TODO: raise event here and move code below to mainform
            // can be NullPointerExceptions here..
            new AddPointForm((ushort)(pls.Step[pointIndex].X), (byte)(isFirstCollector ? 1 : 2)).ShowDialog();
        }
        internal void DiffWithCoeff(object sender, EventArgs e) {
            // can be NullPointerExceptions here..
            Utility.PreciseEditorData ped = pls.PEDreference;
            if (((ToolStripMenuItem)sender).Name == CollectorsForm.DIFF_ON_PEAK_TAG) {
                //!!!! modify!
                OnDiffOnPoint(ushort.MaxValue, null, ped);
                return;
            }
            //must be: scan - (pnt, pls, null), precise - (pnt, pls, ped)
            OnDiffOnPoint((ushort)pls.Step[pointIndex].X, pls, ped);
        }
        internal void SetScalingCoeff(object sender, EventArgs e) {
            // TODO: raise event here and move code below to mainform
            // can be NullPointerExceptions here..
            new SetScalingCoeffForm((ushort)(pls.Step[pointIndex].X), (byte)(isFirstCollector ? 1 : 2)).ShowDialog();
        }
    }
}

