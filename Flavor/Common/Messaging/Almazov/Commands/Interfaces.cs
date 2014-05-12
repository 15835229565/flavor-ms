namespace Flavor.Common.Messaging.Almazov.Commands {
    interface IChannel {
        byte Channel { get; }
    }
    interface ITIC {
        string Request { get; }
    }
}
