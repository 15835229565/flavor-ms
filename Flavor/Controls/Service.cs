using System;
using System.Windows.Forms;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Controls {
    static class Utility {
        #region Textbox charset limitations
        public static void oneDigitTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            // <= 9 collectors!
            char max = '0';
            max += (char)Config.COLLECTOR_COEFFS.Length;
            genericProcessKeyPress(sender, e, c => (c >= '1' && c <= max));
        }
        public static void integralTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            genericProcessKeyPress(sender, e, c => Char.IsNumber(c));
        }
        public static void positiveNumericTextbox_TextChanged(object sender, KeyPressEventArgs e) {
            //!!! decimal separator here !!!
            genericProcessKeyPress(sender, e, c => (Char.IsNumber(c) || (c == '.' && sender is TextBox && !((TextBox)sender).Text.Contains("."))));
        }
        static void genericProcessKeyPress(object sender, KeyPressEventArgs e, Predicate<char> isAllowed) {
            char c = e.KeyChar;
            if (Char.IsControl(c))
                return;
            if (isAllowed(c))
                return;
            e.Handled = true;
        }
        #endregion
    }
}
