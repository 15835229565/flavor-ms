using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Commands;
using Flavor.Common.Messaging;
using Flavor.Common.Library;

namespace Flavor.Common {
    internal static class Commander {
        internal enum programStates: byte {
            Start,
            Shutdown,
            Init,
            WaitHighVoltage,
            Ready,
            WaitBackgroundMeasure,
            BackgroundMeasureReady,
            Measure,
            WaitInit,
            WaitShutdown
        }

        internal delegate void ProgramEventHandler();
        internal delegate void AsyncReplyHandler(string msg);
        internal delegate void ErrorHandler(string msg);

        internal static event ProgramEventHandler OnProgramStateChanged;
        internal static event ProgramEventHandler OnScanCancelled;
        internal static event ErrorHandler OnError;
        private static Commander.programStates programState = programStates.Start;
        private static Commander.programStates programStatePrev;
        private static bool handleBlock = true;
        private static bool cancelMeasure = false;
        private static bool notRareMode = false;
        private static bool isConnected = false;
        private static bool onTheFly = true;

        private static MeasureMode measureMode = null;
        internal static MeasureMode CurrentMeasureMode {
            get { return measureMode; }
        }

        internal static event AsyncReplyHandler OnAsyncReply;

        internal static Commander.programStates pState {
            get {
                return programState;
            }
            private set {
                if (programState != value) {
                    programState = value;
                    if (value == programStates.Start)
                        Disable();
                    OnProgramStateChanged();
                };
            }
        }

        internal static Commander.programStates pStatePrev {
            get { return programStatePrev; }
            private set { programStatePrev = value; }
        }
        // TODO: remove two remaining references to this method and make it private
        internal static void setProgramStateWithoutUndo(Commander.programStates state) {
            pState = state;
            pStatePrev = pState;
        }
        private static void setProgramState(Commander.programStates state) {
            pStatePrev = pState;
            pState = state;
        }

