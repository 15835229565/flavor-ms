using System.Collections.Generic;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Common.Data.Measure {
    static class ExtensionMethods {
        public static List<PreciseEditorData> getUsed(this List<PreciseEditorData> peds) {
            return peds.FindAll(PreciseEditorData.PeakIsUsed);
        }
        public static List<PreciseEditorData> getWithId(this List<PreciseEditorData> peds) {
            // ! temporary solution
            #warning make this operation one time a cycle
            return peds.FindAll(x => x.Comment.StartsWith(Config.ID_PREFIX_TEMPORARY));
        }
    }
}