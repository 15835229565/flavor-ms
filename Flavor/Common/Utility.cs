using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Flavor.Common {
    abstract class CommonData {
        public ushort hCurrent { get; set; }
        public double hCurrentReal {
            get { return hCurrentConvert(hCurrent); }
            set { hCurrent = hCurrentConvert(value); }
        }
        public static double hCurrentConvert(ushort current) {
            return (double)current / 4096;
        }
        public static ushort hCurrentConvert(double current) {
            return genericConvert((ushort)(current * 4096));
        }

        public ushort eCurrent { get; set; }
        public double eCurrentReal {
            get { return eCurrentConvert(eCurrent); }
            set { eCurrent = eCurrentConvert(value); }
        }
        public static double eCurrentConvert(ushort current) {
            return 50 * (double)current / 4096;
        }
        public static ushort eCurrentConvert(double current) {
            return genericConvert((ushort)((current / 50) * 4096));
        }

        public ushort iVoltage { get; set; }
        public double iVoltageReal {
            get { return iVoltageConvert(iVoltage); }
            set { iVoltage = iVoltageConvert(value); }
        }
        public static double iVoltageConvert(ushort voltage) {
            return 150 * (double)voltage / 4096;
        }
        public static ushort iVoltageConvert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }

        public ushort fV1 { get; set; }
        public double fV1Real {
            get { return fV1Convert(fV1); }
            set { fV1 = fV1Convert(value); }
        }
        public static double fV1Convert(ushort voltage) {
            return 150 * (double)voltage / 4096;
        }
        public static ushort fV1Convert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }

        public ushort fV2 { get; set; }
        public double fV2Real {
            get { return fV2Convert(fV2); }
            set { fV2 = fV2Convert(value); }
        }
        public static double fV2Convert(ushort voltage) {
            return 150 * (double)voltage / 4096;
        }
        public static ushort fV2Convert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }
        
        protected static ushort genericConvert(ushort x) {
            return x < 4096 ? x : (ushort)4095;
        }
    }
    class CommonOptions: CommonData {
        public ushort befTime { get; set; }
        public ushort befTimeReal {
            get { return (ushort)(befTime * 5); }
            set { befTime = (ushort)(value / 5); }
        }
        public ushort fTime { get; set; }
        public ushort fTimeReal {
            get { return (ushort)(fTime * 5); }
            set { fTime = (ushort)(value / 5); }
        }
        public ushort bTime { get; set; }
        public ushort bTimeReal {
            get { return (ushort)(bTime * 5); }
            set { bTime = (ushort)(value / 5); }
        }
        public bool ForwardTimeEqualsBeforeTime { get; set; }
        public ushort eTime { get; set; }
        public ushort eTimeReal {
            get { return (ushort)(eTime * 5); }
            set { eTime = (ushort)(value / 5); }
        }
        public ushort iTime { get; set; }
        public ushort iTimeReal {
            get { return (ushort)(5 * iTime); }
            set { iTime = (ushort)(value / 5); }
        }
        public ushort CP { get; set; }
        public double CPReal {
            get { return CPConvert(CP); }
            set { CP = CPConvert(value); }
        }
        public static double CPConvert(ushort coeff) {
            return (10 / (double)coeff) * 4096;
        }
        public static ushort CPConvert(double coeff) {
            return genericConvert((ushort)((10 / coeff) * 4096));
        }

        public CommonOptions() {
            // defaults
            iVoltage = 1911;
            hCurrent = 0;
            eCurrent = 79;
            fV1 = 2730;
            fV2 = 2730;
            befTime = 100;
            fTime = 100;
            bTime = 400;
            ForwardTimeEqualsBeforeTime = false;
            eTime = 200;
            iTime = 10;
            CP = 3780;
        }
        // scan voltage modification law
        public static ushort scanVoltage(ushort step) {
            if (step > Config.MAX_STEP) step = Config.MAX_STEP;
            return (ushort)(4095 * Math.Pow(((double)527 / (double)528), 1056 - step));
            //if (step <= 456) return (ushort)(4095 - 5 * step);
            //return (ushort)(4095 - 5 * 456 - 2 * (step - 456));
        }
        public static double scanVoltageReal(ushort step) {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }

        const string DELIMITER = " ";
        const string START = "{";
        const string END = "}";
        public override string ToString() {
            return (new StringBuilder())
                .Append(START)
                .Append(eTime)
                .Append(DELIMITER)
                .Append(iTime)
                .Append(DELIMITER)
                .Append(iVoltage)
                .Append(DELIMITER)
                .Append(CP)
                .Append(DELIMITER)
                .Append(hCurrent)
                .Append(DELIMITER)
                .Append(eCurrent)
                .Append(DELIMITER)
                .Append(fV1)
                .Append(DELIMITER)
                .Append(fV2)
                .Append(END).ToString();
        }
    }

    class Collector: List<Graph.pListScaled> {
        double coeff;
        public double Coeff {
            get { return coeff; }
            set {
                if (coeff == value)
                    return;
                coeff = value;
                foreach (var pl in this)
                    pl.RecomputeMassRow();
            }
        }

        public Collector(double coeff) {
            this.coeff = coeff;
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public double pointToMass(ushort pnt) {
            return coeff / CommonOptions.scanVoltageReal(pnt);
        }
        public new void Clear() {
            base.Clear();
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public void Add(PointPairListPlus ppl) {
            Add(new Graph.pListScaled(this, ppl));
        }
    }
    class Spectrum: List<Collector> {
        public CommonOptions CommonOptions { get; set; }
        public Spectrum(CommonOptions cd, params double[] coeffs)
            : base(coeffs.Length) {
            // better to clone here?
            CommonOptions = cd;
            if (coeffs.Length == 0)
                throw new ArgumentOutOfRangeException("coeffs");
            foreach (double coeff in coeffs)
                Add(new Collector(coeff));
        }
        public int[] RecomputeMassRows(double[] coeffs) {
            if (this.Count != coeffs.Length)
                throw new ArgumentOutOfRangeException("coeffs");
            var result = new List<int>(coeffs.Length);
            for (int i = 0; i < coeffs.Length; ++i) {
                var collector = this[i];
                double coeff = coeffs[i];
                if (coeff == collector.Coeff)
                    continue;
                collector.Coeff = coeff;
                //natural-based index
                result.Add(i + 1);
            }
            return result.ToArray();
        }
        public bool RecomputeMassRows(byte collectorNumber, double coeff) {
            //natural-based index
            if (collectorNumber > this.Count || collectorNumber < 1)
                throw new ArgumentOutOfRangeException("collectorNumber");
            var collector = this[--collectorNumber];
            if (collector.Coeff == coeff)
                return false;
            collector.Coeff = coeff;
            return true;
        }
        public new void Clear() {
            foreach (var collector in this)
                collector.Clear();
        }
    }

    class PreciseSpectrum: List<Utility.PreciseEditorData> {
        CommonOptions myCommonOptions = null;
        public PreciseSpectrum(CommonOptions cd)
            : this() {
            // better to clone here?
            myCommonOptions = cd;
        }
        public PreciseSpectrum()
            : base() {
        }
        public PreciseSpectrum(IEnumerable<Utility.PreciseEditorData> other)
            : base(other) {
            // TODO: check that copies here!
        }
        public CommonOptions CommonOptions {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }

    class PointPairListPlus: ZedGraph.PointPairList {
        Utility.PreciseEditorData myPED;
        Graph.pListScaled myPLS;

        public Utility.PreciseEditorData PEDreference {
            get { return myPED; }
            set { myPED = value; }
        }
        public Graph.pListScaled PLSreference {
            get { return myPLS; }
            set { myPLS = value; }
        }

        public PointPairListPlus()
            : base() {
            myPED = null;
            myPLS = null;
        }
        public PointPairListPlus(Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base() {
            myPED = ped;
            myPLS = pls;
        }
        public PointPairListPlus(PointPairListPlus other, Utility.PreciseEditorData ped, Graph.pListScaled pls)
            : base(other) {
            myPED = ped;
            myPLS = pls;
        }
    }
    #region TreeNodes
    class TreeNodePlus: System.Windows.Forms.TreeNode {
        public enum States {
            Ok,
            Warning,
            Error
        }
        protected States myState = States.Ok;
        public virtual States State {
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
        public TreeNodePlus(string text, TreeNode[] nodes)
            : base(text, nodes) { }
        protected TreeNodePlus()
            : base() { }
        void setStateImageKey() {
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
        void computeState(States previous, States current) {
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
        States computeState(States hint) {
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
    class TreeNodeLeaf: TreeNodePlus {
        public override States State {
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
        new TreeNodeCollection Nodes {
            get { return base.Nodes; }
        }
        void setForeColor() {
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
        public TreeNodeLeaf()
            : base() {
            setForeColor();
        }
    }
    class TreeNodePair: TreeNodePlus {
        new TreeNodeCollection Nodes {
            get { return base.Nodes; }
        }
        public TreeNodePair(string text, TreeNodeLeaf valueNode)
            : base() {
            Text = text;
            Nodes.Add(valueNode);
        }
    }
    #endregion
    static class Utility {
        public static List<PreciseEditorData> getUsed(this List<PreciseEditorData> peds) {
            return peds.FindAll(PreciseEditorData.PeakIsUsed);
        }
        public static List<PreciseEditorData> getUsed(this PreciseSpectrum peds) {
            return peds.FindAll(PreciseEditorData.PeakIsUsed);
        }
        public static List<PreciseEditorData> getWithId(this List<PreciseEditorData> peds) {
            // ! temporary solution
            #warning make this operation one time a cycle
            return peds.FindAll(
                        x => x.Comment.StartsWith(Config.ID_PREFIX_TEMPORARY)
                    );
        }
        #region PreciseEditorData
        public class PreciseEditorData: IComparable<PreciseEditorData> {
            public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr) {
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
            }
            public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                : this(pn, st, co, it, wi, pr) {
                comment = comm;
            }
            public PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
                : this(pn, st, co, it, wi, pr, comm) {
                usethis = useit;
            }
            public PreciseEditorData(PreciseEditorData other)
                : this(other.usethis, other.pointNumber, other.step, other.collector, other.iterations, other.width, other.precision, other.comment) {
                associatedPoints = other.associatedPoints == null ? null : new PointPairListPlus(other.associatedPoints, this, null);
            }
            // use for generate checker peak
            public PreciseEditorData(PreciseEditorData other, ushort iterations)
                : this(other.usethis, other.pointNumber, other.step, other.collector, iterations, other.width, other.precision, other.comment) {
            }
            bool usethis = true;
            byte pointNumber;
            ushort step;
            byte collector;
            ushort iterations;
            ushort width;
            float precision;
            string comment = "";
            PointPairListPlus associatedPoints = null;
            public PointPairListPlus AssociatedPoints {
                get { return associatedPoints; }
                set {
                    if (value == null) {
                        associatedPoints = null;
                        return;
                    }
                    if (value.PEDreference == null) {
                        associatedPoints = value;
                        associatedPoints.PEDreference = this;
                        return;
                    }
                    associatedPoints = new PointPairListPlus(value, this, null);
                }
            }
            public bool Use {
                get { return usethis; }
                //set { usethis = value; }
            }
            public byte pNumber {
                get { return pointNumber; }
                //set { pointNumber = value; }
            }
            public ushort Step {
                get { return step; }
                //set { step = value; }
            }
            public byte Collector {
                get { return collector; }
                //set { collector = value; }
            }
            public ushort Iterations {
                get { return iterations; }
                //set { iterations = value; }
            }
            public ushort Width {
                get { return width; }
                //set { width = value; }
            }
            public float Precision {
                get { return precision; }
                //set { precision = value; }
            }
            public string Comment {
                get { return comment; }
                //set { comment = value; }
            }
            public override bool Equals(object other) {
                if (other is PreciseEditorData) {
                    PreciseEditorData o = other as PreciseEditorData;
                    return (this.pointNumber == o.pointNumber) && (this.collector == o.collector) && (this.step == o.step) &&
                           (this.iterations == o.iterations) && (this.width == o.width) && (this.comment == o.comment);
                }
                return false;
            }
            public override int GetHashCode() {
                //later it will be better!
                return base.GetHashCode();
            }
            const string DELIMITER = " ";
            const string START = "{";
            const string END = "}";
            const string START_SUBST = "&start;";
            const string END_SUBST = "&end;";
            public override string ToString() {
                return (new StringBuilder())
                    .Append(START)
                    .Append(pointNumber)
                    .Append(DELIMITER)
                    .Append(usethis)
                    .Append(DELIMITER)
                    .Append(step)
                    .Append(DELIMITER)
                    .Append(collector)
                    .Append(DELIMITER)
                    .Append(iterations)
                    .Append(DELIMITER)
                    .Append(width)
                    .Append(DELIMITER)
                    .Append(precision)
                    .Append(DELIMITER)
                    //? multi-line comments, empty comments ?
                    .Append(comment.Replace("&", "&amp;").Replace(START, START_SUBST).Replace(END, END_SUBST))
                    .Append(END).ToString();
            }
            public static List<PreciseEditorData> fromString(string str) {
                //better pattern = @"{(\d+)\s+(True|False)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\S+)\s*?(.*?)}";
                Match match = new Regex((new StringBuilder())
                    .Append(START)
                    .Append(@"(\d+)")
                    .Append(DELIMITER)
                    .Append(@"(True|False)")
                    .Append(DELIMITER)
                    .Append(@"(\d+)")
                    .Append(DELIMITER)
                    .Append(@"(\d+)")
                    .Append(DELIMITER)
                    .Append(@"(\d+)")
                    .Append(DELIMITER)
                    .Append(@"(\d+)")
                    .Append(DELIMITER)
                    .Append(@"(\S+)")
                    .Append(DELIMITER)
                    .Append(@"(.*?)")
                    .Append(END)
                    .ToString()).Match(str);
                List<PreciseEditorData> res = new List<PreciseEditorData>();
                GroupCollection groups;
                while (match.Success) {
                    groups = match.Groups;
                    try {
                        res.Add(new PreciseEditorData(
                            bool.Parse(groups[2].Value),
                            byte.Parse(groups[1].Value),
                            ushort.Parse(groups[3].Value),
                            byte.Parse(groups[4].Value),
                            ushort.Parse(groups[5].Value),
                            ushort.Parse(groups[6].Value),
                            float.Parse(groups[7].Value),
                            groups[8].Value.Replace(START_SUBST, START).Replace(END_SUBST, END).Replace("&amp;", "&")));
                    } catch (FormatException) {
                        //continue;
                        return null;
                    }
                    match = match.NextMatch();
                }
                return res;
            }
            #region Custom comparison and predicate for sorting and finding Utility.PreciseEditorData objects in List
            public readonly static Predicate<PreciseEditorData> PeakIsUsed =
                ped => ped != null && ped.usethis;
            public readonly static Comparison<PreciseEditorData> ComparePreciseEditorDataByPeakValue =
                (ped1, ped2) => genericCompare(ped1, ped2, ped => ped == null, () => {
                    if (ped1.step != ped2.step)
                        return ped1.step - ped2.step;
                    return ped2.pointNumber - ped1.pointNumber;
                });
            static int genericCompare(PreciseEditorData ped1, PreciseEditorData ped2, Predicate<PreciseEditorData> predicate, Generator<int> comparison) {
                // stub for any comparison
                if (predicate(ped1)) {
                    if (predicate(ped2))
                        return 0;
                    return -1;
                }
                if (predicate(ped2))
                    return 1;
                return comparison();
            }
            #endregion
            #region IComparable<PreciseEditorData> Members
            // compare by data
			public int CompareTo(PreciseEditorData other) {
                if (other == null)
                    return 1;
                if (collector != other.collector)
                    return collector - other.collector;
                if (step != other.step)
                    return step - other.step;
                if (width != other.width)
                    return other.width - width;
                if (iterations != other.iterations)
                    return other.iterations - iterations;
                return other.pointNumber - pointNumber;
            }
            #endregion
        }
        #endregion
        #region Textbox charset limitations
        public static void oneDigitTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            genericProcessKeyPress(sender, e, ch => (ch == '1' || ch == '2'));
        }
        public static void integralTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            genericProcessKeyPress(sender, e, ch => Char.IsNumber(ch));
        }
        public static void positiveNumericTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            //!!! decimal separator here !!!
            genericProcessKeyPress(sender, e, ch => (Char.IsNumber(ch) || (ch == '.' && !(sender as TextBox).Text.Contains("."))));
        }
        static void genericProcessKeyPress(object sender, KeyPressEventArgs e, Predicate<char> isAllowed){
            if (!(sender is TextBox))
                return;
            char ch = e.KeyChar;
            if (Char.IsControl(ch))
                return;
            if (isAllowed(ch))
                return;
            e.Handled = true;
        }
        #endregion
    }
}
