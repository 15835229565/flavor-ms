using System;
namespace Flavor.Common.Data.Measure {
    abstract class MeasureModeBase: IMeasureMode {
        public event EventHandler<VoltageStepEventArgs> VoltageStepChangeRequested;
        protected virtual void OnVoltageStepChangeRequested(ushort step) {
            // TODO: lock here?
            if (VoltageStepChangeRequested == null)
                throw new NoListenersException();
            VoltageStepChangeRequested(this, new VoltageStepEventArgs(step));
        }

        EventHandler _successfulExit;
        //public event EventHandler SuccessfulExit;
        protected virtual void OnSuccessfulExit(EventArgs args) {
            _successfulExit(this, args);
            //SuccessfulExit.Raise(this, args);
        }
        public event EventHandler Disable;
        protected virtual void OnDisable() {
            Disable.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<CancelEventArgs> Finalize;
        protected virtual void OnFinalize(CancelEventArgs e) {
            Finalize.Raise(this, e);
        }

        protected MeasureModeBase(EventHandler successfulExit) {
            _successfulExit = successfulExit;
            isOperating = false;
        }

        public bool isOperating { get; protected set; }
        public bool CancelRequested { protected get; set; }
        abstract public int StepsCount { get; }
        abstract public void UpdateGraph();
        abstract public void NextMeasure(Action<ushort, ushort> send);
        abstract public bool onUpdateCounts(uint[] counts);
        public virtual bool Start() {
            return isOperating = true;
        }
    }
}