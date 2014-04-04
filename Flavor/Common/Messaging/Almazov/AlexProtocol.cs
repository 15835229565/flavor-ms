using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging.Almazov {
    internal class AlexProtocol: CheckableProtocol<CommandCode> {
        public AlexProtocol(PortLevel port)
            : base(new AlexProtocolByteDispatcher(port, false)) { }
        protected override void Parse(object sender, ByteArrayEventArgs e) {
            throw new NotImplementedException();
        }

        protected override byte ComputeCS(IEnumerable<byte> data) {
            byte checkSum = 0;
            foreach (byte b in data)
                checkSum -= b;
            return checkSum;
        }
        protected override ICollection<byte> buildPackBody(IEnumerable<byte> data, byte checksum) {
            var pack = new List<byte>(data);
            pack.Add(checksum);
            return pack;
        }

        private class AlexProtocolByteDispatcher: ByteDispatcher {
            private readonly byte LOCK = 13;
            private readonly byte KEY = 58;
            public AlexProtocolByteDispatcher(PortLevel port, bool singleByteDispatching)
                : base(port, singleByteDispatching) { }
            private readonly List<byte> packetBuffer = new List<byte>();
            private enum PacketingState {
                Idle,
                Wait,
            }
            private PacketingState state = PacketingState.Idle;
            protected override void DispatchByte(byte data) {
                switch (state) {
                    case PacketingState.Idle: {
                            if (data == KEY) {
                                packetBuffer.Clear();
                                state = PacketingState.Wait;
                            } else {
                                //Symbol outside packet
                                OnLog(string.Format("Error({0})", data));
                            }
                            break;
                        }
                    case PacketingState.Wait: {
                            if (data == LOCK) {
                                OnPackageReceived(packetBuffer.ToArray());
                                OnLog("[in]", packetBuffer);
                                packetBuffer.Clear();
                                state = PacketingState.Idle;
                            } else {
                                packetBuffer.Add(data);
                            }
                            break;
                        }
                }
            }
            private ICollection<byte> buildPack(ICollection<byte> data) {
                var pack = new List<byte>(data.Count + 2);
                pack.Add(KEY);
                pack.AddRange(data);
                pack.Add(LOCK);
                return pack;
            }
            private ICollection<byte> buildPackBody(ICollection<byte> data) {
                var pack = new List<byte>(data.Count);
                foreach (byte b in data) {
                    pack.Add(b);
                }
                return pack;
            }
            private void OnLog(string prefix, ICollection<byte> pack) {
                var sb = new StringBuilder(prefix);
                foreach (byte b in pack) {
                    sb.Append((char)b);
                    sb.Append(GetNibble(b >> 4));
                    sb.Append(GetNibble(b));
                }
                OnLog(sb.ToString());
            }
            private byte GetNibble(int data) {
                data &= 0x0F;
                if (data < 10) {
                    data += (int)'0';
                } else {
                    data += (int)'a';
                    data -= 10;
                }
                return (byte)data;
            }
            #region IByteDispatcher Members
            public override void Transmit(ICollection<byte> pack) {
                base.Transmit(buildPack(pack));
                OnLog("[out]", pack);
            }
            #endregion
        }
    }
}
