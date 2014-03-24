using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Commands;
using Flavor.Common.Library;

namespace Flavor.Common {
    internal static class Commander2 {
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
        internal delegate void MessageHandler(string msg);

        internal static event ProgramEventHandler ProgramStateChanged;
        internal static event ProgramEventHandler MeasureCancelled;
        private static void OnMeasureCancelled() {
            // TODO: lock here?
            if (MeasureCancelled != null)
                MeasureCancelled();
        }
        internal static event MessageHandler ErrorOccured;
        private static void OnErrorOccured(string msg) {
            // TODO: lock here?
            if (ErrorOccured != null)
                ErrorOccured(msg);
            OnLog(msg);
        }
        internal static event MessageHandler AsyncReplyReceived;
        private static void OnAsyncReplyReceived(string msg) {
            // TODO: lock here?
            if (AsyncReplyReceived != null)
                AsyncReplyReceived(msg);
            OnLog(msg);
        }
        internal static event MessageHandler Log;
        private static void OnLog(string msg) {
            // TODO: lock here?
            if (Log != null)
                Log(msg);
        }
        
        private static bool onTheFly = true;

        internal static MeasureMode MeasureMode { get; private set; }

        private static programStates programState = programStates.Start;
        internal static programStates pState {
            get {
                return programState;
            }
            private set {
                if (programState != value) {
                    programState = value;
                    if (value == programStates.Start)
                        Disable();
                    ProgramStateChanged();
                    //OnProgramStateChanged(value);
                };
            }
        }

        internal static programStates pStatePrev { get; private set; }
        // TODO: remove two remaining references to this method and make it private
        internal static void setProgramStateWithoutUndo(programStates state) {
            pState = state;
            pStatePrev = pState;
        }
        private static void setProgramState(programStates state) {
            pStatePrev = pState;
            pState = state;
        }

        private static bool handleBlock = true;
        internal static bool hBlock {
            get {
                return handleBlock;
            }
            set {
                if (handleBlock != value) {
                    handleBlock = value;
                    ProgramStateChanged();
                };
            }
        }

        internal static bool measureCancelRequested { get; set; }

        internal static bool notRareModeRequested { get; set; }

        private static bool isConnected = false;
        internal static bool DeviceIsConnected {
            get {
                return isConnected;
            }
            private set {
                if (isConnected != value) {
                    isConnected = value;
                    if (value)
                        protocol.CommandReceived += Realize;
                    else
                        protocol.CommandReceived -= Realize;
                    ProgramStateChanged();
                }
            }
        }

        private static MessageQueueWithAutomatedStatusChecks toSend;

        internal static void AddToSend(UserRequest command) {
            toSend.AddToSend(command);
        }

