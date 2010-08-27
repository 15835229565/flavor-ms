using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls {
    public partial class ZedGraphControlPlus: ZedGraphControl {
        internal delegate void DiffOnPointEventHandler(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference);
        internal event DiffOnPointEventHandler OnDiffOnPoint;
        private CurveItem curveReference;
        internal CurveItem CurveReference {
            get { return curveReference; }
            set { curveReference = value; }
        }
        /*private int curveIndex = -1;
        internal int CurveIndex {
            get { return curveIndex; }
            set { curveIndex = value; }
        }*/
        private int pointIndex;
        internal int PointIndex {
            get { return pointIndex; }
            set { pointIndex = value; }
        }
        private bool isFirstCollector;
        internal bool IsFirstCollector {
            get { return isFirstCollector; }
            set { isFirstCollector = value; }
        }

        public ZedGraphControlPlus()
            : base() {
            InitializeComponent();
        }
        internal void AddPointToPreciseEditor(object sender, EventArgs e) {
            // TODO: raise event here and move code below to mainform
            if (new AddPointForm((ushort)(curveReference[pointIndex].X), (byte)(isFirstCollector ? 1 : 2)).ShowDialog() == DialogResult.OK) {
                /*PreciseOptionsForm pForm = PreciseOptionsForm.getInstance();
                pForm.UpLevel = (mainForm)((GraphForm)(this.ParentForm)).MdiParent;
                pForm.Show();
                pForm.BringToFront();*/
            }
        }
        internal void DiffWithCoeff(object sender, EventArgs e) {
            // can be NullPointerExceptions here..
            Graph.pListScaled pls = (curveReference.Points as PointPairListPlus).PLSreference;
            Utility.PreciseEditorData ped = pls.PEDreference;
            if (((ToolStripMenuItem)sender).Name == "custom_diff_peak") {
                //!!!! modify!
                OnDiffOnPoint(ushort.MaxValue, null, ped);
                return;
            }
            //must be: scan - (pnt, pls, null), precise - (pnt, pls, ped)
            OnDiffOnPoint((ushort)pls.Step[pointIndex].X, pls, ped);
        }
        internal void SetScalingCoeff(object sender, EventArgs e) {
            // TODO: raise event here and move code below to mainform
            if (new SetScalingCoeffForm((ushort)(curveReference[pointIndex].X), (byte)(isFirstCollector ? 1 : 2)).ShowDialog() == DialogResult.OK) {
                //Recompute of mass rows
                //Repaint with new coeffs if needed (mass displaying mode)
                //Implemented in Config & Graph respectively
            }
        }
    }
}