        internal static bool hBlock {
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

        internal static bool measureCancelRequested {
            get { return cancelMeasure; }
            set { cancelMeasure = value; }
        }

        internal static bool notRareModeRequested {
            get { return notRareMode; }
            set { notRareMode = value; }
        }

        internal static bool DeviceIsConnected {
            get {
                return isConnected;
            }
            private set {
                if (isConnected != value) {
                    isConnected = value;
                    OnProgramStateChanged();
                }
            }
        }

        private static MessageQueueWithAutomatedStatusChecks toSend;

        internal static void AddToSend(UserRequest command) {
            toSend.AddToSend(command);
        }

        internal static void Realize(ServicePacket Command) {
            if (Command is AsyncErrorReply) {
                CheckInterfaces(Command);
                ConsoleWriter.WriteLine("Device says: {0}", ((AsyncErrorReply)Command).errorMessage);
                Commander.OnAsyncReply(((AsyncErrorReply)Command).errorMessage);
                if (Commander.pState != Commander.programStates.Start) {
                    toSend.IsRareMode = false;
                    setProgramStateWithoutUndo(Commander.programStates.Start);
                    //Commander.hBlock = true;//!!!
                    Commander.measureCancelRequested = false;
                }
                return;
            }
            if (Command is AsyncReply) {
                CheckInterfaces(Command);
                if (Command is AsyncReply.confirmShutdowned) {
                    ConsoleWriter.WriteLine("System is shutdowned");
                    setProgramStateWithoutUndo(Commander.programStates.Start);
                    Commander.hBlock = true;
                    ConsoleWriter.WriteLine(Commander.pState);
                    Device.Init();
                    return;
                }
                if (Command is AsyncReply.SystemReseted) {
                    ConsoleWriter.WriteLine("System reseted");
                    Commander.OnAsyncReply("Система переинициализировалась");
                    if (Commander.pState != Commander.programStates.Start) {
                        toSend.IsRareMode = false;
                        setProgramStateWithoutUndo(Commander.programStates.Start);
                        //Commander.hBlock = true;//!!!
                        Commander.measureCancelRequested = false;
                    }
                    return;
                }
                if (Command is AsyncReply.confirmVacuumReady) {
                    if (Commander.hBlock) {
                        setProgramStateWithoutUndo(Commander.programStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(Commander.programStates.Ready);
                    }
                    return;
                }
                if (Command is AsyncReply.confirmHighVoltageOff) {
                    Commander.hBlock = true;
                    setProgramStateWithoutUndo(Commander.programStates.WaitHighVoltage);//???
                    return;
                }
                if (Command is AsyncReply.confirmHighVoltageOn) {
                    Commander.hBlock = false;
                    if (Commander.pState == Commander.programStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(Commander.programStates.Ready);
                    }
                    toSend.AddToSend(new UserRequest.sendSVoltage(0));//Set ScanVoltage to low limit
                    toSend.AddToSend(new UserRequest.sendIVoltage());// и остальные напряжения затем
                    return;
                }
                return;
            }
            if (Command is SyncErrorReply) {
                toSend.Dequeue();
                CheckInterfaces(Command);
                return;
            }
            if (Command is SyncReply) {
                if (null == toSend.Peek((SyncReply)Command)) {
                    return;
                }
                CheckInterfaces(Command);
                if (Command is SyncReply.confirmInit) {
                    ConsoleWriter.WriteLine("Init request confirmed");
                    setProgramStateWithoutUndo(Commander.programStates.Init);
                    ConsoleWriter.WriteLine(Commander.pState);
                    return;
                }
                if (Command is SyncReply.confirmShutdown) {
                    ConsoleWriter.WriteLine("Shutdown request confirmed");
                    setProgramStateWithoutUndo(Commander.programStates.Shutdown);
                    ConsoleWriter.WriteLine(Commander.pState);
                    return;
                }
                if (onTheFly && (Commander.pState == Commander.programStates.Start) && (Command is SyncReply.updateStatus)) {
                    switch (Device.sysState) {
                        case (byte)Device.DeviceStates.Init:
                        case (byte)Device.DeviceStates.VacuumInit:
                            Commander.hBlock = true;
                            setProgramStateWithoutUndo(Commander.programStates.Init);
                            break;
                        
                        case (byte)Device.DeviceStates.ShutdownInit:
                        case (byte)Device.DeviceStates.Shutdowning:
                            Commander.hBlock = true;
                            setProgramStateWithoutUndo(Commander.programStates.Shutdown);
                            break;

                        case (byte)Device.DeviceStates.Measured:
                            toSend.AddToSend(new UserRequest.getCounts());
                            // waiting for fake counts reply
                            break;
                        case (byte)Device.DeviceStates.Measuring:
                            // async message here with auto send-back
                            // and waiting for fake counts reply
                            break;
                        
                        case (byte)Device.DeviceStates.Ready:
                            Commander.hBlock = false;
                            setProgramStateWithoutUndo(Commander.programStates.Ready);
                            break;
                        case (byte)Device.DeviceStates.WaitHighVoltage:
                            Commander.hBlock = true;
                            setProgramStateWithoutUndo(Commander.programStates.WaitHighVoltage);
                            break;
                    }
                    ConsoleWriter.WriteLine(Commander.pState);
                    onTheFly = false;
                    return;
                }
                if (Command is SyncReply.updateCounts) {
                    if (measureMode == null) {
                        // fake reply caught here (in order to put device into proper state)
                        Commander.hBlock = false;
                        setProgramStateWithoutUndo(Commander.programStates.Ready);
                        return;
                    }
                    if (!measureMode.onUpdateCounts()) {
                        // error (out of limits), raise event here and notify in UI
                        // TODO: lock here
                        if (OnError != null) {
                            OnError("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                        }
                    }
                    return;
                }
                if (Command is SyncReply.confirmF2Voltage) {
                    if (Commander.pState == programStates.Measure ||
                        Commander.pState == programStates.WaitBackgroundMeasure) {
                        toSend.IsRareMode = !notRareMode;
                        if (!measureMode.start()) {
                            // error (no points)
                            // TODO: lock here
                            if (OnError != null) {
                                OnError("Нет точек для измерения.");
                            }
                        }
                    }
                    return;
                }
                return;
            }
        }

        private static void CheckInterfaces(ServicePacket Command) {
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
                ((IUpdateGraph)Command).UpdateGraph();
            }
        }

        internal static void Init() {
            ConsoleWriter.WriteLine(pState);

            setProgramState(Commander.programStates.WaitInit);
            toSend.AddToSend(new UserRequest.sendInit());

            ConsoleWriter.WriteLine(pState);
        }
        internal static void Shutdown() {
            Disable();
            toSend.AddToSend(new UserRequest.sendShutdown());
            setProgramState(Commander.programStates.WaitShutdown);
            // TODO: добавить контрольное время ожидания выключения
        }

        internal static void Scan() {
            if (pState == Commander.programStates.Ready) {
                Graph.Reset();
                measureMode = new MeasureMode.Scan();
                initMeasure(Commander.programStates.Measure);
            }
        }
        internal static void Sense() {
            if (pState == Commander.programStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Reset();
                    measureMode = new MeasureMode.Precise();
                    initMeasure(Commander.programStates.Measure);
                } else {
                    ConsoleWriter.WriteLine("No points for precise mode measure.");
                }
            }
        }

        // TODO: use simple arrays
        private static FixedSizeQueue<List<long>> background;
        private static Matrix matrix;
        private static List<long> backgroundResult;
        internal static void Monitor() {
            // TODO: configurable capacity
            // Config.BackgroundCycles
            int backgroundCycles = 5;
            // TODO: configurable?
            // Config.DoBackgroundPremeasure
            bool doBackgroundPremeasure = true;
            
            if (pState == Commander.programStates.Ready) {
                // ! temporary solution
                List<Utility.PreciseEditorData> peds = Graph.Instance.PreciseData.getUsed().getWithId();
                if (peds.Count > 0/*SomePointsUsed*/) {
                    Graph.ResetForMonitor();
                    measureMode = new MeasureMode.Precise.Monitor(0, Config.AllowedShift, Config.TimeLimit);
                    initMeasure(Commander.programStates.WaitBackgroundMeasure);
                    // TODO: feed measure mode with start shift value (really?)

                    if (doBackgroundPremeasure) {
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe fake realization: one item, always recounting (accumulate values)..
                    }
                    // TODO: move matrix formation to manual operator actions
                    // TODO: parallelize matrix formation, flag on completion
                    // TODO: duplicates
                    matrix = new Matrix(Config.LoadLibrary(peds));
                    matrix.Init();

                    Graph.Instance.OnNewGraphData += NewBackgroundMeasureReady;
                } else {
                    ConsoleWriter.WriteLine("No points for monitor mode measure.");
                }
            } else if (pState == Commander.programStates.BackgroundMeasureReady) {
                Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;

                backgroundResult = background.Aggregate(Summarize);
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    backgroundResult[i] /= backgroundCycles;
                }

                setProgramStateWithoutUndo(programStates.Measure);
                Graph.Instance.OnNewGraphData += NewMonitorMeasureReady;
            } else {
                // wrong state, strange!
            }
        }
        private static void NewBackgroundMeasureReady(Graph.Recreate recreate) {
            if (recreate == Graph.Recreate.None)
                return;
            List<long> currentMeasure = new List<long>();
            // ! temporary solution
            foreach (Utility.PreciseEditorData ped in Graph.Instance.PreciseData.getUsed().getWithId()) {
                currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
            }
            //maybe null if background premeasure is false!
            background.Enqueue(currentMeasure);
            if (pState == programStates.WaitBackgroundMeasure && background.IsFull) {
                setProgramStateWithoutUndo(programStates.BackgroundMeasureReady);
            }
        }
        private static void NewMonitorMeasureReady(Graph.Recreate recreate) {
            if (recreate == Graph.Recreate.None)
                return;
            List<long> currentMeasure = new List<long>();
            // ! temporary solution
            foreach (Utility.PreciseEditorData ped in Graph.Instance.PreciseData.getUsed().getWithId()) {
                currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
            }
            if (currentMeasure.Count != backgroundResult.Count) { 
                // length mismatch
                // TODO: throw smth
            }
            // distract background
            /*for (int i = 0; i < backgroundResult.Count; ++i) {
                currentMeasure[i] -= backgroundResult[i];
            }*/
            // solve matrix equation
            double[] result = matrix.Solve(currentMeasure.ConvertAll<double>(x => (double)x));
            Config.AutoSaveSolvedSpectra(result);
            // TODO: put here all automatic logic from measure modes
        }
        private static List<Utility.PreciseEditorData> getWithId(this List<Utility.PreciseEditorData> peds) {
            // ! temporary solution
            return peds.FindAll(
                        x => x.Comment.StartsWith(Config.ID_PREFIX_TEMPORARY)
                    );
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

        internal static void DisableMeasure() {
            if (measureMode is MeasureMode.Precise.Monitor) {
                Graph.Instance.OnNewGraphData -= NewMonitorMeasureReady;
                matrix = null;
            }
            Disable();
            Commander.setProgramStateWithoutUndo(Commander.programStates.Ready);//really without undo?
        }
        private static void Disable() {
            Commander.measureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here
            if (OnScanCancelled != null) {
                OnScanCancelled();
            }
            measureMode = null;//?
        }
        internal static void sendSettings() {
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
        private static void initMeasure(Commander.programStates state) {
            ConsoleWriter.WriteLine(pState);
            if (measureMode != null && measureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            setProgramState(state);

            toSend.IsRareMode = !Commander.notRareModeRequested;
            Commander.measureCancelRequested = false;
            sendSettings();
        }
        internal static bool SomePointsUsed {
            get {
                if (Config.PreciseData.Count > 0)
                    foreach (Utility.PreciseEditorData ped in Config.PreciseData)
                        if (ped.Use) return true;
                return false;
            }
        }

        internal static void Unblock() {
            if (Commander.pState == programStates.Measure ||
                Commander.pState == programStates.WaitBackgroundMeasure ||
                Commander.pState == programStates.BackgroundMeasureReady)//strange..
                Commander.measureCancelRequested = true;
            toSend.AddToSend(new UserRequest.enableHighVoltage(Commander.hBlock));
        }

        internal static ModBus.PortStates Connect() {
            ModBus.PortStates res = ModBus.Open();
            switch (res) {
                case ModBus.PortStates.Opening:
                    toSend = new MessageQueueWithAutomatedStatusChecks();
                    toSend.IsOperating = true;
                    Commander.DeviceIsConnected = true;
                    break;
                case ModBus.PortStates.Opened:
                    Commander.DeviceIsConnected = true;
                    break;
                case ModBus.PortStates.ErrorOpening:
                    break;
                default:
                    // фигня
                    break;
            }
            return res;
        }
        internal static ModBus.PortStates Disconnect() {
            toSend.IsOperating = false;
            toSend.Clear();
            ModBus.PortStates res = ModBus.Close();
            switch (res) {
                case ModBus.PortStates.Closing:
                    Commander.DeviceIsConnected = false;
                    onTheFly = true;// надо ли здесь???
                    break;
                case ModBus.PortStates.Closed:
                    Commander.DeviceIsConnected = false;
                    break;
                case ModBus.PortStates.ErrorClosing:
                    break;
                default:
                    // фигня
                    break;
            }
            return res;
        }

        internal static void reconnect() {
            if (Commander.DeviceIsConnected) {
                switch (Disconnect()) {
                    case ModBus.PortStates.Closing:
                        ModBus.Open();
                        break;
                    case ModBus.PortStates.Closed:
                        break;
                    case ModBus.PortStates.ErrorClosing:
                        break;
                    default:
                        // фигня
                        break;
                }
            }
        }
        internal static string[] AvailablePorts {
            get { return ModBus.AvailablePorts; }
        }
    }
}
