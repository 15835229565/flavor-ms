using System;

namespace Flavor {
    interface IMSControl {
        event EventHandler<CallBackEventArgs<bool, string>> Connect;
        event EventHandler<CallBackEventArgs<bool>> Init;
        event EventHandler<CallBackEventArgs<bool>> Shutdown;
        event EventHandler<CallBackEventArgs<bool>> Unblock;
        // TODO: measure mode events
    }
}
