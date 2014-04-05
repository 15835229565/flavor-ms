using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    interface IId<T> where T
        : struct, IConvertible, IComparable {
        T Id { get; }
    }
    interface ISend {
        IList<byte> Data { get; }
    }
    interface IAutomatedReply {
        ISend AutomatedReply();
    }
    interface IUpdateDevice {
        void UpdateDevice();
    }
    interface IUpdateGraph { }
    interface IStatusRequest { }
}