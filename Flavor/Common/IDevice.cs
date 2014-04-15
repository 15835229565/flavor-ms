using System;

namespace Flavor.Common {
    interface IDevice {
        event EventHandler OnDeviceStateChanged;
        event EventHandler OnDeviceStatusChanged;
        event EventHandler OnVacuumStateChanged;
        event EventHandler OnTurboPumpStatusChanged;
        event TurboPumpAlertEventHandler OnTurboPumpAlert;
        event EventHandler<EventArgs<int[]>> CountsUpdated;
        void RelaysState(byte value);
    }
}
