using System;

namespace Flavor.Common {
    interface IDevice {
        event EventHandler<EventArgs<byte>> DeviceStateChanged;
        event EventHandler DeviceStatusChanged;
        event EventHandler VacuumStateChanged;
        event EventHandler TurboPumpStatusChanged;
        event TurboPumpAlertEventHandler TurboPumpAlert;
        event EventHandler<EventArgs<int[]>> CountsUpdated;
        [Obsolete]
        void RelaysState(byte value);
        void OperationReady(bool on);
        void OperationBlock(bool on);
        void UpdateStatus(params ValueType[] data);
        int[] Detectors { get; set; }
    }
}