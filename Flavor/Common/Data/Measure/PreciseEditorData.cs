using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Flavor.Common.Data.Measure {
    class PreciseEditorData: IComparable<PreciseEditorData> {
        PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr) {
            pNumber = pn;
            Step = st;
            Collector = co;
            Iterations = it;
            Width = wi;
            Precision = pr;
        }
        public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
            : this(pn, st, co, it, wi, pr) {
            Comment = comm;
        }
        public PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
            : this(pn, st, co, it, wi, pr, comm) {
            Use = useit;
        }
        public PreciseEditorData(PreciseEditorData other)
            : this(other.Use, other.pNumber, other.Step, other.Collector, other.Iterations, other.Width, other.Precision, other.Comment) {
            associatedPoints = other.associatedPoints == null ? null : new PointPairListPlus(other.associatedPoints, this, null);
        }
        public static PreciseEditorData GetCheckerPeak(PreciseEditorData peak, ushort iterations) {
            return new PreciseEditorData(peak) { Iterations = iterations };
        }
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
        public bool Use { get; private set; }
        public byte pNumber { get; private set; }
        public ushort Step { get; private set; }
        public byte Collector { get; private set; }
        public ushort Iterations { get; private set; }
        public ushort Width { get; private set; }
        public float Precision { get; private set; }
        public string Comment { get; private set; }
        public override bool Equals(object other) {
            if (other is PreciseEditorData) {
                var o = (PreciseEditorData)other;
                return (this.pNumber == o.pNumber) && (this.Collector == o.Collector) && (this.Step == o.Step) &&
                       (this.Iterations == o.Iterations) && (this.Width == o.Width) && (this.Comment == o.Comment);
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
                .Append(pNumber)
                .Append(DELIMITER)
                .Append(Use)
                .Append(DELIMITER)
                .Append(Step)
                .Append(DELIMITER)
                .Append(Collector)
                .Append(DELIMITER)
                .Append(Iterations)
                .Append(DELIMITER)
                .Append(Width)
                .Append(DELIMITER)
                .Append(Precision)
                .Append(DELIMITER)
                //? multi-line comments, empty comments ?
                .Append(Comment.Replace("&", "&amp;").Replace(START, START_SUBST).Replace(END, END_SUBST))
                .Append(END).ToString();
        }
        public static List<PreciseEditorData> fromString(string str) {
            //better pattern = @"{(\d+)\s+(True|False)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\S+)\s*?(.*?)}";
            var match = new Regex((new StringBuilder())
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
            var res = new List<PreciseEditorData>();
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
        #region Custom comparison and predicate for sorting and finding PreciseEditorData objects in List
        public readonly static Predicate<PreciseEditorData> PeakIsUsed =
            ped => ped != null && ped.Use;
        public readonly static Comparison<PreciseEditorData> ComparePreciseEditorDataByPeakValue =
            (ped1, ped2) => genericCompare(ped1, ped2, ped => ped == null, () => {
                if (ped1.Step != ped2.Step)
                    return ped1.Step - ped2.Step;
                return ped2.pNumber - ped1.pNumber;
            });
        static int genericCompare(PreciseEditorData ped1, PreciseEditorData ped2, Predicate<PreciseEditorData> predicate, Func<int> comparison) {
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
            if (Collector != other.Collector)
                return Collector - other.Collector;
            if (Step != other.Step)
                return Step - other.Step;
            if (Width != other.Width)
                return other.Width - Width;
            if (Iterations != other.Iterations)
                return other.Iterations - Iterations;
            return other.pNumber - pNumber;
        }
        #endregion
    }
}
