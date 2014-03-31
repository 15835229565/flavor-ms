using System;

namespace Flavor.Common {
    internal static class ConsoleWriter {
        internal static void Subscribe(ILog o) {
            o.Log += new MessageHandler(Commander2_Log);
        }
        internal static void Desubscribe(ILog o) {
            o.Log -= new MessageHandler(Commander2_Log);
        }
        private static void Commander2_Log(string msg) {
            WriteLine(msg);
        }
        internal static void Write(char c) {
            Console.Write(c);
        }
        internal static void Write(string s) {
            Console.Write(s);
        }
        internal static void WriteLine() {
            Console.WriteLine();
        }
        internal static void WriteLine(object value) {
            Console.WriteLine(value);
        }
        internal static void WriteLine(string format, params object[] args) {
            Console.WriteLine(format, args);
        }
    }
}
