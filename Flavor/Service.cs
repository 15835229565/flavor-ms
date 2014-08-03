using System;
using System.Collections.Generic;
using System.Linq;

namespace Flavor {
    delegate T Processor<T>(T obj);
    class EventArgs<T>: EventArgs {
        public T Value { get; set; }
        public EventArgs(T value) {
            Value = value;
        }
    }
    class CallBackEventArgs<T>: EventArgs<T> {
        public EventHandler Handler { get; set; }
        public CallBackEventArgs(T value, EventHandler handler)
            : base(value) {
            Handler = handler;
        }
    }
    class CallBackEventArgs<T, T1>: EventArgs<T> {
        public EventHandler<EventArgs<T1>> Handler { get; set; }
        public CallBackEventArgs(T value, EventHandler<EventArgs<T1>> handler)
            : base(value)  {
            Handler = handler;
        }
    }
    class FixedSizeQueue<T> {
        readonly Queue<T> queue;
        readonly int maxCapacity;
        //
        // Summary:
        //     Initializes a new instance of the FixedSizeQueue<T> class
        //     that is empty and has the specified maximum capacity.
        //
        // Parameters:
        //   capacity:
        //     The maximum number of elements that the FixedSizeQueue<T>
        //     can contain.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     capacity is less than one.
        public FixedSizeQueue(int capacity) {
            if (capacity < 1)
                throw new System.ArgumentOutOfRangeException("capacity", capacity, "capacity is less than one");
            maxCapacity = capacity;
            queue = new Queue<T>(capacity);
        }

        // Summary:
        //     Gets the number of elements contained in the FixedSizeQueue<T>.
        //
        // Returns:
        //     The number of elements contained in the FixedSizeQueue<T>.
        public int Count { get { return queue.Count; } }

        // Summary:
        //     Removes all objects from the FixedSizeQueue<T>.
        public void Clear() {
            queue.Clear();
        }
        //
        // Summary:
        //     Adds an object to the end of the FixedSizeQueue<T>.
        //
        // Parameters:
        //   item:
        //     The object to add to the FixedSizeQueue<T>. The value can
        //     be null for reference types.
        public void Enqueue(T item) {
            if (queue.Count == maxCapacity) {
                queue.Dequeue();
            }
            queue.Enqueue(item);
        }
        //
        // Summary:
        //     Returns whether the instance of FixedSizeQueue<T> contains maximum number of elements.
        //
        // Returns:
        //     A boolean value indicating whether the instance of FixedSizeQueue<T> contains maximum number of elements.
        public bool IsFull {
            get { return queue.Count == maxCapacity; }
        }
        public T Aggregate(Func<T, T, T> func) {
            return queue.Aggregate(func);
        }
    }
    static class ExtensionMethods {
        public static void Raise(this EventHandler handler, object sender, EventArgs args) {
            // thread-safe as handler is cached as method argument
            if (handler != null)
                handler(sender, args);
        }
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args)
          where T: EventArgs {
            // thread-safe as handler is cached as method argument
            if (handler != null)
                handler(sender, args);
        }
    }
}