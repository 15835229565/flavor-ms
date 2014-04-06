using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.SevMorGeo;
using Flavor.Common.Library;

namespace Flavor.Common {
    class Commander2: ICommander {
        bool onTheFly = true;

        ProgramStates programState = ProgramStates.Start;
        public override ProgramStates pState {
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

        void setProgramStateWithoutUndo(ProgramStates state) {
            pState = state;
            pStatePrev = pState;
        }
        void setProgramState(ProgramStates state) {
            pStatePrev = pState;
            pState = state;
        }

        bool handleBlock = true;
        public override bool hBlock {
            get {
                return handleBlock;
            }
            set {
                if (handleBlock != value) {
                    handleBlock = value;
                    OnProgramStateChanged();
                };
            }
        }

        bool isConnected = false;
        public override bool DeviceIsConnected {
            get {
                return isConnected;
            }
            protected set {
                if (isConnected != value) {
                    isConnected = value;
                    if (value)
                        protocol.CommandReceived += Realize;
                    else
                        protocol.CommandReceived -= Realize;
                    OnProgramStateChanged();
                }
            }
        }

        MessageQueueWithAutomatedStatusChecks<CommandCode> toSend;

        bool measureCancelRequested = false;
        public override bool MeasureCancelRequested {
            protected get { return measureCancelRequested; }
            set {
                measureCancelRequested = value;
                if (value && CurrentMeasureMode != null)
                    CurrentMeasureMode.CancelRequested = value;
            }
        }

        void Realize(object sender, CommandReceivedEventArgs<CommandCode> e) {
            var command = e.Command;

            if (command is AsyncError<CommandCode>) {
                CheckInterfaces(command);

                string message = string.Format("Device says: {0}", ((AsyncError<CommandCode>)command).Message);
                OnAsyncReplyReceived(message);
                // TODO: subscribe in Config for event
                Config.logCrash(message);

                if (pState != ProgramStates.Start) {
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    MeasureCancelRequested = false;
                }
                return;
            }
            if (command is Async<CommandCode>) {
                CheckInterfaces(command);
                if (command is confirmShutdowned) {
                    OnLog("System is shutdowned");
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    hBlock = true;
                    OnLog(pState.ToString());
                    Device.Init();
                    return;
                }
                if (command is SystemReseted) {
                    OnAsyncReplyReceived("Система переинициализировалась");
                    if (pState != ProgramStates.Start) {
                        setProgramStateWithoutUndo(ProgramStates.Start);
                        MeasureCancelRequested = false;
                    }
                    return;
                }
                if (command is confirmVacuumReady) {
                    if (hBlock) {
                        setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                    return;
                }
                if (command is confirmHighVoltageOff) {
                    hBlock = true;
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
                    return;
                }
                if (command is confirmHighVoltageOn) {
                    hBlock = false;
                    if (pState == ProgramStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                    toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                    toSend.Enqueue(new sendIVoltage());// и остальные напряжения затем
                    return;
                }
                return;
            }
            if (command is SyncError<CommandCode>) {
                toSend.Dequeue();
                CheckInterfaces(command);
                return;
            }
            if (command is Sync<CommandCode>) {
                if (null == toSend.Peek((Sync<CommandCode>)command)) {
                    return;
                }
                CheckInterfaces(command);
                if (command is confirmInit) {
                    OnLog("Init request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Init);
                    OnLog(pState.ToString());
                    return;
                }
                if (command is confirmShutdown) {
                    OnLog("Shutdown request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                    OnLog(pState.ToString());
                    return;
                }
                // On The Fly part!!!
                if (onTheFly && (pState == ProgramStates.Start) && (command is updateStatus)) {
                    switch (Device.sysState) {
                        case Device.DeviceStates.Init:
                        case Device.DeviceStates.VacuumInit:
                            hBlock = true;
                            setProgramStateWithoutUndo(ProgramStates.Init);
                            break;

                        case Device.DeviceStates.ShutdownInit:
                        case Device.DeviceStates.Shutdowning:
                            hBlock = true;
                            setProgramStateWithoutUndo(ProgramStates.Shutdown);
                            break;

                        case Device.DeviceStates.Measured:
                            toSend.Enqueue(new getCounts());
                            // waiting for fake counts reply
                            break;
                        case Device.DeviceStates.Measuring:
                            // async message here with auto send-back
                            // and waiting for fake counts reply
                            break;

                        case Device.DeviceStates.Ready:
                            hBlock = false;
                            setProgramStateWithoutUndo(ProgramStates.Ready);
                            break;
                        case Device.DeviceStates.WaitHighVoltage:
                            hBlock = true;
                            setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                            break;
                    }
                    OnLog(pState.ToString());
                    onTheFly = false;
                    return;
                }
                if (command is updateCounts) {
                    if (CurrentMeasureMode == null) {
                        // fake reply caught here (in order to put device into proper state)
                        hBlock = false;
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                        return;
                    }
                    if (!CurrentMeasureMode.onUpdateCounts()) {
                        OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                    }
                    return;
                }
                if (command is confirmSVoltage) {
                    if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                        CurrentMeasureMode.NextMeasure((t1, t2) => { toSend.Enqueue(new sendMeasure(t1, t2)); });
                    }
                }
                if (command is confirmF2Voltage) {
                    if (pState == ProgramStates.Measure ||
                        pState == ProgramStates.WaitBackgroundMeasure) {
                        if (!CurrentMeasureMode.Start()) {
                            OnErrorOccured("Нет точек для измерения.");
                        }
                    }
                    return;
                }
                return;
            }
        } 

        void CheckInterfaces(ServicePacket<CommandCode> Command) {
            // TODO: make common auto-action
            if (Command is IAutomatedReply) {
                // BAD!
                toSend.Enqueue(((IAutomatedReply)Command).AutomatedReply() as UserRequest<CommandCode>);
            }
            if (Command is IUpdateDevice) {
                ((IUpdateDevice)Command).UpdateDevice();
                hBlock = !Device.highVoltageOn;
            }
            if (Command is IUpdateGraph) {
                if (CurrentMeasureMode == null) {
                    //error
                    return;
                }
                CurrentMeasureMode.UpdateGraph();
            }
        }

        public override void Init() {
            OnLog(pState.ToString());

            setProgramState(ProgramStates.WaitInit);
            toSend.Enqueue(new sendInit());

            OnLog(pState.ToString());
        }
        public override void Shutdown() {
            Disable();
            toSend.Enqueue(new sendShutdown());
            setProgramState(ProgramStates.WaitShutdown);
            // TODO: добавить контрольное время ожидания выключения
        }

        public override void Scan() {
            if (pState == ProgramStates.Ready) {
                Graph.Instance.Reset();
                CurrentMeasureMode = new MeasureMode.Scan();
                CurrentMeasureMode.SuccessfulExit += (s, e) => Config.autoSaveSpectrumFile();
                CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringScanMeasure(p, Device.Detector1, Device.Detector2);
                initMeasure(ProgramStates.Measure);
            }
        }
        public override bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Instance.Reset();
                    {
                        var temp = new MeasureMode.Precise();
                        temp.SaveResults += (s, e) => Config.autoSavePreciseSpectrumFile(e.Shift);
                        CurrentMeasureMode = temp;
                    }
                    CurrentMeasureMode.SuccessfulExit += (s, e) => {
                        var ee = e as MeasureMode.Precise.SuccessfulExitEventArgs;
                        Graph.Instance.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                    };
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak);
                    initMeasure(ProgramStates.Measure);
                    return true;
                } else {
                    OnLog("No points for precise mode measure.");
                    return false;
                }
            }
            return false;
        }

