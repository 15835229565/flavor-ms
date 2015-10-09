using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Controls {
    static class ExtensionMethods {
        public static void AddLabel(this GraphPane pane, double x, int n) {
            var yScale = pane.YAxis.Scale;
            double yMin = yScale.Min;
            double yMax = yScale.Max;

            var line = new LineObj(x, yMin, x, yMax) { IsClippedToChartRect = true };
            //line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            line.Line.Color = Color.DarkGreen;
            pane.GraphObjList.Add(line);

            if (n > 0) {
                var text = new TextObj(n.ToString(), x, yMax - 0.02 * (yMax - yMin)) { IsClippedToChartRect = true };
                text.FontSpec.Border.IsVisible = true;
                pane.GraphObjList.Add(text);
            }
        }
        #region Textbox charset limitations
        public static KeyPressEventHandler OneDigitTextChanged(int count) {
            // <= 9 collectors!
            char max = '0';
            max += (char)count;
            return (s, e) => processKeyPress(e, c => c >= '1' && c <= max);
        }
        //public static void OneDigitTextChanged(object sender, KeyPressEventArgs e) {
        //    // <= 9 collectors!
        //    char max = '0';
        //    max += (char)Config.COLLECTOR_COEFFS.Length;
        //    processKeyPress(e, c => c >= '1' && c <= max);
        //}
        public static void IntegralTextChanged(object sender, KeyPressEventArgs e) {
            processKeyPress(e, c => Char.IsNumber(c));
        }
        public static KeyPressEventHandler PositiveNumericTextChanged(this TextBox textBox) {
            //!!! decimal separator here !!!
            return (s, e) => processKeyPress(e, c => Char.IsNumber(c) || (c == '.' && textBox.Text.Contains(".")));
        }
        static void processKeyPress(KeyPressEventArgs e, Predicate<char> isAllowed) {
            char c = e.KeyChar;
            if (Char.IsControl(c) || isAllowed(c))
                return;
            e.Handled = true;
        }
        #endregion
    }
}
