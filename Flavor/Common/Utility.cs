using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Flavor.Common {
    internal class CommonOptions {
        // defaults
        private ushort beforeTime = 100;
        private ushort forwardTime = 100;
        private ushort backwardTime = 400;
        private bool forwardAsBefore = false;

        internal ushort befTime {
            get { return beforeTime; }
            set { beforeTime = value; }
        }
        internal ushort befTimeReal {
            get { return (ushort)(beforeTime * 5); }
            set {
                beforeTime = (ushort)(value / 5);
            }
        }
        internal ushort fTime {
            get { return forwardTime; }
            set { forwardTime = value; }
        }
        internal ushort fTimeReal {
            get { return (ushort)(forwardTime * 5); }
            set {
                forwardTime = (ushort)(value / 5);
            }
        }
        internal ushort bTime {
            get { return backwardTime; }
            set { backwardTime = value; }
        }
        internal ushort bTimeReal {
            get { return (ushort)(backwardTime * 5); }
            set {
                backwardTime = (ushort)(value / 5);
            }
        }
        internal bool ForwardTimeEqualsBeforeTime {
            get { return forwardAsBefore; }
            set { forwardAsBefore = value; }
        }

        // defaults
        private ushort expTime = 200;
        private ushort idleTime = 10;
        private ushort ionizationVoltage = 1911;
        private ushort CPVoltage = 3780;
        private ushort heatCurrent = 0;
        private ushort emissionCurrent = 79;
        private ushort focusVoltage1 = 2730;
        private ushort focusVoltage2 = 2730;

        internal ushort eTime {
            get { return expTime; }
            set { expTime = value; }
        }
        internal ushort eTimeReal {
            get { return (ushort)(expTime * 5); }
            set {
                expTime = (ushort)(value / 5);
            }
        }

        internal ushort iTime {
            get { return idleTime; }
            set { idleTime = value; }
        }
        internal ushort iTimeReal {
            get { return (ushort)(5 * idleTime); }
            set {
                idleTime = (ushort)(value / 5);
            }
        }

        internal ushort iVoltage {
            get { return ionizationVoltage; }
            set { ionizationVoltage = value; }
        }
        internal double iVoltageReal {
            get { return iVoltageConvert(ionizationVoltage); }
            set {
                ionizationVoltage = iVoltageConvert(value);
            }
        }
        internal double iVoltageConvert(ushort voltage) {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort iVoltageConvert(double voltage) {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort CP {
            get { return CPVoltage; }
            set { CPVoltage = value; }
        }
        internal double CPReal {
            get { return CPConvert(CPVoltage); }
            set {
                CPVoltage = CPConvert(value);
            }
        }
        internal double CPConvert(ushort coeff) {
            return (double)((10 / (double)coeff) * 4096);
        }
        internal ushort CPConvert(double coeff) {
            ushort x = (ushort)((10 / coeff) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort eCurrent {
            get { return emissionCurrent; }
            set { emissionCurrent = value; }
        }
        internal double eCurrentReal {
            get { return eCurrentConvert(emissionCurrent); }
            set {
                emissionCurrent = eCurrentConvert(value);
            }
        }
        internal double eCurrentConvert(ushort current) {
            return (double)((50 * (double)current) / 4096);
        }
        internal ushort eCurrentConvert(double current) {
            ushort x = (ushort)((current / 50) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort hCurrent {
            get { return heatCurrent; }
            set { heatCurrent = value; }
        }
        internal double hCurrentReal {
            get { return hCurrentConvert(heatCurrent); }
            set {
                heatCurrent = hCurrentConvert(value);
            }
        }
        internal double hCurrentConvert(ushort current) {
            return (double)((double)current / 4096);
        }
        internal ushort hCurrentConvert(double current) {
            ushort x = (ushort)(current * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort fV1 {
            get { return focusVoltage1; }
            set { focusVoltage1 = value; }
        }
        internal double fV1Real {
            get { return fV1Convert(focusVoltage1); }
            set {
                focusVoltage1 = fV1Convert(value);
            }
        }
        internal double fV1Convert(ushort voltage) {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort fV1Convert(double voltage) {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort fV2 {
            get { return focusVoltage2; }
            set { focusVoltage2 = value; }
        }
        internal double fV2Real {
            get { return fV2Convert(focusVoltage2); }
            set {
                focusVoltage2 = fV2Convert(value);
            }
        }
        internal double fV2Convert(ushort voltage) {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort fV2Convert(double voltage) {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort scanVoltage(ushort step) {
            if (step > Config.MAX_STEP) step = Config.MAX_STEP;
            return Convert.ToUInt16(4095 * Math.Pow(((double)527 / (double)528), 1056 - step));
            //if (step <= 456) return (ushort)(4095 - 5 * step);
            //return (ushort)(4095 - 5 * 456 - 2 * (step - 456));
        }
        internal double scanVoltageReal(ushort step) {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }
    }

    internal class Spectrum: List<List<Graph.pListScaled>> {
        private CommonOptions myCommonOptions = null;
        internal Spectrum(CommonOptions cd)
            : this() {
            // better to clone here?
            myCommonOptions = cd;
        }
        internal Spectrum()
            : base() {
            List<Graph.pListScaled> temp1 = new List<Graph.pListScaled>();
            List<Graph.pListScaled> temp2 = new List<Graph.pListScaled>();
            this.Add(temp1);
            this.Add(temp2);
        }
        internal CommonOptions CommonOptions {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }

    internal class PreciseSpectrum: List<Utility.PreciseEditorData> {
        private CommonOptions myCommonOptions = null;
        internal PreciseSpectrum(CommonOptions cd)
            : this() {
            // better to clone here?
            myCommonOptions = cd;
        }
        internal PreciseSpectrum()
            : base() {
        }
        internal CommonOptions CommonOptions {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }

    internal class PointPairListPlus: ZedGraph.PointPairList {
        private Utility.PreciseEditorData myPED;
        private Graph.pListScaled myPLS;

        internal Utility.PreciseEditorData PEDreference {
            get { return myPED; }
            set { myPED = value; }
        }
        internal Graph.pListScaled PLSreference {
            get { return myPLS; }
            set { myPLS = value; }
        }

        internal PointPairListPlus()
            : base() {
            myPED = null;
            myPLS = null;
        }
        internal PointPairListPlus(Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base() {
            myPED = ped;
            myPLS = pls;
        }
        internal PointPairListPlus(PointPairListPlus other, Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base(other) {
            myPED = ped;
            myPLS = pls;
        }
    }
    #region TreeNodes
    internal class TreeNodePlus: System.Windows.Forms.TreeNode {
        internal enum States {
            Ok,
            Warning,
            Error
        }
        protected States myState = States.Ok;
        internal virtual States State {
            get { return myState; }
            set {
                if (myState != value) {
                    States previous = myState;
                    myState = value;
                    if (Parent is TreeNodePlus) {
                        (Parent as TreeNodePlus).computeState(previous, myState);
                    }
                    setStateImageKey();
                }
            }
        }
        internal TreeNodePlus(string text, TreeNode[] nodes)
            : base(text, nodes) { }
        protected TreeNodePlus()
            : base() { }
        private void setStateImageKey() {
            switch (myState) {
                case States.Ok:
                    StateImageKey = "";
                    break;
                case States.Warning:
                    StateImageKey = "warning";
                    break;
                case States.Error:
                    StateImageKey = "error";
                    break;
            }
        }
        private void computeState(States previous, States current) {
            if (myState < previous) {
                // illegal state
                throw new InvalidOperationException();
            }
            if (myState < current) {
                State = current;
                return;
            }
            if (myState > current) {
                if (previous < current) {
                    return;
                }
                State = computeState(current);
            }
        }
        private States computeState(States hint) {
            States result = hint;
            foreach (TreeNodePlus node in Nodes) {
                if (result < node.State) {
                    result = node.State;
                    if (result == States.Error)
                        return result;
                }
            }
            return result;
        }
    }
    internal class TreeNodeLeaf: TreeNodePlus {
        internal override States State {
            get { return myState; }
            set {
                if (myState != value) {
                    myState = value;
                    if (Parent is TreeNodePair) {
                        (Parent as TreeNodePair).State = value;
                    }
                    setForeColor();
                }
            }
        }
        private new TreeNodeCollection Nodes {
            get { return base.Nodes; }
        }
        /*internal new string Text
        {
            get { return base.Text; }
            set 
            {
                if (base.Text != value)
                    base.Text = value;
            }
        }*/

        private void setForeColor() {
            switch (State) {
                case States.Ok:
                    ForeColor = Color.Green;
                    break;
                case States.Warning:
                    ForeColor = Color.Blue;
                    break;
                case States.Error:
                    ForeColor = Color.Red;
                    break;
            }
        }
        internal TreeNodeLeaf()
            : base() {
            setForeColor();
        }
    }
    internal class TreeNodePair: TreeNodePlus {
        private new TreeNodeCollection Nodes {
            get { return base.Nodes; }
        }
        internal TreeNodePair(string text, TreeNodeLeaf valueNode)
            : base() {
            Text = text;
            Nodes.Add(valueNode);
        }
    }
    #endregion
    internal static class Utility {
        #region PreciseEditorData
        internal class PreciseEditorData {
            internal PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr) {
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
            }
            internal PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                : this(pn, st, co, it, wi, pr) {
                comment = comm;
            }
            internal PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                : this(pn, st, co, it, wi, pr, comm) {
                usethis = useit;
            }
            internal PreciseEditorData(PreciseEditorData other)
                : this(other.usethis, other.pointNumber, other.step, other.collector, other.iterations, other.width, other.precision, other.comment) {
                associatedPoints = new PointPairListPlus(other.associatedPoints, this, null);
            }
            private bool usethis = true;
            private byte pointNumber;
            private ushort step;
            private byte collector;
            private ushort iterations;
            private ushort width;
            private float precision;
            private string comment = "";
            private PointPairListPlus associatedPoints = null;
            internal PointPairListPlus AssociatedPoints {
                get { return associatedPoints; }
                set {
                    if (value.PEDreference == null) {
                        associatedPoints = value;
                        associatedPoints.PEDreference = this;
                    } else
                        associatedPoints = new PointPairListPlus(value, this, null);
                }
            }
            internal bool Use {
                get { return usethis; }
                //set { usethis = value; }
            }
            internal byte pNumber {
                get { return pointNumber; }
                //set { pointNumber = value; }
            }
            internal ushort Step {
                get { return step; }
                //set { step = value; }
            }
            internal byte Collector {
                get { return collector; }
                //set { collector = value; }
            }
            internal ushort Iterations {
                get { return iterations; }
                //set { iterations = value; }
            }
            internal ushort Width {
                get { return width; }
                //set { width = value; }
            }
            internal float Precision {
                get { return precision; }
                //set { precision = value; }
            }
            internal string Comment {
                get { return comment; }
                //set { comment = value; }
            }
            public override bool Equals(object other) {
                if (other is PreciseEditorData) {
                    PreciseEditorData o = other as PreciseEditorData;
                    bool result = (this.collector == o.collector) && (this.step == o.step) &&
                                  (this.iterations == o.iterations) && (this.width == o.width);
                    return result;
                }
                return false;
            }
            public override int GetHashCode() {
                //later it will be better!
                return base.GetHashCode();
            }
        }
        #endregion
        #region Comparers and predicate for sorting and finding Utility.PreciseEditorData objects in List
        internal static int ComparePreciseEditorData(PreciseEditorData ped1, PreciseEditorData ped2) {
            if (ped1 == null) {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            } else {
                if (ped2 == null)
                    return 1;
                else {
                    if (ped1.Collector != ped2.Collector)
                        return (int)(ped1.Collector - ped2.Collector);
                    if (ped1.Step != ped2.Step)
                        return (int)(ped1.Step - ped2.Step);
                    if (ped1.Width != ped2.Width)
                        return (int)(ped2.Width - ped1.Width);
                    if (ped1.Iterations != ped2.Iterations)
                        return (int)(ped2.Iterations - ped1.Iterations);
                    return 0;
                }
            }
        }
        internal static int ComparePreciseEditorDataByPeakValue(PreciseEditorData ped1, PreciseEditorData ped2) {
            //Forward sort
            if (ped1 == null) {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            } else {
                if (ped2 == null)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static int ComparePreciseEditorDataByUseFlagAndPeakValue(PreciseEditorData ped1, PreciseEditorData ped2) {
            //Forward sort
            if ((ped1 == null) || !ped1.Use) {
                if ((ped2 == null) || !ped2.Use)
                    return 0;
                else
                    return -1;
            } else {
                if ((ped2 == null) || !ped2.Use)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static bool PeakIsUsed(PreciseEditorData ped) {
            return ped.Use;
        }
        #endregion
        #region Textbox charset limitations
        internal static void oneDigitTextbox_TextChanged(object sender, EventArgs e) {
            char[] numbers = { '1', '2' };
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray) {
                foreach (char compareChar in numbers) {
                    if (ch == compareChar) {
                        outputString += ch;
                        ((TextBox)sender).Text = outputString;
                        return;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }
        internal static void integralTextbox_TextChanged(object sender, EventArgs e) {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray) {
                foreach (char compareChar in numbers) {
                    if (ch == compareChar) {
                        outputString += ch;
                        break;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }
        internal static void positiveNumericTextbox_TextChanged(object sender, EventArgs e) {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            bool waitFirstDot = true;
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray) {
                if (waitFirstDot && (ch == '.')) {
                    waitFirstDot = false;
                    outputString += ch;
                    continue;
                }
                foreach (char compareChar in numbers) {
                    if (ch == compareChar) {
                        outputString += ch;
                        break;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }
        #endregion
    }
}
