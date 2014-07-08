using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common {
    class AlmazovDevice: IDevice {
        [Flags]
        public enum DeviceStates: byte {
            None = 0,
            Init = 1,
            SEMV1 = 2,
            Turbo = 4,
            Relay2 = 8,
            HVE = 16,
            PRGE = 32,
            Relay3 = 64,
            Alert = 128
        }
        DeviceStates state = DeviceStates.None;
        DeviceStates State {
            get { return state; }
            set {
                if (value != state) {
                    state = value;
                    OnDeviceStateChanged((byte)state);
                }
            }
        }

        #region IDevice Members
        public event EventHandler<EventArgs<byte>> DeviceStateChanged;
        protected void OnDeviceStateChanged(byte state) {
            DeviceStateChanged.Raise(this, new EventArgs<byte>(state));
        }
        // TODO: use!
        public event EventHandler DeviceStatusChanged;
        protected void OnDeviceStatusChanged() {
            DeviceStatusChanged.Raise(this, EventArgs.Empty);
        }
        // TODO: use!
        public event EventHandler VacuumStateChanged;
        protected void OnVacuumStateChanged() {
            VacuumStateChanged.Raise(this, EventArgs.Empty);
        }
        public event EventHandler TurboPumpStatusChanged;
        public event TurboPumpAlertEventHandler TurboPumpAlert;
        public event EventHandler<EventArgs<uint[]>> CountsUpdated;
        protected void OnCountsUpdated() {
            CountsUpdated.Raise(this, new EventArgs<uint[]>(Detectors));
        }

        public void RelaysState(byte value) {
            throw new NotImplementedException();
        }
        public void OperationReady(bool on) {
            State = SwitchState(State, DeviceStates.HVE, on);
        }

        public void OperationBlock(bool on) {
            State = SwitchState(State, DeviceStates.PRGE, on);
        }

        public void UpdateStatus(params ValueType[] data) {
            OnDeviceStatusChanged();
        }
        public void UpdateVacuumStatus(params ValueType[] data) {
            try {
                var temp = State;
                var temp2 = temp;
                SwitchState(temp, DeviceStates.Turbo, (bool)(data[0]));
                temp = SwitchState(temp, DeviceStates.SEMV1, (bool)(data[1]));

                temp = SwitchState(temp, DeviceStates.Relay2, (bool)(data[2]));
                temp = SwitchState(temp, DeviceStates.Relay3, (bool)(data[3]));
                temp = SwitchState(temp, DeviceStates.Alert, (int)(data[4]) != 0);
                State = temp;
                if (temp2 != temp)
                    // TODO: proper data!
                    OnVacuumStateChanged();
            } catch (InvalidCastException) {
            };
        }
        uint[] detectors = new uint[3];
        public uint[] Detectors {
            get { return (uint[])detectors.Clone(); }
            set {
                if (value == null || value.Length != detectors.Length)
                    return;
                value.CopyTo(detectors, 0);
                OnCountsUpdated();
            }
        }

        #endregion
        DeviceStates SwitchState(DeviceStates state, DeviceStates flag, bool on) {
            if ((state & flag) != 0)
                state ^= flag;
            state |= on ? flag : DeviceStates.None;
            return state;
        }
    }
}
