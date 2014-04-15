using System;

namespace Flavor.Common.Messaging {
    interface IRealizer: IConnectActions, ISystemEvents, IOperationBlock, IOnOff, IMeasure, IOnTheFlyEvents, IUndoEvents, ILog {
        void SetSettings();
        void Reset();
    }
    interface IConnectActions {
        void Connect();
        void Disconnect();
    }
    interface ISystemEvents {
        event EventHandler<EventArgs<bool>> SystemDown;
        event EventHandler<EventArgs> SystemReady;
        event EventHandler<EventArgs<IUpdateDevice>> UpdateDevice;
    }
    interface IOperationBlock {
        event EventHandler<EventArgs<bool>> OperationBlock;
        void SetOperationBlock(bool block);
    }
    interface IOnOff {
        event EventHandler<EventArgs<bool>> OperationToggle;
        void SetOperationToggle(bool on);
    }
    interface IMeasure {
        event EventHandler<EventArgs> MeasurePreconfigured;
        event EventHandler<EventArgs<Action<ushort, ushort>>> MeasureSend;
        event EventHandler<EventArgs> MeasureDone;
        void SetMeasureStep(ushort step);
    }
    interface IOnTheFlyEvents {
        event EventHandler<EventArgs<Action>> FirstStatus;
    }
    interface IUndoEvents {
        event EventHandler Undo;
    }
}