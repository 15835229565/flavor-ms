using System;

namespace Flavor.Common {
    static class ConsoleWriter {
        static readonly object locker = new object();

        public static void Subscribe(ILog o) {
            o.Log += Log;
        }
        public static void Unsubscribe(ILog o) {
            o.Log -= Log;
        }
        static void Log(string msg) {
            WriteLine(msg);
        }
        static void Write(char c) {
            Console.Write(c);
        }
        // used in Config
        public static void Write(string s) {
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
        public static void WriteLine(string format, params object[] args) {
            lock (locker) {
                Console.WriteLine(format, args);
            }
        }
    }
}
