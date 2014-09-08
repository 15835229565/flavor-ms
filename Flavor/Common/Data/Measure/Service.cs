using System.Collections.Generic;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Common.Data.Measure {
    static class ExtensionMethods {
        // TODO: move to more specific location (Almazov only)
        public static bool IsOxygen(this PreciseEditorData ped) {
            // TODO: better comment parsing
            // and/or use id+mass pair
            return ped.Comment == "!o2";
        }
        public static bool IsCarbonDioxide(this PreciseEditorData ped) {
            // TODO: better comment parsing
            // and/or use id+mass pair
            return ped.Comment == "!co2";
        }
        
        public static List<PreciseEditorData> GetUsed(this List<PreciseEditorData> peds) {
            return peds.FindAll(PreciseEditorData.PeakIsUsed);
        }
        public static List<PreciseEditorData> GetWithId(this List<PreciseEditorData> peds) {
            return peds.FindAll(x => x.Comment.StartsWith(Config.ID_PREFIX_TEMPORARY));
        }
    }
}