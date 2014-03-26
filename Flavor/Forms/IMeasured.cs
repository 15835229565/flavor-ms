using System;

namespace Flavor.Forms {
    internal interface IMeasured {
        event EventHandler MeasureCancelRequested;
        void initMeasure(bool isPrecise);
        void deactivateOnMeasureStop();
    }
}
