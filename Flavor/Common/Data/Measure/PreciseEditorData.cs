using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Flavor.Common.Data.Measure {
    class PreciseEditorData: IComparable<PreciseEditorData> {
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
}
