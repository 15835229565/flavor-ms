using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    static class Utility
    {
        public class PreciseEditorData
        {
            public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr)
            {
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
            }
            public PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
            {
                usethis = useit;
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
                comment = comm;
            }
            private bool usethis = true;
            private byte pointNumber;
            private ushort step;
            private byte collector;
            private ushort iterations;
            private ushort width;
            private float precision;
            private string comment = "";
            ZedGraph.PointPairList associatedPoints;
            public ZedGraph.PointPairList AssociatedPoints
            {
                get { return associatedPoints; }
                set { associatedPoints = value; }
            }
            public bool Use
            {
                get { return usethis; }
                //set { usethis = value; }
            }
            public byte pNumber
            {
                get { return pointNumber; }
                //set { pointNumber = value; }
            }
            public ushort Step
            {
                get { return step; }
                //set { step = value; }
            }
            public byte Collector
            {
                get { return collector; }
                //set { collector = value; }
            }
            public ushort Iterations
            {
                get { return iterations; }
                //set { iterations = value; }
            }
            public ushort Width
            {
                get { return width; }
                //set { width = value; }
            }
            public float Precision
            {
                get { return precision; }
                //set { precision = value; }
            }
            public string Comment
            {
                get { return comment; }
                //set { comment = value; }
            }
        }
        //Comparers and predicate for sorting and finding Utility.PreciseEditorData objects in List
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
        //Textbox charset limitations
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
    }
}
