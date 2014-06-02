using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using UserRequest = Flavor.Common.Messaging.UserRequest<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov.Commands {
    class CPUStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.CPU_Status; }
        }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id); }
        }
    }
    class HighVoltagePermittedStatusRequest: UserRequest {
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id); }
        }
        public override CommandCode Id {
            get { return CommandCode.HVE; }
        }
    }
    class OperationBlockRequest: UserRequest {
        readonly bool? on;
        public OperationBlockRequest(bool? on) {
            this.on = on;
        }
        public override IList<byte> Data {
            get {
                if (on.HasValue) {
                    ;
                }
                return AlexProtocol.collectData(Id, on.HasValue ? (byte)(on.Value ? 1 : 0) : byte.MaxValue);
            }
        }
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
    }
    class TICStatusRequest: UserRequest, ITIC {
        public string Request { get { return "?V902\r"; } }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id, Request); }
        }
        public override CommandCode Id {
            get { return CommandCode.TIC_Retransmit; }
        }
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as ITIC).Request.Equals(this.Request);
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 17 * Request.GetHashCode();
        }
    }
    class Valve1Request: UserRequest {
        readonly bool? on;
        public Valve1Request(bool? on) {
            this.on = on;
        }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id, on.HasValue ? (byte)(on.Value ? 1 : 0) : byte.MaxValue); }
        }
        public override CommandCode Id {
            get { return CommandCode.SEMV1; }
        }
    }
    abstract class DACADCRequest: UserRequest {
        readonly protected byte channel;
        protected DACADCRequest(byte channel) {
            this.channel = channel;
        }
    }
    abstract class SetDACRequest: DACADCRequest {
        readonly ushort voltage;
        protected SetDACRequest(byte channel, ushort voltage)
            : base(channel) {
            voltage &= 0xFFF;
            this.voltage = voltage;
        }
        public override IList<byte> Data {
            get {
                byte first = channel;
                --first;
                first <<= 4;
                first += (byte)(voltage >> 8);
                byte second = (byte)(voltage & 0xFF);
                return AlexProtocol.collectData(Id, first, second);
            }
        }
    }
    abstract class IonSourceSetRequest: SetDACRequest {
        protected IonSourceSetRequest(byte channel, ushort voltage)
            : base(channel, voltage) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSIS_SetVoltage; }
        }
    }
    class SetEmissionCurrentRequest: IonSourceSetRequest {
        public SetEmissionCurrentRequest(ushort voltage)
            : base(1, voltage) { }
    }
    class SetIonizationVoltageRequest: IonSourceSetRequest {
        public SetIonizationVoltageRequest(ushort voltage)
            : base(2, voltage) { }
    }
    class SetF1VoltageRequest: IonSourceSetRequest {
        public SetF1VoltageRequest(ushort voltage)
            : base(3, voltage) { }
    }
    class SetF2VoltageRequest: IonSourceSetRequest {
        public SetF2VoltageRequest(ushort voltage)
            : base(4, voltage) { }
    }
    abstract class DetectorSetRequest: SetDACRequest {
        protected DetectorSetRequest(byte channel, ushort voltage)
            : base(channel, voltage) { }
        public override CommandCode Id {
            get {
                return CommandCode.SPI_DPS_SetVoltage;
            }
        }
    }
    class SetD1VoltageRequest: DetectorSetRequest {
        public SetD1VoltageRequest(ushort voltage)
            : base(1, voltage) { }
    }
    class SetD2VoltageRequest: DetectorSetRequest {
        public SetD2VoltageRequest(ushort voltage)
            : base(2, voltage) { }
    }
    class SetD3VoltageRequest: DetectorSetRequest {
        public SetD3VoltageRequest(ushort voltage)
            : base(3, voltage) { }
    }
    abstract class InletSetRequest: SetDACRequest {
        protected InletSetRequest(byte channel, ushort voltage)
            : base(channel, voltage) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSInl_SetVoltage; }
        }
    }
    class SetInletVoltageRequest: InletSetRequest {
        public SetInletVoltageRequest(ushort voltage)
            : base(1, voltage) { }
    }
    class SetHeaterVoltageRequest: InletSetRequest {
        public SetHeaterVoltageRequest(ushort voltage)
            : base(2, voltage) { }
    }
    abstract class GetADCRequest: DACADCRequest, IChannel {
        const byte HBYTE = 127;
        const byte LBYTE_DoubleRange = 16;
        const byte CHANNEL_STEP = 4;
        protected GetADCRequest(byte channel)
            : base(channel) { }
        public override IList<byte> Data {
            get {
                byte first = channel;
                first *= CHANNEL_STEP;
                first += HBYTE;
                return AlexProtocol.collectData(Id, first, LBYTE_DoubleRange);
            }
        }
        // what to do with reply?
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as IChannel).Channel == this.Channel;
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 19 * Channel.GetHashCode();
        }

        #region IChannel Members
        public byte Channel {
            get { return channel; }
        }
        #endregion
    }
    abstract class IonSourceGetRequest: GetADCRequest {
        protected IonSourceGetRequest(byte channel)
            : base(channel) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSIS_GetVoltage; }
        }
    }
    class GetEmissionCurrentRequest: IonSourceGetRequest {
        public GetEmissionCurrentRequest()
            : base(1) { }
    }
    class GetIonizationVoltageRequest: IonSourceGetRequest {
        public GetIonizationVoltageRequest()
            : base(2) { }
    }
    class GetF1VoltageRequest: IonSourceGetRequest {
        public GetF1VoltageRequest()
            : base(3) { }
    }
    class GetF2VoltageRequest: IonSourceGetRequest {
        public GetF2VoltageRequest()
            : base(4) { }
    }
    abstract class DetectorGetRequest: GetADCRequest {
        protected DetectorGetRequest(byte channel)
            : base(channel) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_DPS_GetVoltage; }
        }
    }
    class GetD1VoltageRequest: DetectorGetRequest {
        public GetD1VoltageRequest()
            : base(1) { }
    }
    class GetD2VoltageRequest: DetectorGetRequest {
        public GetD2VoltageRequest()
            : base(2) { }
    }
    class GetD3VoltageRequest: DetectorGetRequest {
        public GetD3VoltageRequest()
            : base(3) { }
    }
    abstract class InletGetRequest: GetADCRequest {
        protected InletGetRequest(byte channel)
            : base(channel) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSInl_GetVoltage; }
        }
    }
    class GetInletVoltageRequest: InletGetRequest {
        public GetInletVoltageRequest()
            : base(1) { }
    }
    class GetHeaterVoltageRequest: InletGetRequest {
        public GetHeaterVoltageRequest()
            : base(2) { }
    }
    class CountsRequest: UserRequest {
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id); }
        }
        public override CommandCode Id {
            get { return CommandCode.RTC_ReceiveResults; }
        }
    }
}