using System;

namespace Flavor.Common {
    internal static class ConsoleWriter {
        static readonly object locker = new object();

        internal static void Subscribe(ILog o) {
            o.Log += Log;
        }
        internal static void Unsubscribe(ILog o) {
            o.Log -= Log;
        }
        static void Log(string msg) {
            WriteLine(msg);
        }
        static void Write(char c) {
            Console.Write(c);
        }
        // used in Config
        internal static void Write(string s) {
            lock (locker) {
                Console.Write(s);
            }
        }
        static void WriteLine() {
            Console.WriteLine();
        }
        static void WriteLine(object value) {
            Console.WriteLine(value);
        }
        // used in UI
        internal static void WriteLine(string format, params object[] args) {
            lock (locker) {
                Console.WriteLine(format, args);
            }
        }
    }
}
