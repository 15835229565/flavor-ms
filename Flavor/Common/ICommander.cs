namespace Flavor.Common {
    enum ProgramStates: byte {
        Start,
        Shutdown,
        Init,
        WaitHighVoltage,
        Ready,
        WaitBackgroundMeasure,
        BackgroundMeasureReady,
        Measure,
        WaitInit,
        WaitShutdown,
    }

    delegate void ProgramEventHandler(ProgramStates state);
    delegate void MessageHandler(string msg);
    delegate void BoolEventHandler(bool t);

    interface ILog {
        event MessageHandler Log;
    }
    interface IErrorOccured: ILog {
        event MessageHandler ErrorOccured;
    }
    interface IAsyncReplyReceived: ILog {
        event MessageHandler AsyncReplyReceived;
    }
    interface IGlobalActions {
        event ProgramEventHandler ProgramStateChanged;
        void SendSettings();
    }
    interface IMeasureActions {
        event ProgramEventHandler ProgramStateChanged;
        event ProgramEventHandler MeasureCancelled;
        void Scan(byte ratio);
        bool Sense();
        bool? Monitor(params object[] data);
    }
}
