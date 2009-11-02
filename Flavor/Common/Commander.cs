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
        
        private static byte Try = 0;

        private static Queue<UserRequest> ToSend = new Queue<UserRequest>();
        private static bool statusToSend = false;
        private static bool turboToSend = false;

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
            //workaround needing
            if (!statusToSend)
            {
                Commander.AddToSend(new requestStatus());
            }
            //if ((ToSend.Count == 0) || !scanning)
            //    Commander.AddToSend(new requestStatus());
        }

        private static void TurboPumpCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //workaround needing
            if (!turboToSend)
            {
                Commander.AddToSend(new getTurboPumpStatus());
            }
            //if ((ToSend.Count == 0) || !scanning)
            //    Commander.AddToSend(new getTurboPumpStatus());
        }

        private static void StartSending()
        {
            lock (SendTimer)
            {
                if (Try != 0)
                {
                    Console.WriteLine("Опа. Таймер отправки сообщений уже был запущен.");
                    return;
                }
                Try = 1;
                SendTimer.Elapsed += new System.Timers.ElapsedEventHandler(SendTime_Elapsed);
                SendTimer.Enabled = true;
            }
        }

        private static void StopSending()
        {
            lock (SendTimer)
            {
                if (Try == 0)
                {
                    Console.WriteLine("Опа. Таймер отправки сообщений уже был остановлен.");
                    return;
                }
                SendTimer.Elapsed -= new System.Timers.ElapsedEventHandler(SendTime_Elapsed);
                SendTimer.Enabled = false;
                Try = 0;
            }
        }

        private static void SendTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (SendTimer)
            {
                ++Try;
            }
            if (Try > Config.Try)
            {
                lock (ToSend)
                {
                    UserRequest packet = null;
                    if (ToSend.Count == 0)
                        Console.WriteLine("Опа. Очередь пакетов пуста, а счетчик отправки не обнулен.");
                    else
                    {
                        if (dequeueToSendInsideLock(ref packet))
                        {
                            if (packet == null)
                                Console.WriteLine("Кирдык. В очереди пакетов лежал null.");
                        }
                    }
                    StopSending();
                    if (packet != null)
                        Console.WriteLine("Прибор не отвечает на {0}", packet.Id);
                    if (Commander.pState != Commander.pStatePrev)
                        Commander.pState = Commander.pStatePrev;
                }
                return;
            }
            Send();
        }

        public static void AddToSend(UserRequest Command)
        {
            lock(ToSend)
            {
                ToSend.Enqueue(Command);
                if (Command is requestStatus)
                {
                    statusToSend = true;
                }
                if (Command is getTurboPumpStatus)
                {
                    turboToSend = true;
                }
                if (SendTimer.Enabled == false)
                {
                    Send();
                }
            }
        }

        private static bool dequeueToSendInsideLock(ref UserRequest packet)
        {
            try
            {
                packet = ToSend.Dequeue();
                if (packet is requestStatus)
                {
                    statusToSend = false;
                }
                if (packet is getTurboPumpStatus)
                {
                    turboToSend = false;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Кирдык. Не удалось удалить из очереди пакетов, хотя там должно что-то быть.");
            }
            try
            {
                ToSend.Clear();
                return false;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Кирдык. Не удалось очистить очередь пакетов.");
            }
            Console.WriteLine("Пересоздаем очередь пакетов.");
            ToSend = new Queue<UserRequest>();
            return false;
        }

        private static void peekToSendInsideLock(ref UserRequest packet)
        {
            try
            {
                packet = ToSend.Peek();
                if (packet == null)
                    Console.WriteLine("Кирдык. Из очереди пакетов достался null.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Кирдык. Не удалось достать из очереди пакетов, хотя там должно что-то быть.");
            }
        }
        
        public static void Send()
        {
            lock (ToSend)
            {
                while (ToSend.Count > 0)
                {
                    UserRequest packet = null;
                    peekToSendInsideLock(ref packet);
                    if (packet != null)
                    {
                        if (0 == Try)
                        {
                            StartSending();
                        }
                        packet.Send();
                        break;
                    }
                    if (dequeueToSendInsideLock(ref packet)) 
                        continue;
                    break;
                }
            }
        }

        public static void Realize(ServicePacket Command)
        {
            if (Command is AsyncErrorReply)
            {
                CheckInterfaces(Command);
                Console.WriteLine("Бяда: {0}", ((AsyncErrorReply)Command).errorMessage);
                Commander.OnAsyncReply(((AsyncErrorReply)Command).errorMessage);
                if (Commander.pState != Commander.programStates.Start)
                {
                    if (scanning & !Commander.notRareModeRequested) Commander.StopScanStatusCheck();
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
                    Console.WriteLine("Система отключена");
                    Commander.pState = Commander.programStates.Start;
                    Commander.pStatePrev = Commander.pState;
                    Commander.hBlock = true;
                    Console.WriteLine(Commander.pState);
                    scanning = false;
                    Device.Init();
                }
                if (Command is SystemReseted)
                {
                    Console.WriteLine("Система переинициализировалась");
                    Commander.OnAsyncReply("Система переинициализировалась");
                    if (Commander.pState != Commander.programStates.Start)
                    {
                        if (scanning & !Commander.notRareModeRequested) Commander.StopScanStatusCheck();
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
                    Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
                    Commander.AddToSend(new sendIVoltage());// и остальные напряжения затем
                }
            }
            if (Command is SyncErrorReply)
            {
                lock (ToSend)
                {
                    UserRequest packet = null;
                    dequeueToSendInsideLock(ref packet);
                    StopSending();
                    if (Commander.pState != Commander.pStatePrev)
                        Commander.pState = Commander.pStatePrev;
                }
                CheckInterfaces(Command);
                if (SendTimer.Enabled == false)
                    Send();
            }
            if (Command is SyncReply)
            {
                UserRequest packet = null;
                lock (ToSend)
                {
                    if (ToSend.Count == 0)
                    {
                        Console.WriteLine("Пришло {0}. Не ждали ничего.", Command);
                        return;
                    }
                    peekToSendInsideLock(ref packet);
                    if (packet == null)
                    {
                        dequeueToSendInsideLock(ref packet);
                        return;
                    }
                    if (packet.Id != ((SyncReply)Command).Id)
                    {
                        Console.WriteLine("Пришло {0}. Ждали {1}.", Command, packet);
                        return;
                    }
                    StopSending();
                    if (!dequeueToSendInsideLock(ref packet))
                        return;
                }
                CheckInterfaces(Command);
                if (Command is confirmInit)
                {
                    Console.WriteLine("Запрос на инициализацию подтвержден");
                    Commander.pState = Commander.programStates.Init;
                    Commander.pStatePrev = Commander.pState;
                    Console.WriteLine(Commander.pState);
                }
                if (Command is confirmShutdown)
                {
                    Console.WriteLine("Запрос на отключение подтвержден");
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
                            Commander.AddToSend(new sendSVoltage(Commander.Point++));
                        }
                        else
                        {
                            if (scanning)
                            {
                                if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                scanning = false;
                                Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
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
                                Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                            }
                            else
                            {
                                --(senseModePeakIteration[senseModePeak]);
                                --smpiSum;
                                if (smpiSum > 0)
                                {
                                    for (int i = 0; i < senseModePoints.Length; ++i)//Поиск пика с оставшейся ненулевой итерацией. Но не более 1 цикла.
                                    {
                                        ++senseModePeak;
                                        if (senseModePeak >= senseModePoints.Length) senseModePeak = 0;
                                        if (senseModePeakIteration[senseModePeak] > 0) break;
                                    }
                                    ushort nextPoint = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                    if (Commander.Point > nextPoint)
                                    {
                                        //!!!case of backward voltage change
                                        customMeasure = new sendMeasure(Config.bTime, Config.eTime);
                                    }
                                    else
                                    {
                                        //!!!case of forward voltage change
                                        if (Config.ForwardTimeEqualsBeforeTime)
                                        {
                                            customMeasure = new sendMeasure(Config.befTime, Config.eTime);
                                        }
                                        else
                                        {
                                            customMeasure = new sendMeasure(Config.fTime, Config.eTime);
                                        }
                                    }
                                    Commander.Point = nextPoint;
                                    Commander.AddToSend(new sendSVoltage(Commander.Point++));
                                    //old code:
                                    //Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                                    //Commander.AddToSend(new sendSVoltage(Commander.Point++/*, isSenseMeasure*/));
                                }
                                else
                                {
                                    if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                                    Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
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
                            if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                            Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
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
                        customMeasure = new sendMeasure(Config.befTime, Config.eTime);
                        if (!Commander.isSenseMeasure)
                        {
                            Commander.Point = Config.sPoint;
                            Commander.AddToSend(new sendSVoltage(Commander.Point++));
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
                if (SendTimer.Enabled == false)
                {
                    Send();
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
            //добавить контрольное время ожидания выключения
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
                if (somePointsUsed())
                {
                    pStatePrev = pState;
                    pState = Commander.programStates.Measure;
                    Commander.isSenseMeasure = true;
                    Graph.ResetPointLists();
                    if (!Commander.notRareModeRequested) Commander.StartScanStatusCheck();
                    Commander.measureCancelRequested = false;
                    Commander.AddToSend(new sendIVoltage());
                }
                else {
                    Console.WriteLine("Нет точек для прецизионного режима");
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
            Commander.AddToSend(new enableHighVoltage(Commander.hBlock));
        }

        internal static void Connect()
        {
            switch (ModBus.Open())
            {
                case ModBus.PortStates.Opening:
                    Commander.deviceIsConnected = true;
                    SendTimer = new System.Timers.Timer(1000);
                    SendTimer.Enabled = false;
                    StartDeviceStatusCheck();
                    break;
                case ModBus.PortStates.Opened:
                    Commander.deviceIsConnected = true;
                    break;
                case ModBus.PortStates.ErrorOpening:
                    break;
                default:
                    // фигня
                    break;
            }
        }

        internal static void Disconnect()
        {
            StopDeviceStatusCheck();
            lock (ToSend)
            {
                if (ToSend.Count > 0)
                {
                    Commander.ToSend.Clear();
                }
                if (SendTimer.Enabled == true)
                {
                    StopSending();
                }
                statusToSend = false;
                turboToSend = false;
            }
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
                    // фигня
                    break;
            }
        }
    }
}
