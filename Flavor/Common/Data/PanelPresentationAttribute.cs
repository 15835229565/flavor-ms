using System;

namespace Flavor.Common.Data {
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    class PanelPresentationAttribute: Attribute {
        public readonly string Id;
        public readonly int Position;
        public string Group { get; set; }
        public PanelPresentationAttribute(string id, int pos) {
            Id = id;
            Position = pos;
        }
    }
}