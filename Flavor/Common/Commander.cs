using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Flavor
{
    static class Commander
    {
        public enum programStates : byte
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

        public delegate void ProgramEventHandler();
        public delegate void AsyncReplyHandler(string msg);

        public static event ProgramEventHandler OnProgramStateChanged;
        public static event ProgramEventHandler OnScanCancelled;
        private static Commander.programStates programState;
        private static Commander.programStates programStatePrev;
        private static bool handleBlock = true;
        private static bool isSenseMeasureMode;
        private static bool cancelMeasure = false;
        private static bool scanning = false;
        private static bool notRareMode = false;
        private static bool isConnected = false;
        private static bool onTheFly = true;
        private static sendMeasure customMeasure = null;
        private static bool doMeasure = true;

        public static event AsyncReplyHandler OnAsyncReply;
        
        public static Commander.programStates pState
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
        
        public static Commander.programStates pStatePrev
        {
            get { return programStatePrev; }
            set { programStatePrev = value; }
        }
        
        private static ushort PointValue = 0;

        private static Utility.PreciseEditorData[] senseModePoints;

        private static int[][] senseModeCounts;
        
        private static byte senseModePeak = 0;

        public static Utility.PreciseEditorData SenseModePeak 
        {
            get { return senseModePoints[senseModePeak]; }
        }

        private static ushort[] senseModePeakIteration;

        private static ushort smpiSum;

        public static bool hBlock
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
        
        public static bool isSenseMeasure
        {
            get { return isSenseMeasureMode; }
            set { isSenseMeasureMode = value; }
        }

        public static bool measureCancelRequested
        {
            get { return cancelMeasure; }
            set { cancelMeasure = value; }
        }

        public static bool notRareModeRequested
        {
            get { return notRareMode; }
            set { notRareMode = value; }
        }

        public static bool deviceIsConnected
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

        public static sendMeasure CustomMeasure
        {
            get { return customMeasure; }
        }

        public static bool DoMeasure
        {
            get { return doMeasure; }
            set { doMeasure = value; }
        }

        public static ushort Point
        {
            get { return PointValue; }
            set { PointValue = value; }
        }
        
        private static MessageQueueWithAutomatedStatusChecks toSend;

        public static void AddToSend(UserRequest Command)
        {
            toSend.AddToSend(Command);
        }

        public static void Realize(ServicePacket Command)
        {
            if (Command is AsyncErrorReply)
            {
                CheckInterfaces(Command);
                Console.WriteLine("Device says: {0}", ((AsyncErrorReply)Command).errorMessage);
                Commander.OnAsyncReply(((AsyncErrorReply)Command).errorMessage);
                if (Commander.pState != Commander.programStates.Start)
                {
                    if (scanning & !Commander.notRareModeRequested) toSend.IsRareMode = false;
                    Commander.pState = Commander.programStates.Start;
                    Commander.pStatePrev = Commander.pState;
                    Commander.hBlock = true;//!!!
                    scanning = false;
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
                    scanning = false;
                    Device.Init();
                }
                if (Command is SystemReseted)
                {
                    Console.WriteLine("System reseted");
                    Commander.OnAsyncReply("—истема переинициализировалась");
                    if (Commander.pState != Commander.programStates.Start)
                    {
                        if (scanning & !Commander.notRareModeRequested) toSend.IsRareMode = false;
                        Commander.pState = Commander.programStates.Start;
                        Commander.pStatePrev = Commander.pState;
                        Commander.hBlock = true;//!!!
                        scanning = false;
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
                    toSend.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
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
                    customMeasure = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
                    if (!Commander.isSenseMeasure)
                    {
                        if (!Commander.measureCancelRequested && (Commander.Point <= Config.ePoint))
                        {
                            toSend.AddToSend(new sendSVoltage(Commander.Point++));
                        }
                        else
                        {
                            if (scanning)
                            {
                                if (!Commander.notRareModeRequested) toSend.IsRareMode = false;
                                scanning = false;
                                toSend.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
                                OnScanCancelled();
                                Commander.pStatePrev = Commander.pState;
                                Commander.pState = Commander.programStates.Ready;
                                Commander.pStatePrev = Commander.pState;
                                Commander.measureCancelRequested = false;
                                Config.AutoSaveSpecterFile();
                            }
                        }
                    }
                    else
                    {
                        if (!Commander.measureCancelRequested)
                        {
                            if (senseModePoints[senseModePeak].Collector == 1)
                                senseModeCounts[senseModePeak][(Commander.Point - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector1;
                            else
                                senseModeCounts[senseModePeak][(Commander.Point - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector2;
                            if ((Commander.Point <= (senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width)))
                            {
                                toSend.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                            }
                            else
                            {
                                --(senseModePeakIteration[senseModePeak]);
                                --smpiSum;
                                if (smpiSum > 0)
                                {
                                    for (int i = 0; i < senseModePoints.Length; ++i)//ѕоиск пика с оставшейс€ ненулевой итерацией. Ќо не более 1 цикла.
                                    {
                                        ++senseModePeak;
                                        if (senseModePeak >= senseModePoints.Length) senseModePeak = 0;
                                        if (senseModePeakIteration[senseModePeak] > 0) break;
                                    }
                                    ushort nextPoint = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                    if (Commander.Point > nextPoint)
                                    {
                                        //!!!case of backward voltage change
                                        customMeasure = new sendMeasure(Config.CommonOptions.bTime, Config.CommonOptions.eTime);
                                    }
                                    else
                                    {
                                        //!!!case of forward voltage change
                                        if (Config.CommonOptions.ForwardTimeEqualsBeforeTime)
                                        {
                                            customMeasure = new sendMeasure(Config.CommonOptions.befTime, Config.CommonOptions.eTime);
                                        }
                                        else
                                        {
                                            customMeasure = new sendMeasure(Config.CommonOptions.fTime, Config.CommonOptions.eTime);
                                        }
                                    }
                                    Commander.Point = nextPoint;
                                    toSend.AddToSend(new sendSVoltage(Commander.Point++));
                                    //old code:
                                    //Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                    //Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                                }
                                else
                                {
                                    if (!Commander.notRareModeRequested) toSend.IsRareMode = false;
                                    toSend.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
                                    Commander.pStatePrev = Commander.pState;
                                    Commander.pState = Commander.programStates.Ready;
                                    Commander.pStatePrev = Commander.pState;
                                    OnScanCancelled();//!!!
                                    Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints);
                                    Config.AutoSavePreciseSpecterFile();
                                }
                            }
                        }
                        else
                        {
                            OnScanCancelled();//!!!
                            Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints);
                            if (!Commander.notRareModeRequested) toSend.IsRareMode = false;
                            toSend.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
                            Commander.pStatePrev = Commander.pState;
                            Commander.pState = Commander.programStates.Ready;
                            Commander.pStatePrev = Commander.pState;
                            Commander.measureCancelRequested = false;
                        }
                    }
                }
                if (Command is confirmF2Voltage)
                {
                    if (Commander.pState == programStates.Measure)
                    {
                        //first measure point with increased idle time
                        customMeasure = new sendMeasure(Config.CommonOptions.befTime, Config.CommonOptions.eTime);
                        if (!Commander.isSenseMeasure)
                        {
                            Commander.Point = Config.sPoint;
                            toSend.AddToSend(new sendSVoltage(Commander.Point++));
                        }
                        else
                        {
                            if (Config.PreciseData.Count > 0)
                            {
                                //Sort in increased order
                                //Config.PreciseData.Sort(ComparePreciseEditorDataByPeakValue);
                                //Config.PreciseData.Sort(ComparePreciseEditorDataByUseFlagAndPeakValue);
                                //senseModePoints = Config.PreciseData.ToArray();
                                List<Utility.PreciseEditorData> temp = Config.PreciseData.FindAll(Utility.PeakIsUsed);
                                temp.Sort(Utility.ComparePreciseEditorDataByPeakValue);
                                senseModePoints = temp.ToArray();
                                senseModePeakIteration = new ushort[senseModePoints.Length];
                                smpiSum = 0;
                                senseModeCounts = new int[senseModePoints.Length][];
                                for (int i = 0; i < senseModePeakIteration.Length; ++i)
                                {
                                    senseModeCounts[i] = new int[2 * senseModePoints[i].Width + 1];
                                    senseModePeakIteration[i] = senseModePoints[i].Iterations;
                                    smpiSum += senseModePoints[i].Iterations; ;
                                }
                                senseModePeak = 0;
                                Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                toSend.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                            }
                            else
                            {
                                if (!Commander.notRareModeRequested) toSend.IsRareMode = false;
                                Commander.pStatePrev = Commander.pState;
                                Commander.pState = Commander.programStates.Ready;// ATTENTION!
                                Commander.pStatePrev = Commander.pState;
                            }
                        }
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

            Console.WriteLine(pState);
            if (pState == Commander.programStates.Ready)
            {
                pStatePrev = pState;
                pState = Commander.programStates.Measure;
                Commander.isSenseMeasure = false;
                Graph.ResetPointLists();
                if (!Commander.notRareModeRequested) toSend.IsRareMode = true;
                scanning = true;
                Commander.measureCancelRequested = false;
                toSend.AddToSend(new sendIVoltage());
            }
        }
        internal static void Sense()
        {
            if (pState == Commander.programStates.Ready)
            {
                if (somePointsUsed())
                {
                    pStatePrev = pState;
                    pState = Commander.programStates.Measure;
                    Commander.isSenseMeasure = true;
                    Graph.ResetPointLists();
                    if (!Commander.notRareModeRequested) toSend.IsRareMode = true;
                    Commander.measureCancelRequested = false;
                    toSend.AddToSend(new sendIVoltage());
                }
                else {
                    Console.WriteLine("Ќет точек дл€ прецизионного режима");
                }
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
        }
        internal static void Monitor()
        {
            throw new Exception("The method or operation is not implemented.");
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
            if (Commander.pState == programStates.Measure)
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
