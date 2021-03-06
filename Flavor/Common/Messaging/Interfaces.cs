﻿using System;
using System.Collections.Generic;
using Flavor.Common.Data.Controlled;

namespace Flavor.Common.Messaging {
    interface IId<T> where T
        : struct, IConvertible, IComparable {
        T Id { get; }
    }
    interface ISend {
        IList<byte> Data { get; }
    }
    interface IUpdateDevice {
        void UpdateDevice(IDevice device);
        [Obsolete]
        void UpdateDevice();
    }
    interface IUpdateGraph { }
    interface IStatusRequest { }
}