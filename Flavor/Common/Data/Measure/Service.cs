using System;
using System.Collections.Generic;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Common.Data.Measure {
    class NoListenersException: Exception { }
    class CancelEventArgs: EventArgs {
        public bool Cancel { get; set; }
        public CancelEventArgs(bool cancel) {
            Cancel = cancel;
        }
    }
    class VoltageStepEventArgs: EventArgs {
        public ushort Step { get; private set; }
        public VoltageStepEventArgs(ushort step) {
            Step = step;
        }
    }
    class SingleMeasureEventArgs: EventArgs {
        public ushort IdleTime { get; private set; }
        public ushort ExpositionTime { get; private set; }
        public SingleMeasureEventArgs(ushort idleTime, ushort expositionTime) {
            IdleTime = idleTime;
            ExpositionTime = expositionTime;
        }
    }
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
            return peds.FindAll(ped => ped != null && ped.Use);
        }
        public static List<PreciseEditorData> GetWithId(this List<PreciseEditorData> peds) {
            return peds.FindAll(ped => ped != null && ped.Comment.StartsWith(Config.ID_PREFIX_TEMPORARY));
        }
    }
}