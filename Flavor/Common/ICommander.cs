using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flavor.Common.Messaging.Commands;

namespace Flavor.Common {
    abstract class ICommander {
        internal enum programStates: byte { }

        delegate void ProgramEventHandler();
        delegate void MessageHandler(string msg);
        
        event ProgramEventHandler OnProgramStateChanged;
        event ProgramEventHandler OnScanCancelled;
        event MessageHandler Error;
        event MessageHandler AsyncReply;
        event MessageHandler Log;

        abstract internal MeasureMode CurrentMeasureMode { get; }
        internal programStates pState { get; private set; }
        internal programStates pStatePrev { get; private set; }
        abstract internal bool hBlock { get; set; }
        abstract internal bool measureCancelRequested { get; set; }
        abstract internal bool notRareModeRequested { get; set; }
        internal bool DeviceIsConnected { get; private set; }
        abstract internal bool SomePointsUsed { get; }

        //Messaging only
        abstract internal void AddToSend(UserRequest command);
        abstract internal void DisableMeasure();

        //OptionsForm only
        abstract internal void sendSettings();
        
        //mainForm only
        abstract internal void Init();
        abstract internal void Shutdown();

        abstract internal void Unblock();

        abstract internal void Scan();
        abstract internal bool Sense();
        abstract internal bool? Monitor();

        abstract internal PortLevel.PortStates Connect();
        abstract internal PortLevel.PortStates Disconnect();

        //ConnectOptionsForm only
        abstract internal string[] AvailablePorts { get; }
        abstract internal void Reconnect();
    }
}
