using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Commands;
using Flavor.Common.Library;

namespace Flavor.Common {
    internal class Commander2: ICommander {
        private bool onTheFly = true;

        private ProgramStates programState = ProgramStates.Start;
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
                    //OnProgramStateChanged(value);
                };
            }
        }

        private void setProgramStateWithoutUndo(ProgramStates state) {
            pState = state;
            pStatePrev = pState;
        }
        private void setProgramState(ProgramStates state) {
            pStatePrev = pState;
            pState = state;
        }

        private bool handleBlock = true;
        internal override bool hBlock {
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

        private bool isConnected = false;
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

        private MessageQueueWithAutomatedStatusChecks toSend;

        private bool measureCancelRequested = false;
        public override bool MeasureCancelRequested {
            protected get { return measureCancelRequested; }
            set {
                measureCancelRequested = value;
                if (value && CurrentMeasureMode != null)
                    CurrentMeasureMode.CancelRequested = value;
            }
        }

        private void Realize(object sender, ModBusNew.CommandReceivedEventArgs e) {
            ServicePacket command = e.Command;
            //var code = e.Code;

            if (command is AsyncErrorReply) {
                CheckInterfaces(command);

                string message = string.Format("Device says: {0}", ((AsyncErrorReply)command).errorMessage);
                OnAsyncReplyReceived(message);
                // TODO: subscribe in Config for event
                Config.logCrash(message);

                if (pState != ProgramStates.Start) {
                    toSend.IsRareMode = false;
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    //Commander.hBlock = true;//!!!
                    MeasureCancelRequested = false;
                }
                return;
            }
            if (command is AsyncReply) {
                CheckInterfaces(command);
                if (command is AsyncReply.confirmShutdowned) {
                    OnLog("System is shutdowned");
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    hBlock = true;
                    OnLog(pState.ToString());
                    Device.Init();
                    return;
                }
                if (command is AsyncReply.SystemReseted) {
                    OnAsyncReplyReceived("Система переинициализировалась");
                    if (pState != ProgramStates.Start) {
                        toSend.IsRareMode = false;
                        setProgramStateWithoutUndo(ProgramStates.Start);
                        //Commander.hBlock = true;//!!!
                        MeasureCancelRequested = false;
                    }
                    return;
                }
                if (command is AsyncReply.confirmVacuumReady) {
                    if (hBlock) {
                        setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                    return;
                }
                if (command is AsyncReply.confirmHighVoltageOff) {
                    hBlock = true;
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
                    return;
                }
                if (command is AsyncReply.confirmHighVoltageOn) {
                    hBlock = false;
                    if (pState == ProgramStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                    toSend.AddToSend(new UserRequest.sendSVoltage(0));//Set ScanVoltage to low limit
                    toSend.AddToSend(new UserRequest.sendIVoltage());// и остальные напряжения затем
                    return;
                }
                return;
            }
            if (command is SyncErrorReply) {
                toSend.Dequeue();
                CheckInterfaces(command);
                return;
            }
            if (command is SyncReply) {
                if (null == toSend.Peek((SyncReply)command)) {
                    return;
                }
                CheckInterfaces(command);
                if (command is SyncReply.confirmInit) {
                    OnLog("Init request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Init);
                    OnLog(pState.ToString());
                    return;
                }
                if (command is SyncReply.confirmShutdown) {
                    OnLog("Shutdown request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                    OnLog(pState.ToString());
                    return;
                }
                // On The Fly part!!!
                if (onTheFly && (pState == ProgramStates.Start) && (command is SyncReply.updateStatus)) {
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
                            toSend.AddToSend(new UserRequest.getCounts());
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
                if (command is SyncReply.updateCounts) {
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
                if (command is SyncReply.confirmSVoltage) {
                    if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                        CurrentMeasureMode.NextMeasure((t1, t2) => { toSend.AddToSend(new UserRequest.sendMeasure(t1, t2)); });
                    }
                }
                if (command is SyncReply.confirmF2Voltage) {
                    if (pState == ProgramStates.Measure ||
                        pState == ProgramStates.WaitBackgroundMeasure) {
                        toSend.IsRareMode = !notRareModeRequested;
                        if (!CurrentMeasureMode.Start()) {
                            OnErrorOccured("Нет точек для измерения.");
                        }
                    }
                    return;
                }
                return;
            }
        } 

        private void CheckInterfaces(ServicePacket Command) {
            // TODO: make common auto-action
            if (Command is IAutomatedReply) {
                toSend.AddToSend(((IAutomatedReply)Command).AutomatedReply());
            }
            // OBSOLETE (ISend only in UI)
            /*if (Command is ISend) {
                toSend.AddToSend((ISend)Command);
            }*/
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
            toSend.AddToSend(new UserRequest.sendInit());

            OnLog(pState.ToString());
        }
        public override void Shutdown() {
            Disable();
            toSend.AddToSend(new UserRequest.sendShutdown());
            setProgramState(ProgramStates.WaitShutdown);
            // TODO: добавить контрольное время ожидания выключения
        }

        public override void Scan() {
            if (pState == ProgramStates.Ready) {
                Graph.Instance.Reset();
                CurrentMeasureMode = new MeasureMode.Scan();
                CurrentMeasureMode.SuccessfulExit += (s, e) => { Config.autoSaveSpectrumFile(); };
                CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => { Graph.Instance.updateGraphDuringScanMeasure(Device.Detector1, Device.Detector2, p); };
                initMeasure(ProgramStates.Measure);
            }
        }
        public override bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Instance.Reset();
                    {
                        var temp = new MeasureMode.Precise();
                        temp.SaveResults += (s, e) => { Config.autoSavePreciseSpectrumFile(e.Shift); };
                        CurrentMeasureMode = temp;
                    }
                    CurrentMeasureMode.SuccessfulExit += (s, e) => {
                        var ee = e as MeasureMode.Precise.SuccessfulExitEventArgs;
                        Graph.Instance.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                    };
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => { Graph.Instance.updateGraphDuringPreciseMeasure(p, peak); };
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
        private FixedSizeQueue<List<long>> background;
        private Matrix matrix;
        private List<long> backgroundResult;
        private bool doBackgroundPremeasure;
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
                    temp.SaveResults += (s, e) => { Config.autoSaveMonitorSpectrumFile(e.Shift); };
                    CurrentMeasureMode = temp;
                    CurrentMeasureMode.Finalize += (s, e) => { Config.finalizeMonitorFile(); };
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => { Graph.Instance.updateGraphDuringPreciseMeasure(p, peak); };
                    
                    if (doBackgroundPremeasure) {
                        initMeasure(ProgramStates.WaitBackgroundMeasure);
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe fake realization: one item, always recounting (accumulate values)..
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
        private void NewBackgroundMeasureReady(int[] recreate) {
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
        private void NewMonitorMeasureReady(int[] recreate) {
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
        private static List<long> Summarize(List<long> workingValue, List<long> nextElem) {
            // TODO: move from Commander to Utility
            if (workingValue.Count != nextElem.Count)
                // data length mismatch
                return null;
            for (int i = 0; i < workingValue.Count; ++i) {
                workingValue[i] += nextElem[i];
            }
            return workingValue;
        }

        private void Disable() {
            MeasureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            CurrentMeasureMode = null;//?
        }
        public override void SendSettings() {
            toSend.AddToSend(new UserRequest.sendIVoltage());
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
        private void initMeasure(ProgramStates state) {
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            
            setProgramState(state);

            toSend.IsRareMode = !notRareModeRequested;
            MeasureCancelRequested = false;
            SendSettings();
        }

        private void CurrentMeasureMode_Disable(object sender, EventArgs e) {
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
            
            Disable();
            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
        }
        private void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            toSend.AddToSend(new UserRequest.sendSVoltage(e.Step));
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
            toSend.AddToSend(new UserRequest.enableHighVoltage(hBlock));
        }

        private PortLevel port = new PortLevel();
        private ModBusNew protocol;
        public Commander2() {
            protocol = new ModBusNew(port);
            ConsoleWriter.Subscribe(protocol);
            port.ErrorPort += (s, e) => {
                // TODO: more accurate
                OnErrorOccured(e.Message);
            };
            protocol.ErrorCommand += (s, e) => {
                // TODO: more accurate
                OnLog(e.Message);
            };
        }
        public override PortLevel.PortStates Connect() {
            PortLevel.PortStates res = port.Open();
            switch (res) {
                case PortLevel.PortStates.Opening:
                    toSend = new MessageQueueWithAutomatedStatusChecks(protocol);
                    ConsoleWriter.Subscribe(toSend);
                    toSend.IsOperating = true;
                    toSend.Undo += (s, e) => setProgramStateWithoutUndo(pStatePrev);
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
        public override PortLevel.PortStates Disconnect() {
            toSend.IsOperating = false;
            ConsoleWriter.Unsubscribe(toSend);
            toSend.Undo -= (s, e) => setProgramStateWithoutUndo(pStatePrev);
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
            return res;
        }

        public override void Reconnect() {
            if (DeviceIsConnected) {
                switch (Disconnect()) {
                    case PortLevel.PortStates.Closing:
                        port.Open();
                        break;
                    case PortLevel.PortStates.Closed:
                        break;
                    case PortLevel.PortStates.ErrorClosing:
                        break;
                    default:
                        // фигня
                        break;
                }
            }
        }
        public override string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }
    }
}
