using System;
namespace Flavor.Common.Data.Measure {
    interface IMeasureMode {
        bool CancelRequested { set; }
        event EventHandler<VoltageStepEventArgs> VoltageStepChangeRequested;
        event EventHandler Disable;
        event EventHandler SuccessfulExit;
        event EventHandler<CancelEventArgs> Finalize;
        bool isOperating { get; }
        void NextMeasure(Action<ushort, ushort> send);
        bool onUpdateCounts(uint[] counts);
        bool Start();
        int StepsCount { get; }
        void UpdateGraph();
    }
}
