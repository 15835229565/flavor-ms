using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    abstract class AsyncReply: ServicePacket
    {
    }
    
    class requestCounts : AsyncReply, IAutomatedReply
    {
        #region IReply Members
        public void AutomatedReply()
        {
            Commander.AddToSend(new getCounts());
        }
        #endregion
    }
    
    class updateVacuumState : AsyncReply, IUpdateDevice
    {
        #region IUpdateDevice Members
        public void UpdateDevice()
        {
        }
        #endregion
    }
    
    class confirmShutdowned : AsyncReply
    {
    }
    
    class SystemReseted : AsyncReply
    {
    }

    class confirmHighVoltageOff : AsyncReply
    {
    }

    class confirmHighVoltageOn : AsyncReply
    {
    }
}