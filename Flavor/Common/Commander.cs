using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Commands;
using Flavor.Common.Library;

namespace Flavor.Common {
    internal static class Commander {
        internal static event ProgramEventHandler ProgramStateChanged;
        internal static event ProgramEventHandler MeasureCancelled;
        private static void OnMeasureCancelled() {
            // TODO: lock here?
            if (MeasureCancelled != null)
                MeasureCancelled();
        }
        internal static event MessageHandler ErrorOccured;
        internal static event MessageHandler AsyncReplyReceived;

        private static ProgramStates programState = ProgramStates.Start;
        private static bool handleBlock = true;
        private static bool isConnected = false;
        private static bool onTheFly = true;

        private static MeasureMode measureMode = null;
        internal static MeasureMode CurrentMeasureMode {
            get { return measureMode; }
        }

        //private
        internal static ProgramStates pState {
            get {
                return programState;
            }
            private set {
                if (programState != value) {
                    programState = value;
                    if (value == ProgramStates.Start)
                        Disable();
                    ProgramStateChanged();
                    //OnProgramStateChanged(value);
                };
            }
        }

        internal static ProgramStates pStatePrev { get; private set; }
        // TODO: remove two remaining references to this method and make it private
        internal static void setProgramStateWithoutUndo(ProgramStates state) {
            pState = state;
            pStatePrev = pState;
        }
        private static void setProgramState(ProgramStates state) {
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
                    ProgramStateChanged();
                };
            }
        }

        internal static bool measureCancelRequested { get; set; }

        internal static bool notRareModeRequested { get; set; }

        internal static bool DeviceIsConnected {
            get {
                return isConnected;
            }
            private set {
                if (isConnected != value) {
                    isConnected = value;
                    ProgramStateChanged();
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
                AsyncReplyReceived(((AsyncErrorReply)Command).errorMessage);
                if (pState != ProgramStates.Start) {
                    toSend.IsRareMode = false;
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    //hBlock = true;//!!!
                    measureCancelRequested = false;
                }
                return;
            }
            if (Command is AsyncReply) {
                CheckInterfaces(Command);
                if (Command is AsyncReply.confirmShutdowned) {
                    ConsoleWriter.WriteLine("System is shutdowned");
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    hBlock = true;
                    ConsoleWriter.WriteLine(pState);
                    Device.Init();
                    return;
                }
                if (Command is AsyncReply.SystemReseted) {
                    ConsoleWriter.WriteLine("System reseted");
                    AsyncReplyReceived("Система переинициализировалась");
                    if (pState != ProgramStates.Start) {
                        toSend.IsRareMode = false;
                        setProgramStateWithoutUndo(ProgramStates.Start);
                        //hBlock = true;//!!!
                        measureCancelRequested = false;
                    }
                    return;
                }
                if (Command is AsyncReply.confirmVacuumReady) {
                    if (hBlock) {
                        setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                    return;
                }
                if (Command is AsyncReply.confirmHighVoltageOff) {
                    hBlock = true;
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
                    return;
                }
                if (Command is AsyncReply.confirmHighVoltageOn) {
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
                    setProgramStateWithoutUndo(ProgramStates.Init);
                    ConsoleWriter.WriteLine(pState);
                    return;
                }
                if (Command is SyncReply.confirmShutdown) {
                    ConsoleWriter.WriteLine("Shutdown request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                    ConsoleWriter.WriteLine(pState);
                    return;
                }
                // On The Fly part!!!
                if (onTheFly && (pState == ProgramStates.Start) && (Command is SyncReply.updateStatus)) {
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
                    ConsoleWriter.WriteLine(pState);
                    onTheFly = false;
                    return;
                }
                if (Command is SyncReply.updateCounts) {
                    if (measureMode == null) {
                        // fake reply caught here (in order to put device into proper state)
                        hBlock = false;
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                        return;
                    }
                    if (!measureMode.onUpdateCounts()) {
                        // error (out of limits), raise event here and notify in UI
                        // TODO: lock here
                        if (ErrorOccured != null) {
                            ErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                        }
                    }
                    return;
                }
                if (Command is SyncReply.confirmSVoltage) {
                    if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating)
                        CurrentMeasureMode.NextMeasure((t1, t2) => { toSend.AddToSend(new UserRequest.sendMeasure(t1, t2)); });
                }
                if (Command is SyncReply.confirmF2Voltage) {
                    if (pState == ProgramStates.Measure ||
                        pState == ProgramStates.WaitBackgroundMeasure) {
                        toSend.IsRareMode = !notRareModeRequested;
                        if (!measureMode.Start()) {
                            // error (no points)
                            // TODO: lock here
                            if (ErrorOccured != null) {
                                ErrorOccured("Нет точек для измерения.");
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
                toSend.AddToSend(((IAutomatedReply)Command).AutomatedReply());
                //((IAutomatedReply)Command).AutomatedReply();
            }
            // OBSOLETE (ISend only in UI)
            /*if (Command is ISend) {
                //???
                //ModBus.Send(((ISend)Command).Data);
                toSend.AddToSend((ISend)Command);
            }*/
            if (Command is IUpdateDevice) {
                ((IUpdateDevice)Command).UpdateDevice();
            }
            if (Command is IUpdateGraph) {
                if (CurrentMeasureMode == null) {
                    //error
                    return;
                }
                CurrentMeasureMode.UpdateGraph();
            }
        }

        internal static void Init() {
            ConsoleWriter.WriteLine(pState);

            setProgramState(ProgramStates.WaitInit);
            toSend.AddToSend(new UserRequest.sendInit());

            ConsoleWriter.WriteLine(pState);
        }
        internal static void Shutdown() {
            Disable();
            toSend.AddToSend(new UserRequest.sendShutdown());
            setProgramState(ProgramStates.WaitShutdown);
            // TODO: добавить контрольное время ожидания выключения
        }

        internal static void Scan() {
            if (pState == ProgramStates.Ready) {
                Graph.Reset();
                measureMode = new MeasureMode.Scan();
                measureMode.SuccessfulExit += (s, e) => { Config.autoSaveSpectrumFile(); };
                measureMode.GraphUpdateDelegate = (p, peak) => { Graph.updateGraphDuringScanMeasure(Device.Detector1, Device.Detector2, p); };
                initMeasure(ProgramStates.Measure);
            }
        }
        internal static bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Reset();
                    {
                        var temp = new MeasureMode.Precise();
                        temp.SaveResults += (s, e) => { Config.autoSavePreciseSpectrumFile(e.Shift); };
                        measureMode = temp;
                    }
                    measureMode.SuccessfulExit += (s, e) => {
                        var ee = e as MeasureMode.Precise.SuccessfulExitEventArgs;
                        Graph.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                    };
                    measureMode.GraphUpdateDelegate = (p, peak) => { Graph.updateGraphDuringPreciseMeasure(p, peak); };
                    initMeasure(ProgramStates.Measure);
                    return true;
                } else {
                    ConsoleWriter.WriteLine("No points for precise mode measure.");
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
                            ConsoleWriter.WriteLine("Error in peak data format or duplicate substance.");
                            return null;
                        }
                    } else
                        matrix = null;

                    // TODO: feed measure mode with start shift value (really?)
                    short? startShiftValue = 0;
                    var temp = new MeasureMode.Precise.Monitor(Config.CheckerPeak == null ? null : startShiftValue, Config.AllowedShift, Config.TimeLimit);
                    temp.SaveResults += (s, e) => { Config.autoSaveMonitorSpectrumFile(e.Shift); };
                    measureMode = temp;
                    measureMode.Finalize += (s, e) => { Config.finalizeMonitorFile(); };
                    measureMode.GraphUpdateDelegate = (p, peak) => { Graph.updateGraphDuringPreciseMeasure(p, peak); };

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
                    ConsoleWriter.WriteLine("No points for monitor mode measure.");
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
                if (pState == ProgramStates.WaitBackgroundMeasure && background.IsFull) {
                    setProgramStateWithoutUndo(ProgramStates.BackgroundMeasureReady);
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

        private static void Disable() {
            measureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();// also on normal exit and fault on start!
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
        private static void initMeasure(ProgramStates state) {
            ConsoleWriter.WriteLine(pState);
            if (measureMode != null && measureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            measureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            measureMode.Disable += measureMode_Disable;

            setProgramState(state);

            toSend.IsRareMode = !notRareModeRequested;
            measureCancelRequested = false;
            sendSettings();
        }

        private static void measureMode_Disable(object sender, EventArgs e) {
            if (measureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.Instance.OnNewGraphData -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.Instance.OnNewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            measureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            measureMode.Disable -= measureMode_Disable;

            Disable();
            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
        }
        private static void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            toSend.AddToSend(new UserRequest.sendSVoltage(e.Step));
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
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                measureCancelRequested = true;
            toSend.AddToSend(new UserRequest.enableHighVoltage(hBlock));
        }

        internal static ModBus.PortStates Connect() {
            ModBus.PortStates res = ModBus.Open();
            switch (res) {
                case ModBus.PortStates.Opening:
                    toSend = new MessageQueueWithAutomatedStatusChecks();
                    toSend.IsOperating = true;
                    DeviceIsConnected = true;
                    break;
                case ModBus.PortStates.Opened:
                    DeviceIsConnected = true;
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
                    DeviceIsConnected = false;
                    onTheFly = true;// надо ли здесь???
                    break;
                case ModBus.PortStates.Closed:
                    DeviceIsConnected = false;
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
            if (DeviceIsConnected) {
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
