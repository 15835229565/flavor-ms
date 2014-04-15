using System;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Almazov;

namespace Flavor.Common {
    class AlmazovCommander: Commander {
        public AlmazovCommander()
            // TODO: proper device
            : base(new PortLevel(), null) { }
        protected override IRealizer GetRealizer(PortLevel port) {
            return new AlmazovRealizer(port, () => notRare() ? 500 : 10000);
        }
        // TODO: common
        bool notRare() {
            if (pState == ProgramStates.Measure || pState == ProgramStates.BackgroundMeasureReady || pState == ProgramStates.WaitBackgroundMeasure)
                return notRareModeRequested;
            return true;
        }

        public override void Bind(IMSControl view) {
            base.Bind(view);
            view.Unblock += Unblock;
        }

        // TODO: common
        void Unblock(object sender, CallBackEventArgs<bool> e) {
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                MeasureCancelRequested = true;
            // TODO: check!
            e.Value = hBlock;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationBlock(hBlock);
        }

        public override void Scan() {
            throw new NotImplementedException();
        }

        public override bool Sense() {
            throw new NotImplementedException();
        }

        public override bool? Monitor() {
            throw new NotImplementedException();
        }
    }
}
