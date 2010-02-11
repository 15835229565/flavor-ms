using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Commands.Interfaces
{
    internal class ServicePacket
    {
    }

    internal class SyncServicePacket: ServicePacket
    {
        internal virtual ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.None; }
        }
    }

    internal interface ISend
    {
        void Send();
    }

    interface IAutomatedReply
    {
        void AutomatedReply();
    }

    interface IUpdateDevice
    {
        void UpdateDevice();
    }
    
    interface IUpdateGraph
    {
        void UpdateGraph();
    }
    /*
    interface IUpdateUserMessage
    {
        void UpdateUserMessage();
    }
    */
}