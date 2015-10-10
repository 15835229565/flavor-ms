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
            Relay3 = 8,
            Relay2 = 16,
            HVE = 32,
            PRGE = 64,
            Alert = 128
        }
        DeviceStates _state = DeviceStates.None;
        DeviceStates State {
            get { return _state; }
            set {
                if (value != _state) {
                    _state = value;
                    OnDeviceStateChanged((byte)value);
                }
            }
        }

        #region IDevice Members
        public event EventHandler<EventArgs<byte>> DeviceStateChanged;
        protected void OnDeviceStateChanged(byte state) {
            DeviceStateChanged.Raise(this, new EventArgs<byte> { Value = state });
        }
        // TODO: use!
        public event EventHandler<EventArgs<ValueType[]>> DeviceStatusChanged;
        protected void OnDeviceStatusChanged(params ValueType[] data) {
            DeviceStatusChanged.Raise(this, new EventArgs<ValueType[]> { Value = data });
        }
        // TODO: use!
        public event EventHandler<EventArgs<ValueType[]>> VacuumStateChanged;
        protected void OnVacuumStateChanged(params ValueType[] data) {
            VacuumStateChanged.Raise(this, new EventArgs<ValueType[]> { Value = data });
        }
        public event EventHandler TurboPumpStatusChanged;
        public event TurboPumpAlertEventHandler TurboPumpAlert;
        public event EventHandler<EventArgs<uint[]>> CountsUpdated;
        protected void OnCountsUpdated() {
            CountsUpdated.Raise(this, new EventArgs<uint[]> { Value = Detectors });
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
            byte flags = (byte)data[13];
            bool HVEport = !CheckBit(flags, 8);
            bool HVE = !CheckBit(flags, 7);
            bool PRGE = CheckBit(flags, 6);
            bool EDCD = CheckBit(flags, 5);
            bool SEMV1 = CheckBit(flags, 4);
            bool SEMV2 = CheckBit(flags, 3);
            bool SEMV3 = CheckBit(flags, 2);
            bool SPUMP = CheckBit(flags, 1);

            var temp = State;
            var temp2 = temp;
            temp = SwitchState(temp, DeviceStates.SEMV1, SEMV1);
            if (temp2 != temp) {
                // TODO: proper data!
                OnVacuumStateChanged();
            }
            temp = SwitchState(temp, DeviceStates.HVE, HVE);
            temp = SwitchState(temp, DeviceStates.PRGE, PRGE);
            State = temp;

            bool isFake = true;
            for (int i = 0; i < 13; ++i) {
                if ((ushort)data[i] != 0xFFF) {
                    isFake = false;
                    break;
                }
            }
            if (isFake) {
                OnDeviceStatusChanged(SEMV2,
                    SEMV3,
                    SPUMP && PRGE//actual state
                    );
            } else {
                OnDeviceStatusChanged(SEMV2,
                    SEMV3,
                    SPUMP && PRGE,//actual state
                    50.0 * (ushort)data[0] / 4096,//eI
                    100.0 * (ushort)data[1] / 4096,//iV
                    100.0 * (ushort)data[2] / 4096,//fV1
                    100.0 * (ushort)data[3] / 4096,//fV2
                    5000.0 * (ushort)data[4] / 4096,//d1V
                    5000.0 * (ushort)data[5] / 4096,//d2V
                    5000.0 * (ushort)data[6] / 4096,//d3V
                    500.0 * (ushort)data[7] / 4096,//cVp ?!!!
                    500.0 * (ushort)data[8] / 4096,//cVm ?!!!
                    5000.0 * (ushort)data[9] / 4096,//sV
                    5000.0 * (ushort)data[10] / 4096,//psV
                    1.25 * (ushort)data[11],//inV
                    500.0 * (ushort)data[12] / 4096 - 273.15//hT celtigrate
                );
            }
        }
        bool CheckBit(byte flags, int number) {
            int mask = 0x1 << --number;
            return (flags & mask) == mask;
        }
        public void UpdateVacuumStatus(params ValueType[] data) {
            try {
                var temp = State;
                var temp2 = temp;
                temp = SwitchState(temp, DeviceStates.Turbo, (bool)(data[0]));
                bool relay1 = (bool)(data[1]);
                temp = SwitchState(temp, DeviceStates.SEMV1, relay1);

                // TODO: use ConsoleWriter logging
                if (relay1 == false)
                    Console.WriteLine("Vacuum unit (TIC) relay 1 is off. Check TIC settings if this message appears constantly.");

                temp = SwitchState(temp, DeviceStates.Relay2, (bool)(data[2]));
                temp = SwitchState(temp, DeviceStates.Relay3, (bool)(data[3]));
                temp = SwitchState(temp, DeviceStates.Alert, (int)(data[4]) != 0);
                State = temp;
                if (temp2 != temp) {
                    // TODO: proper data!
                    OnVacuumStateChanged();
                }
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