        private static void Realize(object sender, ModBusNew.CommandReceivedEventArgs e) {
            ServicePacket command = e.Command;
            //var code = e.Code;

            if (command is AsyncErrorReply) {
                CheckInterfaces(command);
                // 2 events..
                OnAsyncReplyReceived(string.Format("Device says: {0}", ((AsyncErrorReply)command).errorMessage));
                if (pState != programStates.Start) {
                    toSend.IsRareMode = false;
                    setProgramStateWithoutUndo(programStates.Start);
                    //Commander.hBlock = true;//!!!
                    measureCancelRequested = false;
                }
                return;
            }
            if (command is AsyncReply) {
                CheckInterfaces(command);
                if (command is AsyncReply.confirmShutdowned) {
                    OnLog("System is shutdowned");
                    setProgramStateWithoutUndo(programStates.Start);
                    hBlock = true;
                    OnLog(pState.ToString());
                    Device.Init();
                    return;
                }
                if (command is AsyncReply.SystemReseted) {
                    OnAsyncReplyReceived("Система переинициализировалась");
                    if (pState != programStates.Start) {
                        toSend.IsRareMode = false;
                        setProgramStateWithoutUndo(programStates.Start);
                        //Commander.hBlock = true;//!!!
                        measureCancelRequested = false;
                    }
                    return;
                }
                if (command is AsyncReply.confirmVacuumReady) {
                    if (hBlock) {
                        setProgramStateWithoutUndo(programStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(programStates.Ready);
                    }
                    return;
                }
                if (command is AsyncReply.confirmHighVoltageOff) {
                    hBlock = true;
                    setProgramStateWithoutUndo(programStates.WaitHighVoltage);//???
                    return;
                }
                if (command is AsyncReply.confirmHighVoltageOn) {
                    hBlock = false;
                    if (pState == programStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(programStates.Ready);
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
                    setProgramStateWithoutUndo(programStates.Init);
                    OnLog(pState.ToString());
                    return;
                }
                if (command is SyncReply.confirmShutdown) {
                    OnLog("Shutdown request confirmed");
                    setProgramStateWithoutUndo(programStates.Shutdown);
                    OnLog(pState.ToString());
                    return;
                }
                if (onTheFly && (pState == programStates.Start) && (command is SyncReply.updateStatus)) {
                    switch (Device.sysState) {
                        case Device.DeviceStates.Init:
                        case Device.DeviceStates.VacuumInit:
                            hBlock = true;
                            setProgramStateWithoutUndo(programStates.Init);
                            break;

                        case Device.DeviceStates.ShutdownInit:
                        case Device.DeviceStates.Shutdowning:
                            hBlock = true;
                            setProgramStateWithoutUndo(programStates.Shutdown);
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
                            setProgramStateWithoutUndo(programStates.Ready);
                            break;
                        case Device.DeviceStates.WaitHighVoltage:
                            hBlock = true;
                            setProgramStateWithoutUndo(programStates.WaitHighVoltage);
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
                        setProgramStateWithoutUndo(programStates.Ready);
                        return;
                    }
                    if (!MeasureMode.onUpdateCounts()) {
                        OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                    }
                    return;
                }
                if (command is SyncReply.confirmF2Voltage) {
                    if (pState == programStates.Measure ||
                        pState == programStates.WaitBackgroundMeasure) {
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
                //((IUpdateGraph)Command).UpdateGraph();
                if (MeasureMode == null) {
                    //error
                    return;
                }
                MeasureMode.updateGraph();
            }
        }

        internal static void Init() {
            OnLog(pState.ToString());

            setProgramState(programStates.WaitInit);
            toSend.AddToSend(new UserRequest.sendInit());

            OnLog(pState.ToString());
        }
        internal static void Shutdown() {
            Disable();
            toSend.AddToSend(new UserRequest.sendShutdown());
            setProgramState(programStates.WaitShutdown);
            // TODO: добавить контрольное время ожидания выключения
        }

        internal static void Scan() {
            if (pState == programStates.Ready) {
                Graph.Reset();
                MeasureMode = new MeasureMode.Scan(Config.autoSaveSpectrumFile);
                initMeasure(programStates.Measure);
            }
        }
        internal static bool Sense() {
            if (pState == programStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Reset();
                    MeasureMode = new MeasureMode.Precise();
                    initMeasure(programStates.Measure);
                    return true;
                } else {
                    OnLog("No points for precise mode measure.");
                    return false;
                }
            }
            return false;
        }

        // TODO: use simple arrays
        private static FixedSizeQueue<List<long>> background;
        private static Matrix matrix;
        private static List<long> backgroundResult;
        private static bool doBackgroundPremeasure;
        internal static bool? Monitor() {
            byte backgroundCycles = Config.BackgroundCycles;
            doBackgroundPremeasure = Config.BackgroundCycles != 0;
            if (pState == programStates.Ready) {
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
                        initMeasure(programStates.WaitBackgroundMeasure);
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe fake realization: one item, always recounting (accumulate values)..
                        Graph.Instance.OnNewGraphData += NewBackgroundMeasureReady;
                    } else {
                        initMeasure(programStates.Measure);
                        Graph.Instance.OnNewGraphData += NewMonitorMeasureReady;
                    }
                    return true;
                } else {
                    OnLog("No points for monitor mode measure.");
                    return null;
                }
            } else if (pState == programStates.BackgroundMeasureReady) {
                Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;

                backgroundResult = background.Aggregate(Summarize);
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    // TODO: check integral operation behaviour here
                    backgroundResult[i] /= backgroundCycles;
                }

                setProgramStateWithoutUndo(programStates.Measure);
                Graph.Instance.OnNewGraphData += NewMonitorMeasureReady;
                return false;
            } else {
                // wrong state, strange!
                return null;
            }
        }
        private static void NewBackgroundMeasureReady(Graph.Recreate recreate) {
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
                if (pState == programStates.WaitBackgroundMeasure && background.IsFull) {
                    setProgramStateWithoutUndo(programStates.BackgroundMeasureReady);
                }
            }
        }
        private static void NewMonitorMeasureReady(Graph.Recreate recreate) {
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
        private static List<Utility.PreciseEditorData> getWithId(this List<Utility.PreciseEditorData> peds) {
            // ! temporary solution
            #warning make this operation one time a cycle
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
            if (MeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == programStates.Measure) {
                    Graph.Instance.OnNewGraphData -= NewMonitorMeasureReady;
                } else if (pState == programStates.WaitBackgroundMeasure || pState == programStates.BackgroundMeasureReady) {
                    Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            Disable();
            Commander2.setProgramStateWithoutUndo(programStates.Ready);//really without undo?
        }
        private static void Disable() {
            measureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            MeasureMode = null;//?
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
        private static void initMeasure(programStates state) {
            OnLog(pState.ToString());
            if (MeasureMode != null && MeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            setProgramState(state);

            toSend.IsRareMode = !notRareModeRequested;
            measureCancelRequested = false;
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
            if (pState == programStates.Measure ||
                pState == programStates.WaitBackgroundMeasure ||
                pState == programStates.BackgroundMeasureReady)//strange..
                measureCancelRequested = true;
            toSend.AddToSend(new UserRequest.enableHighVoltage(hBlock));
        }

        private static PortLevel port = new PortLevel();
        private static ModBusNew protocol = new ModBusNew(port);
        internal static PortLevel.PortStates Connect() {
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
        internal static PortLevel.PortStates Disconnect() {
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

        internal static void Reconnect() {
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
        internal static string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }
    }
}
