using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Flavor
{
    delegate void ProgramEventHandler();
    delegate void AsyncReplyHandler(string msg);
    
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

        public static System.Timers.Timer DeviceStatusCheckTimer;
        public static System.Timers.Timer TurboPumpCheckTimer;

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

        private static PreciseEditorData[] senseModePoints;

        private static int[][] senseModeCounts;
        
        private static byte senseModePeak = 0;

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

        public static ushort Point
        {
            get { return PointValue; }
            set { PointValue = value; }
        }

        private static byte Try = 0;

        private static Queue<ISend> ToSend = new Queue<ISend>();

        private static System.Timers.Timer SendTimer;

        private static void StartDeviceStatusCheck()
        {
            DeviceStatusCheckTimer = new System.Timers.Timer(500);
            DeviceStatusCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(StatusCheckTime_Elapsed);
            DeviceStatusCheckTimer.Enabled = true;
            TurboPumpCheckTimer = new System.Timers.Timer(2000);
            TurboPumpCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(TurboPumpCheckTime_Elapsed);
            TurboPumpCheckTimer.Enabled = true;
        }

        private static void StopDeviceStatusCheck()
        {
            DeviceStatusCheckTimer.Enabled = false;
            DeviceStatusCheckTimer.Elapsed -= new System.Timers.ElapsedEventHandler(StatusCheckTime_Elapsed);
            TurboPumpCheckTimer.Enabled = false;
            TurboPumpCheckTimer.Elapsed -= new System.Timers.ElapsedEventHandler(TurboPumpCheckTime_Elapsed);
        }

        private static void StartScanStatusCheck()
        {
            StopDeviceStatusCheck();
            DeviceStatusCheckTimer = new System.Timers.Timer(10000);
            DeviceStatusCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(StatusCheckTime_Elapsed);
            DeviceStatusCheckTimer.Enabled = true;
            TurboPumpCheckTimer = new System.Timers.Timer(20000);
            TurboPumpCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(TurboPumpCheckTime_Elapsed);
            TurboPumpCheckTimer.Enabled = true;
        }

        private static void StopScanStatusCheck()
        {
            DeviceStatusCheckTimer.Enabled = false;
            DeviceStatusCheckTimer.Elapsed -= new System.Timers.ElapsedEventHandler(StatusCheckTime_Elapsed);
            TurboPumpCheckTimer.Enabled = false;
            TurboPumpCheckTimer.Elapsed -= new System.Timers.ElapsedEventHandler(TurboPumpCheckTime_Elapsed);
            StartDeviceStatusCheck();
        }

        private static void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ((ToSend.Count == 0) || !scanning) Commander.AddToSend(new requestStatus());
        }

        private static void TurboPumpCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ((ToSend.Count == 0) || !scanning) Commander.AddToSend(new getTurboPumpStatus());
        }

        private static void StartSending()
        {
            Try = 1;
            SendTimer.Elapsed += new System.Timers.ElapsedEventHandler(SendTime_Elapsed);
            SendTimer.Enabled = true;
        }

        private static void StopSending()
        {
            SendTimer.Elapsed -= new System.Timers.ElapsedEventHandler(SendTime_Elapsed);
            SendTimer.Enabled = false;
            Try = 0;
        }

        private static void SendTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Try++;
            Send();
        }

        public static void AddToSend(SyncServicePacket Command)
        {
            if (Command is ISend)
            {
                ToSend.Enqueue((ISend)Command);
                if (SendTimer.Enabled == false)
                {
                    Send();
                }
            }
        }

        public static void Send()
        {
            while (ToSend.Count > 0)
            {
                if (Try <= Config.Try)
                {
                    try
                    {
                        if (0 == Try)
                        {
                            StartSending();
                        }
                        ToSend.Peek().Send();
                    }
                    catch (NullReferenceException)
                    {
                        StopSending();
                        Console.WriteLine("������");
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("������ �� �������� �� {0}", ((SyncServicePacket)(ToSend.Peek())).Id);
                    ToSend.Dequeue();
                    StopSending();
                    if (Commander.pState != Commander.pStatePrev)
                    {
                        Commander.pState = Commander.pStatePrev;
                    }
                }
            }
        }

        public static void Realize(ServicePacket Command)
        {
            if (Command is AsyncErrorReply)
            {
                Commander.OnAsyncReply(((AsyncErrorReply)Command).errorMessage);
                CheckInterfaces(Command);
            }
            if (Command is AsyncReply)
            {
                CheckInterfaces(Command);
                if (Command is confirmShutdowned)
                {
                    Console.WriteLine("������� ���������");
                    Commander.pState = Commander.programStates.Start;
                    Commander.pStatePrev = Commander.pState;
                    Commander.hBlock = true;
                    Console.WriteLine(Commander.pState);
                    scanning = false;
                    Device.Init();
                }
                if (Command is SystemReseted)
                {
                    Console.WriteLine("������� ����������������������");
                }
                if (Command is updateVacuumState)
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
                    Commander.pState = Commander.programStates.WaitHighVoltage;
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
                    Commander.AddToSend(new sendIVoltage());
                }
            }
            if (Command is SyncErrorReply)
            {
                StopSending();
                ToSend.Dequeue();
                if (Commander.pState != Commander.pStatePrev)
                {
                    Commander.pState = Commander.pStatePrev;
                }
                CheckInterfaces(Command);
                if (SendTimer.Enabled == false)
                {
                    Send();
                }
            }
            if (Command is SyncReply)
            {
                if (ToSend.Count > 0)
                {
                    try
                    {
                        if (((SyncServicePacket)(ToSend.Peek())).Id == ((SyncReply)Command).Id)
                        {
                            StopSending();
                            ToSend.Dequeue();
                            CheckInterfaces(Command);
                            if (Command is confirmInit)
                            {
                                Console.WriteLine("������ �� ������������� �����������");
                                Commander.pState = Commander.programStates.Init;
                                Commander.pStatePrev = Commander.pState;
                                Console.WriteLine(Commander.pState);
                            }
                            if (Command is confirmShutdown)
                            {
                                Console.WriteLine("������ �� ���������� �����������");
                                Commander.pState = Commander.programStates.Shutdown;
                                Commander.pStatePrev = Commander.pState;
                                Console.WriteLine(Commander.pState);
                            }
                            if (Command is updateCounts)
                            {
                                if (!Commander.isSenseMeasure)
                                {
                                    if (!Commander.measureCancelRequested && (Commander.Point <= Config.ePoint))
                                    {
                                        Commander.AddToSend(new sendSVoltage(Commander.Point++));
                                    }
                                    else
                                    {
                                        if (scanning)
                                        {
                                            if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                            scanning = false;
                                            OnScanCancelled();
                                            Commander.pStatePrev = Commander.pState;
                                            Commander.pState = Commander.programStates.Ready;
                                            Commander.pStatePrev = Commander.pState;
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
                                            Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                                        }
                                        else
                                        {
                                            --senseModePeakIteration[senseModePeak];
                                            --smpiSum;
                                            if (smpiSum > 0)
                                            {
                                                for (int i = 0; i < senseModePoints.Length; ++i)//����� ���� � ���������� ��������� ���������. �� �� ����� 1 �����.
                                                {
                                                    ++senseModePeak;
                                                    if (senseModePeak >= senseModePoints.Length) senseModePeak = 0;
                                                    if (senseModePoints[senseModePeak].Iterations != 0) break;
                                                }
                                                Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                                Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                                            }
                                            else
                                            {
                                                if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                                Commander.pStatePrev = Commander.pState;
                                                Commander.pState = Commander.programStates.Ready;
                                                Commander.pStatePrev = Commander.pState;
                                                Graph.updateGraph(senseModeCounts, senseModePoints);
                                                Config.AutoSavePreciseSpecterFile();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Graph.updateGraph(senseModeCounts, senseModePoints);
                                        if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                        Commander.pStatePrev = Commander.pState;
                                        Commander.pState = Commander.programStates.Ready;
                                        Commander.pStatePrev = Commander.pState;
                                    }
                                }
                            }
                            if (Command is confirmF2Voltage)
                            {
                                if (Commander.pState == programStates.Measure)
                                {
                                    if (!Commander.isSenseMeasure)
                                    {
                                        Commander.Point = Config.sPoint;
                                        Commander.AddToSend(new sendSVoltage(Commander.Point++));
                                    }
                                    else
                                    {
                                        if (Config.PreciseData.Count > 0)
                                        {
                                            senseModePoints = Config.PreciseData.ToArray();
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
                                            Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                                        }
                                        else
                                        {
                                            if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                            Commander.pStatePrev = Commander.pState;
                                            Commander.pState = Commander.programStates.Ready;// ATTENTION!
                                            Commander.pStatePrev = Commander.pState;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("������ {0}. ����� {1}.", Command, ToSend.Peek());
                        }
                    }
                    catch (NullReferenceException) 
                    {
                        Console.WriteLine("���-��!");
                    }
                    if (SendTimer.Enabled == false)
                    {
                        Send();
                    }
                }
                else
                {
                    Console.WriteLine("������ {0}. �� ����� ������.", Command);
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
            Commander.AddToSend(new sendInit());

            Console.WriteLine(pState);
        }

        internal static void Shutdown()
        {

            Commander.AddToSend(new sendShutdown());
            Commander.pStatePrev = Commander.pState;
            Commander.pState = Commander.programStates.WaitShutdown;
            //�������� ����������� ����� �������� ����������
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
                if (!Commander.notRareModeRequested) Commander.StartScanStatusCheck();
                scanning = true;
                Commander.measureCancelRequested = false;
                Commander.AddToSend(new sendIVoltage());
            }
        }

        internal static void Sense()
        {
            if (pState == Commander.programStates.Ready)
            {
                if (Config.PreciseData.Count > 0)
                {
                    pStatePrev = pState;
                    pState = Commander.programStates.Measure;
                    Commander.isSenseMeasure = true;
                    Graph.ResetPointLists();
                    if (!Commander.notRareModeRequested) Commander.StartScanStatusCheck();
                    Commander.AddToSend(new sendIVoltage());
                }
                else {
                    Console.WriteLine("��� ����� ��� ������������� ������");
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

        internal static void Unblock()
        {
            if (Commander.pState == programStates.Measure)
            {
                if (!Commander.isSenseMeasure)
                {
                    Commander.measureCancelRequested = true;
                }
                else
                {
                }
            }
            Commander.AddToSend(new enableHighVoltage(Commander.hBlock));
        }

        internal static void Connect()
        {
            switch (ModBus.Open())
            {
                case ModBus.PortStates.Opening:
                    Commander.deviceIsConnected = true;
                    SendTimer = new System.Timers.Timer(2000);
                    SendTimer.Enabled = false;
                    StartDeviceStatusCheck();
                    break;
                case ModBus.PortStates.Opened:
                    Commander.deviceIsConnected = true;
                    break;
                case ModBus.PortStates.ErrorOpening:
                    break;
                default:
                    // �����
                    break;
            }
        }

        internal static void Disconnect()
        {
            //SendTimer.Enabled = true;
            switch (ModBus.Close())
            {
                case ModBus.PortStates.Closing:
                    StopDeviceStatusCheck();
                    if (ToSend.Count > 0)
                    {
                        Commander.ToSend.Clear();
                    }
                    if (SendTimer.Enabled == true)
                    {
                        StopSending();
                    }
                    /*SendTimer.Enabled = false;
                    if (ToSend.Count > 0)
                    {
                        Commander.ToSend.Clear();
                    }*/
                    Commander.deviceIsConnected = false;
                    break;
                case ModBus.PortStates.Closed:
                    Commander.deviceIsConnected = false;
                    break;
                case ModBus.PortStates.ErrorClosing:
                    break;
                default:
                    // �����
                    break;
            }
        }
    }
}
