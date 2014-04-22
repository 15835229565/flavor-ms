using System;
using Flavor.Common.Messaging;

namespace Flavor.Common {
    abstract class Commander: IErrorOccured, IAsyncReplyReceived, IGlobalActions, IMeasureActions {
        // TODO: use binding flags to bind proper controls (common and measure)
        public virtual void Bind(IMSControl view) {
            view.Connect += Connect;
        }
        readonly EventHandler undoProgramState;
        // BAD!
        public readonly IDevice device;
        protected Commander(PortLevel port, IDevice device) {
            this.port = port;
            port.ErrorPort += (s, e) => {
                // TODO: more accurate
                OnErrorOccured(e.Message);
            };
            this.device = device;

            undoProgramState = (s, e) => setProgramStateWithoutUndo(pStatePrev);
            notRareModeRequested = false;
            var r = GetRealizer(port, notRare);
            ConsoleWriter.Subscribe(r);

            // eliminate local events!
            ProgramStateChanged += s => {
                if (s == ProgramStates.Measure || s == ProgramStates.BackgroundMeasureReady || s == ProgramStates.WaitBackgroundMeasure) {
                    r.Reset();
                }
            };
            MeasureCancelled += s => r.Reset();

            r.SystemReady += (s, e) => {
                if (hBlock) {
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                } else {
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                }
            };
            r.UpdateDevice += (s, e) => {
                // TODO: proper device
                if (device != null)
                    e.Value.UpdateDevice(device);
            };
            r.OperationBlock += (s, e) => {
                hBlock = e.Value;
                if (hBlock) {
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
                } else {
                    if (pState == ProgramStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                }
            };
            r.MeasurePreconfigured += (s, e) => {
                if (pState == ProgramStates.Measure ||
                    pState == ProgramStates.WaitBackgroundMeasure) {
                    if (!CurrentMeasureMode.Start()) {
                        OnErrorOccured("Нет точек для измерения.");
                    }
                }
            };
            r.MeasureSend += (s, e) => {
                if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                    CurrentMeasureMode.NextMeasure(e.Value);
                }
            };
            r.MeasureDone += (s, e) => {
                if (CurrentMeasureMode == null) {
                    // fake reply caught here (in order to put device into proper state)
                    hBlock = false;
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                }
            };
            this.realizer = r;

            hBlock = true;
        }
        protected abstract IRealizer GetRealizer(PortLevel port, Generator<bool> notRare);
        bool notRare() {
            if (pState == ProgramStates.Measure || pState == ProgramStates.BackgroundMeasureReady || pState == ProgramStates.WaitBackgroundMeasure)
                return notRareModeRequested;
            return true;
        }
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string msg) {
            var temp = Log;
            if (temp != null)
                temp(msg);
        }
        #endregion

        #region IErrorOccured Members
        public event MessageHandler ErrorOccured;
        protected virtual void OnErrorOccured(string msg) {
            var temp = ErrorOccured;
            if (temp != null)
                temp(msg);
            OnLog(msg);
        }
        #endregion

        #region IAsyncReplyReceived Members
        public event MessageHandler AsyncReplyReceived;
        protected virtual void OnAsyncReplyReceived(string msg) {
            var temp = AsyncReplyReceived;
            if (temp != null)
                temp(msg);
            OnLog(msg);
        }
        #endregion

        ProgramStates pStatePrev = ProgramStates.Start;
        protected void setProgramStateWithoutUndo(ProgramStates state) {
            pState = state;
            pStatePrev = pState;
        }
        protected void setProgramState(ProgramStates state) {
            pStatePrev = pState;
            pState = state;
        }

        ProgramStates programState = ProgramStates.Start;
        public ProgramStates pState {
            get {
                return programState;
            }
            protected set {
                if (programState != value) {
                    programState = value;
                    if (value == ProgramStates.Start)
                        Disable();
                    OnProgramStateChanged();
                };
            }
        }
        public event BoolEventHandler RareModeChanged;
        protected virtual void OnRareModeChanged(bool t) {
            var temp = RareModeChanged;
            if (temp != null)
                temp(t);
        }
        bool rare;
        public bool notRareModeRequested {
            get {
                return rare;
            }
            set {
                if (rare == value)
                    return;
                rare = value;
                OnRareModeChanged(value);
            }
        }
        #region IGlobalActions Members
        public event ProgramEventHandler ProgramStateChanged;
        protected virtual void OnProgramStateChanged() {
            var temp = ProgramStateChanged;
            if (temp != null)
                temp(pState);
        }
        public void SendSettings() {
            realizer.SetSettings();
        }
        #endregion

        readonly PortLevel port;
        bool DeviceIsConnected = false;
        protected void Connect(object sender, CallBackEventArgs<bool, string> e) {
            if (DeviceIsConnected) {
                Disconnect();
            } else {
                Connect();
            }
            e.Value = DeviceIsConnected;
            // TODO: own event
            //e.Handler = this.RareModeChanged;
        }
        protected virtual PortLevel.PortStates Connect() {
            PortLevel.PortStates res = port.Open();
            switch (res) {
                case PortLevel.PortStates.Opening:
                    realizer.Undo += undoProgramState;
                    realizer.Connect();

                    DeviceIsConnected = true;
                    break;
                case PortLevel.PortStates.Opened:
                    DeviceIsConnected = true;
                    break;
                case PortLevel.PortStates.ErrorOpening:
                    break;
                default:
                    // фигня
                    break;
            }
            return res;
        }
        protected virtual void Disconnect() {
            realizer.Disconnect();
            realizer.Undo -= undoProgramState;

            PortLevel.PortStates res = port.Close();
            switch (res) {
                case PortLevel.PortStates.Closing:
                    DeviceIsConnected = false;
                    break;
                case PortLevel.PortStates.Closed:
                    DeviceIsConnected = false;
                    break;
                case PortLevel.PortStates.ErrorClosing:
                    break;
                default:
                    // фигня
                    break;
            }
        }
        // Used in MainForm
        public void Reconnect() {
            if (DeviceIsConnected) {
                Disconnect();
                if (!DeviceIsConnected)
                    port.Open();
            }
        }
        public string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }

        // TODO: private set!
        protected bool hBlock { get; set; }

        readonly IRealizer realizer;
        public MeasureMode CurrentMeasureMode { get; protected set; }
        bool measureCancelRequested = false;
        public bool MeasureCancelRequested {
            protected get { return measureCancelRequested; }
            set {
                measureCancelRequested = value;
                if (value && CurrentMeasureMode != null)
                    CurrentMeasureMode.CancelRequested = value;
            }
        }
        // TODO: other event class here!
        public event ProgramEventHandler MeasureCancelled;
        protected virtual void OnMeasureCancelled() {
            var temp = MeasureCancelled;
            if (temp != null)
                temp(pState);
        }
        abstract public void Scan();
        abstract public bool Sense();
        public bool SomePointsUsed {
            get {
                if (Config.PreciseData.Count > 0)
                    foreach (Utility.PreciseEditorData ped in Config.PreciseData)
                        if (ped.Use) return true;
                return false;
            }
        }
        abstract public bool? Monitor();
        protected void Disable() {
            MeasureCancelRequested = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            CurrentMeasureMode = null;//?
        }
        protected void SubscribeToUndo(EventHandler handler) {
            ProgramEventHandler ph = s => realizer.Undo -= handler; ;
            ph += s => this.ProgramStateChanged -= ph;
            handler += (s, e) => {
                realizer.Undo -= handler;
                this.ProgramStateChanged -= ph;
            };
            realizer.Undo += handler;
            this.ProgramStateChanged += ph;
        }
    }
}
