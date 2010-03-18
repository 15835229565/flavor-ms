using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Flavor.Common
{
    internal class CommonOptions
    {
        // defaults
        private ushort beforeTime = 100;
        private ushort forwardTime = 100;
        private ushort backwardTime = 400;
        private bool forwardAsBefore = false;

        internal ushort befTime
        {
            get { return beforeTime; }
            set { beforeTime = value; }
        }
        internal ushort befTimeReal
        {
            get { return (ushort)(beforeTime * 5); }
            set
            {
                beforeTime = (ushort)(value / 5);
            }
        }
        internal ushort fTime
        {
            get { return forwardTime; }
            set { forwardTime = value; }
        }
        internal ushort fTimeReal
        {
            get { return (ushort)(forwardTime * 5); }
            set
            {
                forwardTime = (ushort)(value / 5);
            }
        }
        internal ushort bTime
        {
            get { return backwardTime; }
            set { backwardTime = value; }
        }
        internal ushort bTimeReal
        {
            get { return (ushort)(backwardTime * 5); }
            set
            {
                backwardTime = (ushort)(value / 5);
            }
        }
        internal bool ForwardTimeEqualsBeforeTime
        {
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

        internal ushort eTime
        {
            get { return expTime; }
            set { expTime = value; }
        }
        internal ushort eTimeReal
        {
            get { return (ushort)(expTime * 5); }
            set
            {
                expTime = (ushort)(value / 5);
            }
        }

        internal ushort iTime
        {
            get { return idleTime; }
            set { idleTime = value; }
        }
        internal ushort iTimeReal
        {
            get { return (ushort)(5 * idleTime); }
            set
            {
                idleTime = (ushort)(value / 5);
            }
        }

        internal ushort iVoltage
        {
            get { return ionizationVoltage; }
            set { ionizationVoltage = value; }
        }
        internal double iVoltageReal
        {
            get { return iVoltageConvert(ionizationVoltage); }
            set
            {
                ionizationVoltage = iVoltageConvert(value);
            }
        }
        internal double iVoltageConvert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort iVoltageConvert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }
        
        internal ushort CP
        {
            get { return CPVoltage; }
            set { CPVoltage = value; }
        }
        internal double CPReal
        {
            get { return CPConvert(CPVoltage); }
            set
            {
                CPVoltage = CPConvert(value);
            }
        }
        internal double CPConvert(ushort coeff)
        {
            return (double)((10 / (double)coeff) * 4096);
        }
        internal ushort CPConvert(double coeff)
        {
            ushort x = (ushort)((10 / coeff) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort eCurrent
        {
            get { return emissionCurrent; }
            set { emissionCurrent = value; }
        }
        internal double eCurrentReal
        {
            get { return eCurrentConvert(emissionCurrent); }
            set
            {
                emissionCurrent = eCurrentConvert(value);
            }
        }
        internal double eCurrentConvert(ushort current)
        {
            return (double)((50 * (double)current) / 4096);
        }
        internal ushort eCurrentConvert(double current)
        {
            ushort x = (ushort)((current / 50) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort hCurrent
        {
            get { return heatCurrent; }
            set { heatCurrent = value; }
        }
        internal double hCurrentReal
        {
            get { return hCurrentConvert(heatCurrent); }
            set
            {
                heatCurrent = hCurrentConvert(value);
            }
        }
        internal double hCurrentConvert(ushort current)
        {
            return (double)((double)current / 4096);
        }
        internal ushort hCurrentConvert(double current)
        {
            ushort x = (ushort)(current * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort fV1
        {
            get { return focusVoltage1; }
            set { focusVoltage1 = value; }
        }
        internal double fV1Real
        {
            get { return fV1Convert(focusVoltage1); }
            set
            {
                focusVoltage1 = fV1Convert(value);
            }
        }
        internal double fV1Convert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort fV1Convert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort fV2
        {
            get { return focusVoltage2; }
            set { focusVoltage2 = value; }
        }
        internal double fV2Real
        {
            get { return fV2Convert(focusVoltage2); }
            set
            {
                focusVoltage2 = fV2Convert(value);
            }
        }
        internal double fV2Convert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        internal ushort fV2Convert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        internal ushort scanVoltage(ushort step)
        {
            if (step > Config.MAX_STEP) step = Config.MAX_STEP;
            return Convert.ToUInt16(4095 * Math.Pow(((double)527 / (double)528), 1056 - step));
            //if (step <= 456) return (ushort)(4095 - 5 * step);
            //return (ushort)(4095 - 5 * 456 - 2 * (step - 456));
        }
        internal double scanVoltageReal(ushort step)
        {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }
    }

    internal class Spectrum: List<List<Graph.pListScaled>>
    {
        private CommonOptions myCommonOptions = null;
        internal Spectrum(CommonOptions cd)
            : this()
        {
            // better to clone here..
            myCommonOptions = cd;
        }
        internal Spectrum()
            : base()
        {
            List<Graph.pListScaled> temp1 = new List<Graph.pListScaled>();
            List<Graph.pListScaled> temp2 = new List<Graph.pListScaled>();
            this.Add(temp1);
            this.Add(temp2);
        }
        internal CommonOptions CommonOptions
        {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }
    
    internal class PreciseSpectrum : List<Utility.PreciseEditorData>
    {
        private CommonOptions myCommonOptions = null;
        internal PreciseSpectrum(CommonOptions cd)
            : this()
        {
            // better to clone here..
            myCommonOptions = cd;
        }
        internal PreciseSpectrum()
            : base()
        {
        }
        internal CommonOptions CommonOptions
        {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }

    internal class PointPairListPlus : ZedGraph.PointPairList
    {
        private Utility.PreciseEditorData myPED;
        private Graph.pListScaled myPLS;

        internal Utility.PreciseEditorData PEDreference
        {
            get { return myPED; }
            set { myPED = value; }
        }
        internal Graph.pListScaled PLSreference
        {
            get { return myPLS; }
            set { myPLS = value; }
        }

        internal PointPairListPlus()
            : base()
        {
            myPED = null;
            myPLS = null;
        }
        internal PointPairListPlus(Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base()
        {
            myPED = ped;
            myPLS = pls;
        }
        internal PointPairListPlus(PointPairListPlus other, Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base(other)
        {
            myPED = ped;
            myPLS = pls;
        }
    }
    #region TreeNodes
    internal class TreeNodePlus : System.Windows.Forms.TreeNode
    {
        internal enum States
        {
            Ok,
            Warning,
            Error
        }
        protected States myState = States.Ok;
        internal virtual States State
        {
            get { return myState; }
            set 
            {
                if (myState != value)
                {
                    States previous = myState;
                    myState = value;
                    if (Parent is TreeNodePlus)
                    {
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
        private void setStateImageKey()
        {
            switch (myState)
            {
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
        private void computeState(States previous, States current)
        {
            if (myState < previous)
            {
                // illegal state
                throw new InvalidOperationException();
            }
            if (myState < current)
            {
                State = current;
                return;
            }
            if (myState > current)
            {
                if (previous < current)
                {
                    return;
                }
                State = computeState(current);
            }
        }
        private States computeState(States hint)
        {
            States result = hint;
            foreach (TreeNodePlus node in Nodes)
            {
                if (result < node.State)
                {
                    result = node.State;
                    if (result == States.Error)
                        return result;
                }
            }
            return result;
        }
    }
    internal class TreeNodeLeaf : TreeNodePlus
    {
        internal override States State
        {
            get { return myState; }
            set
            {
                if (myState != value)
                {
                    myState = value;
                    if (Parent is TreeNodePair)
                    {
                        (Parent as TreeNodePair).State = value;
                    }
                    setForeColor();
                }
            }
        }
        private new TreeNodeCollection Nodes
        {
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

        private void setForeColor()
        {
            switch (State)
            {
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
            : base()
        {
            setForeColor();
        }
    }
    internal class TreeNodePair : TreeNodePlus
    {
        private new TreeNodeCollection Nodes
        {
            get { return base.Nodes; }
        }
        internal TreeNodePair(string text, TreeNodeLeaf valueNode): base()
        {
            Text = text;
            Nodes.Add(valueNode);
        }
    }
    #endregion
    internal static class Utility
    {
        #region PreciseEditorData
        internal class PreciseEditorData
        {
            internal PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr)
            {
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
            }
            internal PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                : this(pn, st, co, it, wi, pr)
            {
                comment = comm;
            }
            internal PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                :this(pn, st, co, it, wi, pr, comm)
            {
                usethis = useit;
            }
            internal PreciseEditorData(PreciseEditorData other)
                : this(other.usethis, other.pointNumber, other.step, other.collector, other.iterations, other.width, other.precision, other.comment)
            {
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
            internal PointPairListPlus AssociatedPoints
            {
                get { return associatedPoints; }
                set
                {
                    if (value.PEDreference == null)
                    {
                        associatedPoints = value;
                        associatedPoints.PEDreference = this;
                    }
                    else
                        associatedPoints = new PointPairListPlus(value, this, null); 
                }
            }
            internal bool Use
            {
                get { return usethis; }
                //set { usethis = value; }
            }
            internal byte pNumber
            {
                get { return pointNumber; }
                //set { pointNumber = value; }
            }
            internal ushort Step
            {
                get { return step; }
                //set { step = value; }
            }
            internal byte Collector
            {
                get { return collector; }
                //set { collector = value; }
            }
            internal ushort Iterations
            {
                get { return iterations; }
                //set { iterations = value; }
            }
            internal ushort Width
            {
                get { return width; }
                //set { width = value; }
            }
            internal float Precision
            {
                get { return precision; }
                //set { precision = value; }
            }
            internal string Comment
            {
                get { return comment; }
                //set { comment = value; }
            }
            public override bool Equals(object other)
            {
                if (other is PreciseEditorData)
                {
                    PreciseEditorData o = other as PreciseEditorData;
                    bool result = (this.collector == o.collector) && (this.step == o.step) &&
                                  (this.iterations == o.iterations) && (this.width == o.width);
                    return result;
                }
                return false;
            }
            public override int GetHashCode()
            {
                //later it will be better!
                return base.GetHashCode();
            }
        }
        #endregion
        #region PreciseEditorRows
        internal class PreciseEditorLabelRow
        {
            protected System.Windows.Forms.Label label8;
            protected System.Windows.Forms.Label colNumLabel;
            protected System.Windows.Forms.Label label9;
            protected System.Windows.Forms.Label label10;
            protected System.Windows.Forms.Label label11;
            protected System.Windows.Forms.Label commentLabel;
            internal PreciseEditorLabelRow()
            {
                this.label8 = new System.Windows.Forms.Label();
                this.colNumLabel = new System.Windows.Forms.Label();
                this.label9 = new System.Windows.Forms.Label();
                this.label10 = new System.Windows.Forms.Label();
                this.label11 = new System.Windows.Forms.Label();
                this.commentLabel = new System.Windows.Forms.Label();
                // label8
                this.label8.AutoSize = true;
                this.label8.BackColor = System.Drawing.SystemColors.Control;
                this.label8.Location = new System.Drawing.Point(0, 0);
                this.label8.Name = "label8";
                this.label8.Size = new System.Drawing.Size(60, 26);
                this.label8.Text = "���������\r\n(<=1056)";
                this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                // colNumLabel
                this.colNumLabel.AutoSize = true;
                this.colNumLabel.Location = new System.Drawing.Point(50, 13);
                this.colNumLabel.Name = "colNumLabel";
                this.colNumLabel.Size = new System.Drawing.Size(29, 13);
                this.colNumLabel.Text = "���.";
                // label10
                this.label10.AutoSize = true;
                this.label10.BackColor = System.Drawing.SystemColors.Control;
                this.label10.Location = new System.Drawing.Point(75, 13);
                this.label10.Name = "label10";
                this.label10.Size = new System.Drawing.Size(46, 13);
                this.label10.Text = "������";
                // label9
                this.label9.AutoSize = true;
                this.label9.BackColor = System.Drawing.SystemColors.Control;
                this.label9.Location = new System.Drawing.Point(126, 13);
                this.label9.Name = "label9";
                this.label9.Size = new System.Drawing.Size(52, 13);
                this.label9.Text = "�������";
                // label11
                this.label11.AutoSize = true;
                this.label11.BackColor = System.Drawing.SystemColors.Control;
                this.label11.Location = new System.Drawing.Point(177, 13);
                this.label11.Name = "label11";
                this.label11.Size = new System.Drawing.Size(54, 13);
                this.label11.Text = "��������";
                // commentLabel
                this.commentLabel.AutoSize = true;
                this.commentLabel.BackColor = System.Drawing.SystemColors.Control;
                this.commentLabel.Location = new System.Drawing.Point(233, 13);
                this.commentLabel.Name = "commentLabel";
                this.commentLabel.Size = new System.Drawing.Size(54, 13);
                this.commentLabel.Text = "�����������";
            }
            internal PreciseEditorLabelRow(int x, int y): this()
            {
                moveTo(x, y);
            }
            internal virtual Control[] getControls()
            {
                return new Control[] { colNumLabel, label11, label10, label9, label8, commentLabel };
            }
            protected virtual void moveTo(int x, int y)
            {
                this.label8.Location = new System.Drawing.Point(this.label8.Location.X + x, this.label8.Location.Y + y);
                this.colNumLabel.Location = new System.Drawing.Point(this.colNumLabel.Location.X + x, this.colNumLabel.Location.Y + y);
                this.label9.Location = new System.Drawing.Point(this.label9.Location.X + x, this.label9.Location.Y + y);
                this.label10.Location = new System.Drawing.Point(this.label10.Location.X + x, this.label10.Location.Y + y);
                this.label11.Location = new System.Drawing.Point(this.label11.Location.X + x, this.label11.Location.Y + y);
                this.commentLabel.Location = new System.Drawing.Point(this.commentLabel.Location.X + x, this.commentLabel.Location.Y + y);
            }
        }
        internal class PreciseEditorLabelRowPlus: PreciseEditorLabelRow
        {
            private System.Windows.Forms.Label label1;
            internal PreciseEditorLabelRowPlus(): base()
            {
                base.moveTo(49, 0);
                this.label1 = new System.Windows.Forms.Label();
                // label1
                this.label1.AutoSize = true;
                this.label1.BackColor = System.Drawing.SystemColors.Control;
                this.label1.Location = new System.Drawing.Point(0, 0);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(41, 26);
                this.label1.Text = "�����\r\n����";
                this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            }
            internal PreciseEditorLabelRowPlus(int x, int y): this()
            {
                moveTo(x, y);
            }
            internal override Control[] getControls()
            {
                return new Control[] { colNumLabel, label11, label10, label9, label8, commentLabel, label1 };
            }
            protected override void moveTo(int x, int y)
            {
                this.label1.Location = new System.Drawing.Point(this.label1.Location.X + x, this.label1.Location.Y + y);
                this.label8.Location = new System.Drawing.Point(this.label8.Location.X + x, this.label8.Location.Y + y);
                this.colNumLabel.Location = new System.Drawing.Point(this.colNumLabel.Location.X + x, this.colNumLabel.Location.Y + y);
                this.label9.Location = new System.Drawing.Point(this.label9.Location.X + x, this.label9.Location.Y + y);
                this.label10.Location = new System.Drawing.Point(this.label10.Location.X + x, this.label10.Location.Y + y);
                this.label11.Location = new System.Drawing.Point(this.label11.Location.X + x, this.label11.Location.Y + y);
                this.commentLabel.Location = new System.Drawing.Point(this.commentLabel.Location.X + x, this.commentLabel.Location.Y + y);
            }
        }
        internal class PreciseEditorRowMinus
        {
            protected TextBox stepTextBox;
            internal string StepText
            {
                get { return stepTextBox.Text; }
                set { stepTextBox.Text = value; }
            }
            protected TextBox colTextBox;
            internal string ColText
            {
                get { return colTextBox.Text; }
                set { colTextBox.Text = value; }
            }
            protected TextBox widthTextBox;
            internal string WidthText
            {
                get { return widthTextBox.Text; }
                //set { widthTextBox.Text = value; }
            }
            protected bool stepAndColModifiable = false;
            protected bool allFilled = false;
            internal bool AllFilled
            {
                get { return allFilled; }
            }
            internal PreciseEditorRowMinus()
            {
                this.stepTextBox = new TextBox();
                this.colTextBox = new TextBox();
                this.widthTextBox = new TextBox();

                if (stepAndColModifiable)
                    this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                else
                    this.stepTextBox.BackColor = System.Drawing.SystemColors.Control;
                this.stepTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.stepTextBox.Location = new System.Drawing.Point(0, 0);
                this.stepTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.stepTextBox.MaxLength = 4;
                this.stepTextBox.Size = new System.Drawing.Size(50, 13);
                this.stepTextBox.ReadOnly = !stepAndColModifiable;

                if (stepAndColModifiable)
                    this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                else
                    this.colTextBox.BackColor = System.Drawing.SystemColors.Control;
                this.colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.colTextBox.Location = new System.Drawing.Point(52, 0);
                this.colTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.colTextBox.MaxLength = 1;
                this.colTextBox.Size = new System.Drawing.Size(20, 13);
                this.colTextBox.ReadOnly = !stepAndColModifiable;

                this.widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.widthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.widthTextBox.Location = new System.Drawing.Point(74, 0);
                this.widthTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.widthTextBox.MaxLength = 4;
                this.widthTextBox.Size = new System.Drawing.Size(50, 13);
                this.widthTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);
            }
            internal PreciseEditorRowMinus(int x, int y): this()
            {
                moveTo(x, y);
            }
            protected virtual void moveTo(int x, int y)
            {
                this.stepTextBox.Location = new System.Drawing.Point(this.stepTextBox.Location.X + x, this.stepTextBox.Location.Y + y);
                this.colTextBox.Location = new System.Drawing.Point(this.colTextBox.Location.X + x, this.colTextBox.Location.Y + y);
                this.widthTextBox.Location = new System.Drawing.Point(this.widthTextBox.Location.X + x, this.widthTextBox.Location.Y + y);
            }
            internal virtual Control[] getControls()
            {
                return new Control[] { stepTextBox, colTextBox, widthTextBox };
            }
            protected virtual void Clear()
            {
                stepTextBox.Text = "";
                colTextBox.Text = "";
                widthTextBox.Text = "";
                stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            }
            internal virtual bool checkTextBoxes()
            {
                bool exitFlag = true;
                bool somethingFilled = (stepTextBox.Text != "") || (colTextBox.Text != "") || (widthTextBox.Text != "");
                this.allFilled = (stepTextBox.Text != "") && (colTextBox.Text != "") && (widthTextBox.Text != "");
                stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                if (somethingFilled && !this.allFilled)
                {
                    stepTextBox.BackColor = Color.Gold;
                    colTextBox.BackColor = Color.Gold;
                    widthTextBox.BackColor = Color.Gold;
                    exitFlag = false;
                }
                if ((widthTextBox.Text != "") && (stepTextBox.Text != "") &&
                    ((Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < Config.MIN_STEP) ||
                    (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP)))
                {
                    stepTextBox.BackColor = Color.Green;
                    widthTextBox.BackColor = Color.Green;
                    exitFlag = false;
                }
                if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP))
                {
                    stepTextBox.BackColor = Color.Red;
                    exitFlag = false;
                }
                return exitFlag;
            }
            internal virtual void setValues(PreciseEditorData p)
            {
                stepTextBox.Text = p.Step.ToString();
                colTextBox.Text = p.Collector.ToString();
                widthTextBox.Text = p.Width.ToString();
            }
        }
        internal class PreciseEditorRow: PreciseEditorRowMinus
        {
            protected TextBox lapsTextBox;
            internal string LapsText
            {
                get { return lapsTextBox.Text; }
                //set { lapsTextBox.Text = value; }
            }
            protected TextBox precTextBox;
            internal string PrecText
            {
                get { return precTextBox.Text; }
                //set { precTextBox.Text = value; }
            }
            protected TextBox commentTextBox;
            internal string CommentText
            {
                get { return commentTextBox.Text; }
                //set { commentTextBox.Text = value; }
            }
            //The other result of checkTextBoxes()
            internal PreciseEditorRow(): base()
            {
                this.lapsTextBox = new TextBox();
                this.precTextBox = new TextBox();
                this.commentTextBox = new TextBox();

                this.lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.lapsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.lapsTextBox.Location = new System.Drawing.Point(126, 0);
                this.lapsTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.lapsTextBox.Size = new System.Drawing.Size(50, 13);
                this.lapsTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);

                this.precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.precTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.precTextBox.Location = new System.Drawing.Point(178, 0);
                this.precTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.precTextBox.Size = new System.Drawing.Size(50, 13);
                this.precTextBox.TextChanged += new System.EventHandler(Utility.positiveNumericTextbox_TextChanged);

                this.commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.commentTextBox.Location = new System.Drawing.Point(230, 0);
                this.commentTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.commentTextBox.Size = new System.Drawing.Size(100, 13);
            }
            internal PreciseEditorRow(int x, int y): this()
            {
                moveTo(x, y);
            }
            protected override void moveTo(int x, int y)
            {
                base.moveTo(x, y);
                this.lapsTextBox.Location = new System.Drawing.Point(this.lapsTextBox.Location.X + x, this.lapsTextBox.Location.Y + y);
                this.precTextBox.Location = new System.Drawing.Point(this.precTextBox.Location.X + x, this.precTextBox.Location.Y + y);
                this.commentTextBox.Location = new System.Drawing.Point(this.commentTextBox.Location.X + x, this.commentTextBox.Location.Y + y);
            }
            internal override Control[] getControls()
            {
                return new Control[] { stepTextBox, colTextBox, lapsTextBox, widthTextBox, precTextBox, commentTextBox };
            }
            protected override void Clear()
            {
                base.Clear();
                lapsTextBox.Text = "";
                precTextBox.Text = "";
                commentTextBox.Text = "";
                lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            }
            internal override bool checkTextBoxes()
            {
                bool exitFlag = true;
                bool somethingFilled = ((lapsTextBox.Text != "") || (stepTextBox.Text != "") ||
                                        (colTextBox.Text != "") || (widthTextBox.Text != "") /*|| (precTextBox].Text != "")*/);
                this.allFilled = ((lapsTextBox.Text != "") && (stepTextBox.Text != "") && (colTextBox.Text != "") &&
                                  (widthTextBox.Text != "")/* && (precTextBox.Text != "")*/);
                stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                if (somethingFilled && !this.allFilled)
                {
                    stepTextBox.BackColor = Color.Gold;
                    colTextBox.BackColor = Color.Gold;
                    lapsTextBox.BackColor = Color.Gold;
                    widthTextBox.BackColor = Color.Gold;
                    precTextBox.BackColor = Color.Gold;
                    commentTextBox.BackColor = Color.Gold;
                    exitFlag = false;
                }
                if ((widthTextBox.Text != "") && (stepTextBox.Text != "") &&
                    ((Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < Config.MIN_STEP) ||
                    (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP)))
                {
                    stepTextBox.BackColor = Color.Green;
                    widthTextBox.BackColor = Color.Green;
                    exitFlag = false;
                }
                if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP))
                {
                    stepTextBox.BackColor = Color.Red;
                    exitFlag = false;
                }
                return exitFlag;
            }
            internal override void setValues(PreciseEditorData p)
            {
                base.setValues(p);
                lapsTextBox.Text = p.Iterations.ToString();
                precTextBox.Text = p.Precision.ToString();
                commentTextBox.Text = p.Comment;
            }
        }
        internal class PreciseEditorRowPlus: PreciseEditorRow
        {
            private Label peakNumberLabel;
            internal string PeakNumber
            {
                set { peakNumberLabel.Text = value; }
            }
            private CheckBox usePeakCheckBox;
            internal bool UseChecked
            {
                get { return usePeakCheckBox.Checked; }
                //set { usePeakCheckBox.Checked = value; }
            }
            private Button clearPeakButton;
            private static ToolTip clearRowToolTip = new ToolTip();
            internal PreciseEditorRowPlus(): base()
            {
                base.moveTo(37, 0);
                
                this.peakNumberLabel = new Label();
                this.usePeakCheckBox = new CheckBox();
                this.clearPeakButton = new Button();

                this.peakNumberLabel.AutoSize = true;
                this.peakNumberLabel.BackColor = System.Drawing.SystemColors.Control;
                this.peakNumberLabel.Location = new System.Drawing.Point(0, 0);
                this.peakNumberLabel.Size = new System.Drawing.Size(16, 13);
                this.peakNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                this.usePeakCheckBox.Location = new System.Drawing.Point(22, 0);
                this.usePeakCheckBox.Size = new System.Drawing.Size(13, 13);
                this.usePeakCheckBox.Text = "";

                this.clearPeakButton.Location = new System.Drawing.Point(369, 0);
                this.clearPeakButton.Size = new System.Drawing.Size(13, 13);
                this.clearPeakButton.Margin = new Padding(1);
                this.clearPeakButton.MouseHover += new EventHandler(clearPeakButton_MouseHover);
                this.clearPeakButton.Click += new EventHandler(clearPeakButton_Click);

                this.stepAndColModifiable = true;
                this.stepTextBox.ReadOnly = !stepAndColModifiable;
                this.colTextBox.ReadOnly = !stepAndColModifiable;
                if (stepAndColModifiable)
                    this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                else
                    this.stepTextBox.BackColor = System.Drawing.SystemColors.Control;
                if (stepAndColModifiable)
                    this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                else
                    this.colTextBox.BackColor = System.Drawing.SystemColors.Control;
                
                this.stepTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);
                this.colTextBox.TextChanged += new System.EventHandler(Utility.oneDigitTextbox_TextChanged);
            }
            internal PreciseEditorRowPlus(int x, int y): this()
            {
                moveTo(x, y);
            }
            protected override void moveTo(int x, int y)
            {
                base.moveTo(x, y);
                
                this.peakNumberLabel.Location = new System.Drawing.Point(this.peakNumberLabel.Location.X + x, this.peakNumberLabel.Location.Y + y);
                this.usePeakCheckBox.Location = new System.Drawing.Point(this.usePeakCheckBox.Location.X + x, this.usePeakCheckBox.Location.Y + y);
                this.clearPeakButton.Location = new System.Drawing.Point(this.clearPeakButton.Location.X + x, this.clearPeakButton.Location.Y + y);
            }
            internal override Control[] getControls()
            {
                return new Control[] { peakNumberLabel, usePeakCheckBox, stepTextBox, colTextBox, lapsTextBox, widthTextBox, precTextBox, commentTextBox, clearPeakButton };
            }
            internal new void Clear()
            {
                base.Clear();
                usePeakCheckBox.Checked = false;
            }
            private void clearPeakButton_MouseHover(object sender, EventArgs e)
            {
                PreciseEditorRowPlus.clearRowToolTip.Show("�������� ������", (IWin32Window)sender);
            }
            private void clearPeakButton_Click(object sender, EventArgs e)
            {
                this.Clear();
            }
            internal override bool checkTextBoxes() 
            {
                return base.checkTextBoxes();
            }
            internal override void setValues(PreciseEditorData ped)
            {
                base.setValues(ped);
                usePeakCheckBox.Checked = ped.Use;
            }
        }
        #endregion
        #region Comparers and predicate for sorting and finding Utility.PreciseEditorData objects in List
        internal static int ComparePreciseEditorData(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            if (ped1 == null)
            {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (ped2 == null)
                    return 1;
                else
                {
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
        internal static int ComparePreciseEditorDataByPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if (ped1 == null)
            {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (ped2 == null)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static int ComparePreciseEditorDataByUseFlagAndPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if ((ped1 == null) || !ped1.Use)
            {
                if ((ped2 == null) || !ped2.Use)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if ((ped2 == null) || !ped2.Use)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static bool PeakIsUsed(PreciseEditorData ped)
        {
            return ped.Use;
        }
        #endregion
        #region Textbox charset limitations
        internal static void oneDigitTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '1', '2' };
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray)
            {
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
                        outputString += ch;
                        ((TextBox)sender).Text = outputString;
                        return;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }
        internal static void integralTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray)
            {
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
                        outputString += ch;
                        break;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }
        internal static void positiveNumericTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            bool waitFirstDot = true;
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray)
            {
                if (waitFirstDot && (ch == '.'))
                {
                    waitFirstDot = false;
                    outputString += ch;
                    continue;
                }
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
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
