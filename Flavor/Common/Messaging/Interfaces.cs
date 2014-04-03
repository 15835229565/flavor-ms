namespace Flavor.Common.Messaging {
    interface IId<T> {
        T Id { get; }
    }
    interface ISend {
        byte[] Data { get; }
    }
    interface IAutomatedReply {
        ISend AutomatedReply();
    }
    interface IUpdateDevice {
        void UpdateDevice();
    }
    interface IUpdateGraph { }
    interface IStatusRequest { }
}