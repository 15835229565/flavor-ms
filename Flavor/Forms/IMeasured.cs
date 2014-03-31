using System;

namespace Flavor.Forms {
    internal interface IMeasured {
        event EventHandler MeasureCancelRequested;
        void initMeasure(int progressMaximum, bool isPrecise);
        void deactivateOnMeasureStop();
    }
}
