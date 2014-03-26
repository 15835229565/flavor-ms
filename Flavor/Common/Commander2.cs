using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Commands;
using Flavor.Common.Library;

namespace Flavor.Common {
    internal class Commander2: ICommander, IMessagingActions {
        private bool onTheFly = true;

        internal MeasureMode MeasureMode { get; private set; }

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

        // TODO: remove two remaining references to this method and make it private
        internal void setProgramStateWithoutUndo(ProgramStates state) {
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
        void IMessagingActions.AddToSend(UserRequest command) {
            toSend.AddToSend(command);
        }

        private void Realize(object sender, ModBusNew.CommandReceivedEventArgs e) {
            ServicePacket command = e.Command;
            //var code = e.Code;

            if (command is AsyncErrorReply) {
                CheckInterfaces(command);
                // 2 events..
                OnAsyncReplyReceived(string.Format("Device says: {0}", ((AsyncErrorReply)command).errorMessage));
                if (pState != ProgramStates.Start) {
                    toSend.IsRareMode = false;
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    //Commander.hBlock = true;//!!!
                    measureCancelRequested = false;
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
                        measureCancelRequested = false;
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
                    if (MeasureMode == null) {
                        // fake reply caught here (in order to put device into proper state)
                        hBlock = false;
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                        return;
                    }
                    if (!MeasureMode.onUpdateCounts()) {
                        OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                    }
                    return;
                }
                if (command is SyncReply.confirmF2Voltage) {
                    if (pState == ProgramStates.Measure ||
                        pState == ProgramStates.WaitBackgroundMeasure) {
                        toSend.IsRareMode = !notRareModeRequested;
                        if (!MeasureMode.start()) {
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
                ((IAutomatedReply)Command).AutomatedReply();
            }
            if (Command is ISend) {
                ((ISend)Command).Send();
            }
            if (Command is IUpdateDevice) {
                ((IUpdateDevice)Command).UpdateDevice();
            }
            if (Command is IUpdateGraph) {
                //((IUpdateGraph)Command).UpdateGraph();
                if (MeasureMode == null) {
                    //error
                    return;
                }
                MeasureMode.updateGraph();
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
                Graph.Reset();
                MeasureMode = new MeasureMode.Scan(Config.autoSaveSpectrumFile);
                initMeasure(ProgramStates.Measure);
            }
        }
        public override bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Reset();
                    MeasureMode = new MeasureMode.Precise();
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
                    Graph.ResetForMonitor();

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
                    MeasureMode = new MeasureMode.Precise.Monitor(Config.CheckerPeak == null ? null : startShiftValue, Config.AllowedShift, Config.TimeLimit);
                    
                    if (doBackgroundPremeasure) {
                        initMeasure(ProgramStates.WaitBackgroundMeasure);
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe fake realization: one item, always recounting (accumulate values)..
                        Graph.Instance.OnNewGraphData += NewBackgroundMeasureReady;
                    } else {
                        initMeasure(ProgramStates.Measure);
                        Graph.Instance.OnNewGraphData += NewMonitorMeasureReady;
                    }
                    return true;
                } else {
                    OnLog("No points for monitor mode measure.");
                    return null;
                }
            } else if (pState == ProgramStates.BackgroundMeasureReady) {
                Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;

                backgroundResult = background.Aggregate(Summarize);
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    // TODO: check integral operation behaviour here
                    backgroundResult[i] /= backgroundCycles;
                }

                setProgramStateWithoutUndo(ProgramStates.Measure);
                Graph.Instance.OnNewGraphData += NewMonitorMeasureReady;
                return false;
            } else {
                // wrong state, strange!
                return null;
            }
        }
        private void NewBackgroundMeasureReady(Graph.Recreate recreate) {
            if (recreate == Graph.Recreate.Both) {
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
        private void NewMonitorMeasureReady(Graph.Recreate recreate) {
            if (recreate == Graph.Recreate.None)
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

        void IMessagingActions.DisableMeasure() {
            if (MeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.Instance.OnNewGraphData -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            Disable();
            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
        }
        private void Disable() {
            measureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            MeasureMode = null;//?
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
            if (MeasureMode != null && MeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            setProgramState(state);

            toSend.IsRareMode = !notRareModeRequested;
            measureCancelRequested = false;
            SendSettings();
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
                measureCancelRequested = true;
            toSend.AddToSend(new UserRequest.enableHighVoltage(hBlock));
        }

        private PortLevel port = new PortLevel();
        private ModBusNew protocol;
        public Commander2() {
            protocol = new ModBusNew(port);
        }
        public override PortLevel.PortStates Connect() {
            PortLevel.PortStates res = port.Open();
            switch (res) {
                case PortLevel.PortStates.Opening:
                    toSend = new MessageQueueWithAutomatedStatusChecks();
                    toSend.IsOperating = true;
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
