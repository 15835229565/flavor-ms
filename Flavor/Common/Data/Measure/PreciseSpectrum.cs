using System.Collections.Generic;
using CommonOptions = Flavor.Common.Settings.CommonOptions;

namespace Flavor.Common.Data.Measure {
    class PreciseSpectrum: List<PreciseEditorData> {
        CommonOptions myCommonOptions = null;
        public PreciseSpectrum(CommonOptions cd)
            : this() {
            // better to clone here?
            myCommonOptions = cd;
        }
        public PreciseSpectrum()
            : base() {
        }
        public PreciseSpectrum(IEnumerable<PreciseEditorData> other)
            : base(other) {
            // TODO: check that copies here!
        }
        public CommonOptions CommonOptions {
            get { return myCommonOptions; }
            set { myCommonOptions = value; }
        }
    }
}
