using System;

namespace Flavor.Common {
    interface IDevice {
        event EventHandler<EventArgs<byte>> DeviceStateChanged;
        event EventHandler<EventArgs<ValueType[]>> DeviceStatusChanged;
        event EventHandler<EventArgs<ValueType[]>> VacuumStateChanged;
        event EventHandler TurboPumpStatusChanged;
        event TurboPumpAlertEventHandler TurboPumpAlert;
        event EventHandler<EventArgs<uint[]>> CountsUpdated;
        [Obsolete]
        void RelaysState(byte value);
        void OperationReady(bool on);
        void OperationBlock(bool on);
        void UpdateStatus(params ValueType[] data);
        void UpdateVacuumStatus(params ValueType[] data);
        uint[] Detectors { get; set; }
    }
}