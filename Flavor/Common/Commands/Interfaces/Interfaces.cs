using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Commands.Interfaces
{
    public class ServicePacket
    {
    }

    public class SyncServicePacket: ServicePacket
    {
        public virtual ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.None; }
        }
    }

    interface ISend
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