        // TODO: use simple arrays
        FixedSizeQueue<List<long>> background;
        Matrix matrix;
        List<long> backgroundResult;
        bool doBackgroundPremeasure;
        public override bool? Monitor() {
            byte backgroundCycles = Config.BackgroundCycles;
            doBackgroundPremeasure = Config.BackgroundCycles != 0;
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    //Order is important here!!!! Underlying data update before both matrix formation and measure mode init.
                    Graph.Instance.ResetForMonitor();

                    #warning matrix is formed too early
                    // TODO: move matrix formation to manual operator actions
                    // TODO: parallelize matrix formation, flag on completion
                    // TODO: duplicates
                    var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
                    if (peaksForMatrix.Count > 0) {
                        // To comply with other processing order (and saved information)
                        peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
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
                    var temp = new MeasureMode.Precise.Monitor(Config.CheckerPeak == null ? null : startShiftValue, Config.AllowedShift, Config.TimeLimit);
                    temp.SaveResults += (s, e) => Config.autoSaveMonitorSpectrumFile(e.Shift);
                    CurrentMeasureMode = temp;
                    CurrentMeasureMode.Finalize += (s, e) => Config.finalizeMonitorFile();
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak);
                    
                    if (doBackgroundPremeasure) {
                        initMeasure(ProgramStates.WaitBackgroundMeasure);
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe Enumerator realization: one item, always recounting (accumulate values)..
                        Graph.Instance.NewGraphData += NewBackgroundMeasureReady;
                    } else {
                        initMeasure(ProgramStates.Measure);
                        Graph.Instance.NewGraphData += NewMonitorMeasureReady;
                    }
                    return true;
                } else {
                    OnLog("No points for monitor mode measure.");
                    return null;
                }
            } else if (pState == ProgramStates.BackgroundMeasureReady) {
                Graph.Instance.NewGraphData -= NewBackgroundMeasureReady;

                backgroundResult = background.Aggregate(Summarize);
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    // TODO: check integral operation behaviour here
                    backgroundResult[i] /= backgroundCycles;
                }

