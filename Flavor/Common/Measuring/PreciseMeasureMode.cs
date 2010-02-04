using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    class PreciseMeasureMode: MeasureMode
    {
        public override void onUpdateCounts()
        {
            throw new Exception("Not implemented/");
            base.onUpdateCounts();
        }
        public override void start()
        {
            base.start();
            throw new Exception("Not implemented/");
        }
    }
}
