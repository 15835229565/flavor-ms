using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Commands.UI;
using Flavor.Common.Commands.Async;
using Flavor.Common.Commands.Sync;
using Flavor.Common.Messaging;
using Flavor.Common.Commands.Interfaces;

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
                if (Command is confirmShutdowned) {
                    ConsoleWriter.WriteLine("System is shutdowned");
                    setProgramStateWithoutUndo(Commander.programStates.Start);
                    Commander.hBlock = true;
                    ConsoleWriter.WriteLine(Commander.pState);
                    Device.Init();
                    return;
                }
                if (Command is SystemReseted) {
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
                if (Command is confirmVacuumReady) {
                    if (Commander.hBlock) {
                        setProgramStateWithoutUndo(Commander.programStates.WaitHighVoltage);
                    } else {
                        setProgramStateWithoutUndo(Commander.programStates.Ready);
                    }
                    return;
                }
                if (Command is confirmHighVoltageOff) {
                    Commander.hBlock = true;
                    setProgramStateWithoutUndo(Commander.programStates.WaitHighVoltage);//???
                    return;
                }
                if (Command is confirmHighVoltageOn) {
                    Commander.hBlock = false;
                    if (Commander.pState == Commander.programStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(Commander.programStates.Ready);
                    }
                    toSend.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
                    toSend.AddToSend(new sendIVoltage());// и остальные напряжения затем
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
                if (Command is confirmInit) {
                    ConsoleWriter.WriteLine("Init request confirmed");
                    setProgramStateWithoutUndo(Commander.programStates.Init);
                    ConsoleWriter.WriteLine(Commander.pState);
                    return;
                }
                if (Command is confirmShutdown) {
                    ConsoleWriter.WriteLine("Shutdown request confirmed");
                    setProgramStateWithoutUndo(Commander.programStates.Shutdown);
                    ConsoleWriter.WriteLine(Commander.pState);
                    return;
                }
                if (onTheFly && (Commander.pState == Commander.programStates.Start) && (Command is updateStatus)) {
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
                            toSend.AddToSend(new getCounts());
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
                if (Command is updateCounts) {
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
                if (Command is confirmF2Voltage) {
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
            toSend.AddToSend(new sendInit());

            ConsoleWriter.WriteLine(pState);
        }
        internal static void Shutdown() {
            Disable();
            toSend.AddToSend(new sendShutdown());
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

        //background measurings. problem is that senseModeCounts are invisible here.
        //TODO: consider refactoring
        private static FixedSizeQueue<List<Utility.PreciseEditorData>> background;

        internal static void Monitor() {
            if (pState == Commander.programStates.Ready) {
                if (SomePointsUsed) {
                    Graph.ResetForMonitor();
                    measureMode = new MeasureMode.Precise.Monitor(0, Config.AllowedShift, Config.TimeLimit);
                    initMeasure(Commander.programStates.WaitBackgroundMeasure);
                    // TODO: feed measure mode with start shift value (really?)

                    // TODO: configurable? 
                    if (true) {
                        // TODO: configurable capacity
                        background = new FixedSizeQueue<List<Utility.PreciseEditorData>>(5);
                    }
                    // TODO: retrieve library data here. form matrix.
                    // TODO: start automatic feeding of fixed queue
                } else {
                    ConsoleWriter.WriteLine("No points for monitor(precise) mode measure.");
                }
                return;
            }
            if (pState == Commander.programStates.BackgroundMeasureReady) {
                // TODO: count average background value
                background.ToArray();
                // TODO: start automatic solving and saving of monitor data.
            }
        }
        internal static void Disable() {
            Commander.measureCancelRequested = false;
            toSend.IsRareMode = false;
            // TODO: lock here
            if (OnScanCancelled != null) {
                OnScanCancelled();
            }
            measureMode = null;//?
        }
        internal static void sendSettings() {
            toSend.AddToSend(new sendIVoltage());
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
            toSend.AddToSend(new enableHighVoltage(Commander.hBlock));
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