                setProgramStateWithoutUndo(ProgramStates.Measure);
                Graph.Instance.NewGraphData += NewMonitorMeasureReady;
                return false;
            } else {
                // wrong state, strange!
                return null;
            }
        }
        void NewBackgroundMeasureReady(int[] recreate) {
            // TODO: more accurately
            if (recreate.Length == Graph.Instance.Collectors.Count) {
                List<long> currentMeasure = new List<long>();
                // ! temporary solution
                var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
                if (peaksForMatrix.Count > 0) {
                    // To comply with other processing order (and saved information)
                    peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                    foreach (Utility.PreciseEditorData ped in peaksForMatrix) {
                        //!!!!! null PLSreference! race condition?
                        currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
                    }
                }
                //maybe null if background premeasure is false!
                background.Enqueue(currentMeasure);
                if (pState == ProgramStates.WaitBackgroundMeasure && background.IsFull) {
                    setProgramStateWithoutUndo(ProgramStates.BackgroundMeasureReady);
                }
            }
        }
        void NewMonitorMeasureReady(int[] recreate) {
            if (recreate.Length == 0)
                return;
            List<long> currentMeasure = new List<long>();
            // ! temporary solution
            var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
            if (peaksForMatrix.Count > 0) {
                // To comply with other processing order (and saved information)
                peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                foreach (Utility.PreciseEditorData ped in peaksForMatrix) {
                    currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
                }
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
        static List<long> Summarize(List<long> workingValue, List<long> nextElem) {
            // TODO: move from Commander to Utility
            if (workingValue.Count != nextElem.Count)
                // data length mismatch
                return null;
            for (int i = 0; i < workingValue.Count; ++i) {
                workingValue[i] += nextElem[i];
            }
            return workingValue;
        }

        void Disable() {
            MeasureCancelRequested = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            CurrentMeasureMode = null;//?
        }
        public override void SendSettings() {
            toSend.Enqueue(new sendIVoltage());
            // All sequence is:
            /*
            Commander.AddToSend(new sendCP());
            Commander.AddToSend(new enableECurrent());
            Commander.AddToSend(new enableHCurrent());
            Commander.AddToSend(new sendECurrent());
            Commander.AddToSend(new sendHCurrent());
            Commander.AddToSend(new sendF1Voltage());
            Commander.AddToSend(new sendF2Voltage());
            */
        }
        void initMeasure(ProgramStates state) {
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            
            setProgramState(state);

            MeasureCancelRequested = false;
            SendSettings();
        }

        void CurrentMeasureMode_Disable(object sender, EventArgs e) {
            if (CurrentMeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.Instance.NewGraphData -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.Instance.NewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            CurrentMeasureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable -= CurrentMeasureMode_Disable;

            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
            Disable();
        }
        void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            toSend.Enqueue(new sendSVoltage(e.Step));
        }
        public override bool SomePointsUsed {
            get {
                if (Config.PreciseData.Count > 0)
                    foreach (Utility.PreciseEditorData ped in Config.PreciseData)
                        if (ped.Use) return true;
                return false;
            }
        }

        public override void Unblock() {
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                MeasureCancelRequested = true;
            toSend.Enqueue(new enableHighVoltage(hBlock));
        }

        PortLevel port = new PortLevel();
        IProtocol<CommandCode> protocol;
        public Commander2() {
            protocol = new ModBus(port);
            ConsoleWriter.Subscribe(protocol);
            port.ErrorPort += (s, e) => {
                // TODO: more accurate
                OnErrorOccured(e.Message);
            };
            protocol.ErrorCommand += (s, e) => {
                // TODO: more accurate
                OnLog(e.Message);
            };
            notRareModeRequested = true;
            // TODO: move this hard-coded defaults to Config
            toSend = new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                new StatusRequestGenerator<CommandCode>(new requestStatus(), new getTurboPumpStatus(), () => notRare() ? 5 : 3),
                () => notRare() ? 500 : 10000);
            //RareModeChanged += t => {
            //    if (pState == ProgramStates.Measure || pState == ProgramStates.BackgroundMeasureReady || pState == ProgramStates.WaitBackgroundMeasure) {
            //        toSend.Stop();
            //        toSend.Start();
            //    }
            //};
            ProgramStateChanged += s => {
                if (s == ProgramStates.Measure || s == ProgramStates.BackgroundMeasureReady || s == ProgramStates.WaitBackgroundMeasure) {
                    toSend.Stop();
                    toSend.Start();
                }
            };
            MeasureCancelled += s => {
                toSend.Stop();
                toSend.Start();
            };
        }
        bool notRare() {
            if (pState == ProgramStates.Measure || pState == ProgramStates.BackgroundMeasureReady || pState == ProgramStates.WaitBackgroundMeasure)
                return notRareModeRequested;
            return true;
        }
        class StatusRequestGenerator<T>: IStatusRequestGenerator<T>
            where T: struct, IConvertible, IComparable {
            int i = 0;
            int f;
            readonly UserRequest<T> statusCheck, vacuumCheck;
            readonly Generator<int> factor;
            public UserRequest<T> Next {
                get {
                    UserRequest<T> res;
                    if (i == 0)
                        res = vacuumCheck;
                    else
                        res = statusCheck;
                    ++i;
                    i %= f;
                    return res;
                }
            }
            public void Reset() {
                f = factor();
            }
            public StatusRequestGenerator(UserRequest<T> statusCheck, UserRequest<T> vacuumCheck, Generator<int> factor) {
                this.factor = factor;
                this.statusCheck = statusCheck;
                this.vacuumCheck = vacuumCheck;
                Reset();
            }
        }
        public override void Connect() {
            PortLevel.PortStates res = port.Open();
            switch (res) {
                case PortLevel.PortStates.Opening:
                    toSend.Clear();
                    ConsoleWriter.Subscribe(toSend);
                    toSend.Undo += (s, e) => setProgramStateWithoutUndo(pStatePrev);
                    toSend.Start();
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
        }
        public override void Disconnect() {
            toSend.Stop();
            toSend.Undo -= (s, e) => setProgramStateWithoutUndo(pStatePrev);
            ConsoleWriter.Unsubscribe(toSend);
            toSend.Clear();
            PortLevel.PortStates res = port.Close();
            switch (res) {
                case PortLevel.PortStates.Closing:
                    DeviceIsConnected = false;
                    onTheFly = true;// надо ли здесь???
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

        public override void Reconnect() {
            if (DeviceIsConnected) {
                Disconnect();
                if (!DeviceIsConnected)
                    port.Open();
            }
        }
        public override string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }
    }
}
