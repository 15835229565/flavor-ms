using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Measuring
{
    abstract public class MeasureMode
    {
        abstract public void onUpdateCounts();
        abstract public void start();
    }
}
