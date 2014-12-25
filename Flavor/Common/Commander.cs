using System;
using Flavor.Common.Messaging;
using Flavor.Common.Data.Measure;
using Flavor.Common.Settings;
using System.Collections.Generic;
using Flavor.Common.Library;

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
        protected abstract IRealizer GetRealizer(PortLevel port, Func<bool> notRare);
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
            private set {
                if (programState != value) {
                    programState = value;
                    if (value == ProgramStates.Start)
                        Disable();
                    OnProgramStateChanged(value);
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
        protected virtual void OnProgramStateChanged(ProgramStates state) {
            var temp = ProgramStateChanged;
            if (temp != null)
                temp(state);
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
            PortLevel.PortStates res = port.Open(Config.Port, Config.BaudRate);
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
                    port.Open(Config.Port, Config.BaudRate);
            }
        }
        public string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }

        protected void Init(object sender, CallBackEventArgs<bool> e) {
            OnLog(pState.ToString());

            setProgramState(ProgramStates.WaitInit);
            e.Value = true;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationToggle(true);

            OnLog(pState.ToString());
        }
        protected void Shutdown(object sender, CallBackEventArgs<bool> e) {
            Disable();
            setProgramState(ProgramStates.WaitShutdown);
            e.Value = true;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationToggle(false);
            // TODO: добавить контрольное время ожидания выключения
        }
        // TODO: private set!
        protected bool hBlock { get; set; }
        protected void Unblock(object sender, CallBackEventArgs<bool> e) {
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                MeasureCancelRequested = true;
            // TODO: check!
            e.Value = hBlock;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationBlock(hBlock);
        }

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
        // TODO: protected
        public void Scan() {
            if (pState == ProgramStates.Ready) {
                var g = Graph.MeasureGraph.Instance;
                g.Reset();
                {
                    var co = g.CommonOptions;
                    var temp = new MeasureMode.Scan(Config.sPoint, Config.ePoint,
                        co.befTimeReal, co.iTimeReal, co.eTimeReal);
                    //var temp = new MeasureMode.Scan(Config.sPoint, Config.ePoint,
                    //    co.befTimeReal, co.iTimeReal, co.eTimeReal,
                    //    2);
                    temp.SuccessfulExit += (s, e) => Config.autoSaveSpectrumFile();
                    temp.GraphUpdateDelegate = (p, peak) => g.updateGraphDuringScanMeasure(p, Counts);
                    CurrentMeasureMode = temp;
                }
                initMeasure(ProgramStates.Measure);
            }
        }
        protected abstract uint[] Counts { get; }
        // TODO: protected
        public bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    var g = Graph.MeasureGraph.Instance;
                    g.Reset();
                    {
                        var co = g.CommonOptions;
                        var temp = new MeasureMode.Precise(Config.MIN_STEP, Config.MAX_STEP,
                            g.PreciseData.GetUsed(),
                            co.befTimeReal, co.iTimeReal, co.eTimeReal,
                            co.ForwardTimeEqualsBeforeTime ? co.befTimeReal : co.fTimeReal, co.bTimeReal);
                        temp.SaveResults += (s, e) => Config.autoSavePreciseSpectrumFile();
                        temp.SuccessfulExit += (s, e) => {
                            var ee = (MeasureMode.Precise.SuccessfulExitEventArgs)e;
                            g.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                        };
                        temp.GraphUpdateDelegate = (p, peak) => g.updateGraphDuringPreciseMeasure(p, peak, Counts);
                        CurrentMeasureMode = temp;
                    }
                    initMeasure(ProgramStates.Measure);
                    return true;
                } else {
                    OnLog("No points for precise mode measure.");
                    return false;
                }
            }
            return false;
        }
        public bool SomePointsUsed {
            get {
                return Config.PreciseData.Exists(ped => ped.Use);
            }
        }
        // TODO: use simple arrays
        FixedSizeQueue<List<long>> background;
        List<PreciseEditorData> peaksForMatrix;
        Matrix matrix;
        List<long> backgroundResult;
        bool doBackgroundPremeasure;
        // TODO: protected
        int? LabelNumber { get; set; }
        public bool? Monitor(params object[] data) {
            // TODO: move partially up
            byte backgroundCycles = Config.BackgroundCycles;
            doBackgroundPremeasure = backgroundCycles != 0;

            Graph.MeasureGraph g;
            switch (pState) {
                case ProgramStates.Ready:
                    if (SomePointsUsed) {
                        //Order is important here!!!! Underlying data update before both matrix formation and measure mode init.
                        g = Graph.MeasureGraph.Instance;
                        g.ResetForMonitor();

#warning matrix is formed too early
                        // TODO: move matrix formation to manual operator actions
                        // TODO: parallelize matrix formation, flag on completion
                        // TODO: duplicates
                        peaksForMatrix = g.PreciseData.GetUsed().GetWithId();
                        if (peaksForMatrix.Count > 0) {
                            // To comply with other processing order (and saved information)
                            peaksForMatrix.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                            matrix = new Matrix(Config.LoadLibrary(peaksForMatrix));
                            // What do with empty matrix?
                            if (matrix != null)
                                matrix.Init();
                            else {
                                OnLog("Error in peak data format or duplicate substance.");
                                return null;
                            }
                        } else
                            matrix = null;

                        // TODO: feed measure mode with start shift value (really?)
                        short? startShiftValue = 0;
                        {
                            var co = Config.CommonOptions;
                            var temp = new MeasureMode.Precise.Monitor(Config.MIN_STEP, Config.MAX_STEP,
                                // TODO: getWithId()
                                g.PreciseData.GetUsed(),
                                co.befTimeReal, co.iTimeReal, co.eTimeReal,
                                co.ForwardTimeEqualsBeforeTime ? co.befTimeReal : co.fTimeReal, co.bTimeReal,
                                // TODO: move extra data into checker
                                Config.CheckerPeak, Config.CheckerPeak == null ? null : startShiftValue, Config.AllowedShift, Config.Iterations, Config.TimeLimit);
                            temp.SaveResults += (s, e) => {
                                Config.autoSaveMonitorSpectrumFile(LabelNumber);
                                if (LabelNumber.HasValue)
                                    LabelNumber = null;
                            };
                            temp.Finalize += (s, e) => Config.finalizeMonitorFile();
                            temp.GraphUpdateDelegate = (p, peak) => g.updateGraphDuringPreciseMeasure(p, peak, Counts);
                            temp.SuccessfulExit += (s, e) => {
                                var ee = (MeasureMode.Precise.SuccessfulExitEventArgs)e;
                                g.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                            };
                            CurrentMeasureMode = temp;
                        }

                        if (doBackgroundPremeasure) {
                            initMeasure(ProgramStates.WaitBackgroundMeasure);
                            background = new FixedSizeQueue<List<long>>(backgroundCycles);
                            // or maybe Enumerator realization: one item, always recounting (accumulate values)..
                            g.GraphDataModified += NewBackgroundMeasureReady;
                        } else {
                            initMeasure(ProgramStates.Measure);
                            g.GraphDataModified += NewMonitorMeasureReady;
                        }
                        return true;
                    } else {
                        OnLog("No points for monitor mode measure.");
                        return null;
                    }
                case ProgramStates.BackgroundMeasureReady:
                    // set background end label
                    LabelNumber = 0;

                    g = Graph.MeasureGraph.Instance;
                    g.GraphDataModified -= NewBackgroundMeasureReady;

                    backgroundResult = background.Aggregate(Summarize);
                    for (int i = 0; i < backgroundResult.Count; ++i) {
                        // TODO: check integral operation behaviour here
                        backgroundResult[i] /= backgroundCycles;
                    }

                    setProgramStateWithoutUndo(ProgramStates.Measure);
                    g.GraphDataModified += NewMonitorMeasureReady;
                    return false;
                case ProgramStates.Measure:
                    // set label
                    LabelNumber = (int)data[0];
                    return false;
                default:
                    // wrong state, strange!
                    return null;
            }
        }
        void NewBackgroundMeasureReady(object sender, EventArgs<int[]> e) {
            var currentMeasure = new List<long>();
            foreach (var ped in peaksForMatrix) {
                //!!!!! null PLSreference! race condition?
                currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
            }
            //maybe null if background premeasure is false!
            background.Enqueue(currentMeasure);
            if (pState == ProgramStates.WaitBackgroundMeasure && background.IsFull) {
                setProgramStateWithoutUndo(ProgramStates.BackgroundMeasureReady);
            }
        }
        void NewMonitorMeasureReady(object sender, EventArgs<int[]> e) {
            var currentMeasure = new List<long>();
            foreach (var ped in peaksForMatrix) {
                currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
            }
            if (doBackgroundPremeasure) {
                if (currentMeasure.Count != backgroundResult.Count) {
                    // length mismatch
                    // TODO: throw smth
                }
                // distract background
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    currentMeasure[i] -= backgroundResult[i];
                }
            }
            if (matrix != null) {
                // solve matrix equation
                double[] result = matrix.Solve(currentMeasure.ConvertAll<double>(x => (double)x));
                // TODO: now it is normalized to 999 on maximum of peak spectrum component
                // but we want actual value
                // weight of mass measured also can differ from 999
                Config.AutoSaveSolvedSpectra(result);
                // TODO: put here all automatic logic from measure modes
            }
        }
        List<long> Summarize(List<long> workingValue, List<long> nextElem) {
            // TODO: move from Commander to Utility
            if (workingValue.Count != nextElem.Count)
                // data length mismatch
                return null;
            for (int i = 0; i < workingValue.Count; ++i) {
                workingValue[i] += nextElem[i];
            }
            return workingValue;
        }
        void initMeasure(ProgramStates state) {
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            // TODO: move inside MeasureMode
            SubscribeToCountsUpdated(deviceCountsUpdated);

            setProgramState(state);

            MeasureCancelRequested = false;
            SendSettings();
        }
        void deviceCountsUpdated(object sender, EventArgs<uint[]> countsData) {
            CurrentMeasureMode.UpdateGraph();
            //if (!CurrentMeasureMode.onUpdateCounts(device.Detectors)) {
            if (!CurrentMeasureMode.onUpdateCounts(countsData.Value)) {
                OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
            }
        } 
        void CurrentMeasureMode_Disable(object sender, EventArgs e) {
            if (CurrentMeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.MeasureGraph.Instance.GraphDataModified -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.MeasureGraph.Instance.GraphDataModified -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            // TODO: move inside MeasureMode
            UnsubscribeToCountsUpdated(deviceCountsUpdated);
            CurrentMeasureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable -= CurrentMeasureMode_Disable;

            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
            Disable();
        }
        protected abstract void SubscribeToCountsUpdated(EventHandler<EventArgs<uint[]>> handler);
        protected abstract void UnsubscribeToCountsUpdated(EventHandler<EventArgs<uint[]>> handler);
        void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            realizer.SetMeasureStep(e.Step);
            // TODO: move to realizer ctor as extra action on measure step
            if (notRareModeRequested) {
                ExtraActionOnMeasureStep();
            }
        }
        protected virtual void ExtraActionOnMeasureStep() { }
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
