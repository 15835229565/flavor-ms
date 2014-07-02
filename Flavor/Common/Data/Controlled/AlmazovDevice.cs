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
        public event EventHandler DeviceStatusChanged;
        // workaround for detecting Relay1 change
        public event EventHandler VacuumStateChanged;
        protected void OnVacuumStateChanged(bool on) {
            VacuumStateChanged.Raise(this, new EventArgs<bool>(on));
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
            try {
                var temp = State;
                SwitchState(temp, DeviceStates.Turbo, (bool)(data[0]));
                // workaround for detecting Relay1 change
                //var temp2 = temp;
                temp = SwitchState(temp, DeviceStates.SEMV1, (bool)(data[1]));
                //if (temp2 != temp)
                //    OnVacuumStateChanged((bool)(data[1]));

                temp = SwitchState(temp, DeviceStates.Relay2, (bool)(data[2]));
                temp = SwitchState(temp, DeviceStates.Relay3, (bool)(data[3]));
                temp = SwitchState(temp, DeviceStates.Alert, (int)(data[4]) != 0);
                State = temp;
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
