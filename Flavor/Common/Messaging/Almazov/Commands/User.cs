using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using System;

namespace Flavor.Common.Messaging.Almazov.Commands {
    abstract class UserRequest: Flavor.Common.Messaging.UserRequest<Flavor.Common.Messaging.Almazov.CommandCode> {
        public sealed override IList<byte> Data {
            get { return AlexProtocol.collectData(Id, Parameters); }
        }
        protected object[] Params(params object[] ps) {
            return ps;
        }
        readonly object[] empty = new object[0];
        protected virtual object[] Parameters {
            get { return empty; }
        }
    }
    
    class CPUStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.CPU_Status; }
        }
    }
    class TICStatusRequest: UserRequest, ITIC {
        public string Request { get { return "?V902\r"; } }
        protected override object[] Parameters {
            get { return Params(Request); }
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
    class VacuumStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.TIC_GetStatus; }
        }
    }
    class HighVoltagePermittedStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.HVE; }
        }
    }
    abstract class FlagRequest: UserRequest {
        readonly bool? on;
        protected FlagRequest(bool? on) {
            this.on = on;
        }
        protected sealed override object[] Parameters {
            get { return Params(on.HasValue ? (byte)(on.Value ? 1 : 0) : byte.MaxValue); }
        }
    }
    class OperationBlockRequest: FlagRequest {
        public OperationBlockRequest(bool? on)
            : base(on) { }
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
    }
    
    class Valve1Request: FlagRequest {
        public Valve1Request(bool? on)
            : base(on) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV1; }
        }
    }
    class Valve2Request: FlagRequest {
        public Valve2Request(bool? on)
            : base(on) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV2; }
        }
    }
    class Valve3Request: FlagRequest {
        public Valve3Request(bool? on)
            : base(on) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV3; }
        }
    }
    class MicroPumpRequest: FlagRequest {
        public MicroPumpRequest(bool? on)
            : base(on) { }
        public override CommandCode Id {
            get { return CommandCode.SPUMP; }
        }
    }

    class AllVoltagesRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SPI_GetAllVoltages; }
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
        protected sealed override object[] Parameters {
            get {
                byte first = channel;
                --first;
                first <<= 4;
                first += (byte)(voltage >> 8);
                byte second = (byte)(voltage & 0xFF);
                return Params(first, second);
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
            get { return CommandCode.SPI_DPS_SetVoltage; }
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
    abstract class PreciseVoltageSetRequest: DACADCRequest {
        readonly uint voltage;
        protected PreciseVoltageSetRequest(byte channel, uint voltage)
            : base((byte)(channel + 24)) {
            this.voltage = voltage;
        }
        public static byte[] VoltageBytes(uint voltage) {
            voltage &= 0x3FFF;
            //TODO: check IsLittleEndian in BitConverter
            return BitConverter.GetBytes(voltage << 2);
        }
        protected sealed override object[] Parameters {
            get {
                byte[] bytes = VoltageBytes(voltage);
                return Params(channel, bytes[1], bytes[0]);
            }
        }
    }
    class CapacitorVoltageSetRequest: PreciseVoltageSetRequest {
        public CapacitorVoltageSetRequest(uint voltage)
            : base(0, voltage) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_CP_SetVoltage; }
        }
    }
    abstract class ScanVoltageSetRequest: PreciseVoltageSetRequest {
        protected ScanVoltageSetRequest(byte channel, uint voltage)
            : base(channel, voltage) { }
        public override CommandCode Id {
            get { return CommandCode.SPI_Scan_SetVoltage; }
        }
    }
    class ParentScanVoltageSetRequest: ScanVoltageSetRequest {
        public ParentScanVoltageSetRequest(uint voltage)
            : base(0, voltage) { }
    }
    class MainScanVoltageSetRequest: ScanVoltageSetRequest {
        public MainScanVoltageSetRequest(uint voltage)
            : base(1, voltage) { }
    }
    abstract class GetADCRequest: DACADCRequest, IChannel {
        const byte HBYTE = 127;
        const byte LBYTE_DoubleRange = 16;
        const byte CHANNEL_STEP = 4;
        protected GetADCRequest(byte channel)
            : base(channel) { }
        protected sealed override object[] Parameters {
            get {
                byte first = channel;
                first *= CHANNEL_STEP;
                first += HBYTE;
                return Params(first, LBYTE_DoubleRange);
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

    //class GetScanVoltageRequest { }

    class SendMeasureRequest: UserRequest {
        static SendMeasureRequest cachedRequest = null;
        public static SendMeasureRequest Form(uint ms) {
            return (cachedRequest == null || cachedRequest.ms != ms) ? (cachedRequest = new SendMeasureRequest(ms)) : cachedRequest;
        }
        readonly uint ms;
        SendMeasureRequest(uint ms) {
            this.ms = ms;
        }
        protected sealed override object[] Parameters {
            get {
                byte[] MeasurePeriod = BitConverter.GetBytes(calcRTCticks());
                return Params(calcRTCprescaler(), MeasurePeriod[1], MeasurePeriod[0]);
            }
        }
        const int min_ms_div1 = 0;
        const int min_ms_div2 = 2000;
        const int min_ms_div8 = 4000;
        const int min_ms_div16 = 16000;
        const int min_ms_div64 = 32000;
        const int min_ms_div256 = 127996;
        const int min_ms_div1024 = 511981;
        const int max_ms_div1024 = 2047925;
        byte calcRTCprescaler() {
            byte prescaler; //Предделитель
            if ((ms >= min_ms_div1) && (ms < min_ms_div2)) {
                prescaler = 1;
            } else if ((ms >= min_ms_div2) && (ms < min_ms_div8)) {
                prescaler = 2;
            } else if ((ms >= min_ms_div8) && (ms < min_ms_div16)) {
                prescaler = 3;
            } else if ((ms >= min_ms_div16) && (ms < min_ms_div64)) {
                prescaler = 4;
            } else if ((ms >= min_ms_div64) && (ms < min_ms_div256)) {
                prescaler = 5;
            } else if ((ms >= min_ms_div256) && (ms < min_ms_div1024)) {
                prescaler = 6;
            } else if ((ms >= min_ms_div1024) && (ms < max_ms_div1024)) {
                prescaler = 7;
            } else {
                return 0;
            }
            return prescaler;
        }
        ushort calcRTCprescaler_long() {
            //ФУНКЦИЯ: Вычисляет, сохраняет и возвращает предделитель.
            ushort prescaler_long; //Предделитель в реальном коэффициенте деления (см. prescaler цифры в скобках)
            if ((ms >= min_ms_div1) && (ms < min_ms_div2)) {
                prescaler_long = 1;
            } else if ((ms >= min_ms_div2) && (ms < min_ms_div8)) {
                prescaler_long = 2;
            } else if ((ms >= min_ms_div8) && (ms < min_ms_div16)) {
                prescaler_long = 8;
            } else if ((ms >= min_ms_div16) && (ms < min_ms_div64)) {
                prescaler_long = 16;
            } else if ((ms >= min_ms_div64) && (ms < min_ms_div256)) {
                prescaler_long = 64;
            } else if ((ms >= min_ms_div256) && (ms < min_ms_div1024)) {
                prescaler_long = 256;
            } else if ((ms >= min_ms_div1024) && (ms < max_ms_div1024)) {
                prescaler_long = 1024;
            } else {
                return 0;
            }
            return prescaler_long;
        }
        const double sourceFrequency = 32.768;//кГц - опорная частота таймера
        ushort calcRTCticks() {
            //ФУНКЦИЯ: Вычисляет количество тиков в соответствии с временем и предделителем. Возвращает количество тиков
            ushort ticks;
            if ((ms < min_ms_div1) || (calcRTCprescaler() == 0)) {
                return ushort.MaxValue;
            }
            ticks = Convert.ToUInt16(Math.Round(Convert.ToDouble(ms) * (sourceFrequency / calcRTCprescaler_long())));
            return ticks;
        }
        public override CommandCode Id {
            get { return CommandCode.RTC_StartMeasure; }
        }
    }
    class DelayedMeasureRequest: UserRequest {
        readonly uint eT, iT, sV, psV, cV;
        public DelayedMeasureRequest(uint eT, uint iT, uint sV, uint psV, uint cV) {
            this.eT = eT;
            this.iT = iT;
            this.sV = sV;
            this.psV = psV;
            this.cV = cV;
        }
        protected override object[] Parameters {
            get {
                byte[] scan = PreciseVoltageSetRequest.VoltageBytes(sV);
                byte[] parentScan = PreciseVoltageSetRequest.VoltageBytes(psV);
                byte[] capacitor = PreciseVoltageSetRequest.VoltageBytes(cV);
                byte[] delay = BitConverter.GetBytes(iT);
                byte[] exposition = BitConverter.GetBytes(eT);
                //TODO: check IsLittleEndian in BitConverter
                return Params(scan[1], scan[0], parentScan[1], parentScan[0], capacitor[1], capacitor[0], delay[0], delay[1], exposition[0], exposition[1]);
            }
        }
        public override CommandCode Id {
            get { return CommandCode.RTC_DelayedStart; }
        }
    }
    
    class CountsRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.RTC_ReceiveResults; }
        }
    }
}