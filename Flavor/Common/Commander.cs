using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Flavor.Common.Commands.UI;
using Flavor.Common.Commands.Async;
using Flavor.Common.Commands.Sync;
using Flavor.Common.Measuring;
using Flavor.Common.Messaging;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common
{
    internal static class Commander
    {
        internal enum programStates : byte
        {
            Start,
            Shutdown,
            Init,
            WaitHighVoltage,
            Ready,
            Measure,
            WaitInit,
            WaitShutdown
        }

        internal delegate void ProgramEventHandler();
        internal delegate void AsyncReplyHandler(string msg);

        internal static event ProgramEventHandler OnProgramStateChanged;
        internal static event ProgramEventHandler OnScanCancelled;
        private static Commander.programStates programState;
        private static Commander.programStates programStatePrev;
        private static bool handleBlock = true;
        private static bool cancelMeasure = false;
        private static bool notRareMode = false;
        private static bool isConnected = false;
        private static bool onTheFly = true;

        private static MeasureMode measureMode = null;
        internal static MeasureMode CurrentMeasureMode
        {
            get { return measureMode; }
        }

        internal static event AsyncReplyHandler OnAsyncReply;
        
        internal static Commander.programStates pState
        {
            get 
            { 
                return programState; 
            }
            set 
            {
                if (programState != value) 
                {
                    programState = value;
                    OnProgramStateChanged();
                };
            }
        }
        
        internal static Commander.programStates pStatePrev
        {
            get { return programStatePrev; }
            set { programStatePrev = value; }
        }
        
        internal static bool hBlock
        {
            get 
            {
                return handleBlock; 
            }
            set
            {
                if (handleBlock != value)
                {
                    handleBlock = value;
                    OnProgramStateChanged();
                };
            }
        }

        internal static bool measureCancelRequested
        {
            get { return cancelMeasure; }
            set { cancelMeasure = value; }
        }

        internal static bool notRareModeRequested
        {
            get { return notRareMode; }
            set { notRareMode = value; }
        }

        internal static bool deviceIsConnected
        {
            get 
            {
                return isConnected; 
            }
            set 
            {
                if (isConnected != value)
                { 
                    isConnected = value;
                    OnProgramStateChanged();
                }
            }
        }

        private static MessageQueueWithAutomatedStatusChecks toSend;

        internal static void AddToSend(UserRequest command)
        {
            toSend.AddToSend(command);
        }

        internal static void Realize(ServicePacket Command)
        {
            if (Command is AsyncErrorReply)
            {
                CheckInterfaces(Command);
                Console.WriteLine("Device says: {0}", ((AsyncErrorReply)Command).errorMessage);
                Commander.OnAsyncReply(((AsyncErrorReply)Command).errorMessage);
                if (Commander.pState != Commander.programStates.Start)
                {
                    toSend.IsRareMode = false;
                    Commander.pState = Commander.programStates.Start;
                    Commander.pStatePrev = Commander.pState;
                    //Commander.hBlock = true;//!!!
                    Commander.measureCancelRequested = false;
                }
            }
            if (Command is AsyncReply)
            {
                CheckInterfaces(Command);
                if (Command is confirmShutdowned)
                {
                    Console.WriteLine("System is shutdowned");
                    Commander.pState = Commander.programStates.Start;
                    Commander.pStatePrev = Commander.pState;
                    Commander.hBlock = true;
                    Console.WriteLine(Commander.pState);
                    Device.Init();
                }
                if (Command is SystemReseted)
                {
                    Console.WriteLine("System reseted");
                    Commander.OnAsyncReply("—истема переинициализировалась");
                    if (Commander.pState != Commander.programStates.Start)
                    {
                        toSend.IsRareMode = false;
                        Commander.pState = Commander.programStates.Start;
                        Commander.pStatePrev = Commander.pState;
                        //Commander.hBlock = true;//!!!
                        Commander.measureCancelRequested = false;
                    }
                }
                if (Command is confirmVacuumReady)
                {
                    if (Commander.hBlock)
                    {
                        Commander.pState = Commander.programStates.WaitHighVoltage;
                        Commander.pStatePrev = Commander.pState;
                    }
                    else
                    {
                        Commander.pState = Commander.programStates.Ready;
                        Commander.pStatePrev = Commander.pState;
                    }
                }
                if (Command is confirmHighVoltageOff)
                {
                    Commander.hBlock = true;
                    Commander.pState = Commander.programStates.WaitHighVoltage;//???
                    Commander.pStatePrev = Commander.pState;
                }
                if (Command is confirmHighVoltageOn)
                {
                    Commander.hBlock = false;
                    if (Commander.pState == Commander.programStates.WaitHighVoltage)
                    {
                        Commander.pState = Commander.programStates.Ready;
                        Commander.pStatePrev = Commander.pState;
                    }
                    toSend.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
                    toSend.AddToSend(new sendIVoltage());// и остальные напр€жени€ затем
                }
            }
            if (Command is SyncErrorReply)
            {
                toSend.Dequeue();
                CheckInterfaces(Command);
            }
            if (Command is SyncReply)
            {
                if (null == toSend.Peek((SyncReply)Command))
                {
                    return;
                }
                CheckInterfaces(Command);
                if (Command is confirmInit)
                {
                    Console.WriteLine("Init request confirmed");
                    Commander.pState = Commander.programStates.Init;
                    Commander.pStatePrev = Commander.pState;
                    Console.WriteLine(Commander.pState);
                }
                if (Command is confirmShutdown)
                {
                    Console.WriteLine("Shutdown request confirmed");
                    Commander.pState = Commander.programStates.Shutdown;
                    Commander.pStatePrev = Commander.pState;
                    Console.WriteLine(Commander.pState);
                }
                if (onTheFly && (Commander.pState == Commander.programStates.Start) && (Command is updateStatus))
                {
                    switch (Device.sysState)
                    {
                        case (byte)Device.DeviceStates.Ready:
                            Commander.hBlock = false;
                            Commander.pState = Commander.programStates.Ready;
                            Commander.pStatePrev = Commander.pState;
                            break;
                        case (byte)Device.DeviceStates.WaitHighVoltage:
                            Commander.hBlock = true;
                            Commander.pState = Commander.programStates.WaitHighVoltage;
                            Commander.pStatePrev = Commander.pState;
                            break;
                        //!!!!
                        case (byte)Device.DeviceStates.Measured:
                            Commander.hBlock = false;
                            Commander.pState = Commander.programStates.Ready;
                            Commander.pStatePrev = Commander.pState;
                            break;
                        case (byte)Device.DeviceStates.Measuring:
                            Commander.hBlock = false;
                            Commander.pState = Commander.programStates.Ready;
                            Commander.pStatePrev = Commander.pState;
                            break;
                    }
                    Console.WriteLine(Commander.pState);
                    onTheFly = false;
                }
                if (Command is updateCounts)
                {
                    if (measureMode == null)
                    {
                        // error here
                        throw new Exception("No measure mode in operation.");
                    }
                    measureMode.onUpdateCounts();
                }
                if (Command is confirmF2Voltage)
                {
                    if (Commander.pState == programStates.Measure)
                    {
                        toSend.IsRareMode = !notRareMode;
                        measureMode.start();
                    }
                }
            }
        }

        private static void CheckInterfaces(ServicePacket Command)
        {
            if (Command is IAutomatedReply)
            {
                ((IAutomatedReply)Command).AutomatedReply();
            }
            if (Command is ISend)
            {
                ((ISend)Command).Send();
            }
            if (Command is IUpdateDevice)
            {
                ((IUpdateDevice)Command).UpdateDevice();
            }
            if (Command is IUpdateGraph)
            {
                ((IUpdateGraph)Command).UpdateGraph();
            }
        }
        
        internal static void Init()
        {
            Console.WriteLine(pState);

            Commander.pStatePrev = Commander.pState;
            Commander.pState = Commander.programStates.WaitInit;
            toSend.AddToSend(new sendInit());

            Console.WriteLine(pState);
        }
        internal static void Shutdown()
        {

            toSend.AddToSend(new sendShutdown());
            Commander.pStatePrev = Commander.pState;
            Commander.pState = Commander.programStates.WaitShutdown;
            //добавить контрольное врем€ ожидани€ выключени€
        }
        
        internal static void Scan()
        {
            if (pState == Commander.programStates.Ready)
            {
                initMeasure();                          //?
                measureMode = new ScanMeasureMode();    //order
            }
        }
        internal static void Sense()
        {
            if (pState == Commander.programStates.Ready)
            {
                if (somePointsUsed())
                {
                    initMeasure();                          //?
                    measureMode = new PreciseMeasureMode(); //order
                }
                else
                {
                    Console.WriteLine("No points for precise mode measure.");
                }
            }
        }
        internal static void Monitor()
        {
            if (pState == Commander.programStates.Ready)
            {
                if (somePointsUsed())
                {
                    initMeasure();                          //?
                    measureMode = new MonitorMeasureMode(); //order
                }
                else
                {
                    Console.WriteLine("No points for monitor(precise) mode measure.");
                }
            }
        }
        internal static void cancelScan()
        {
            Commander.measureCancelRequested = false;
            toSend.IsRareMode = false;
            Commander.pStatePrev = Commander.pState;
            Commander.pState = Commander.programStates.Ready;
            Commander.pStatePrev = Commander.pState;
            OnScanCancelled();
            measureMode = null;//?
        }
        private static void initMeasure()
        {
            Console.WriteLine(pState);
            if (measureMode != null && measureMode.isOperating)
            {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            pStatePrev = pState;
            pState = Commander.programStates.Measure;

            Graph.ResetPointLists();
            toSend.IsRareMode = !Commander.notRareModeRequested;
            Commander.measureCancelRequested = false;
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
        internal static bool somePointsUsed()
        {
            if (Config.PreciseData.Count > 0)
                foreach (Utility.PreciseEditorData ped in Config.PreciseData)
                    if (ped.Use) return true;
            return false;
        }

        internal static void Unblock()
        {
            if (Commander.pState == programStates.Measure)//strange..
            {
                Commander.measureCancelRequested = true;
            }
            toSend.AddToSend(new enableHighVoltage(Commander.hBlock));
        }

        internal static void Connect()
        {
            switch (ModBus.Open())
            {
                case ModBus.PortStates.Opening:
                    Commander.deviceIsConnected = true;
                    toSend = new MessageQueueWithAutomatedStatusChecks();
                    toSend.IsOperating = true;
                    break;
                case ModBus.PortStates.Opened:
                    Commander.deviceIsConnected = true;
                    break;
                case ModBus.PortStates.ErrorOpening:
                    break;
                default:
                    // фигн€
                    break;
            }
        }
        internal static void Disconnect()
        {
            toSend.IsOperating = false;
            toSend.Clear();
            switch (ModBus.Close())
            {
                case ModBus.PortStates.Closing:
                    Commander.deviceIsConnected = false;
                    onTheFly = true;// надо ли здесь???
                    break;
                case ModBus.PortStates.Closed:
                    Commander.deviceIsConnected = false;
                    break;
                case ModBus.PortStates.ErrorClosing:
                    break;
                default:
                    // фигн€
                    break;
            }
        }
    }
}
