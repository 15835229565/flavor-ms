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
            //хорошо бы сюда на автомате очистку Commander.CustomMeasure...
            Commander.AddToSend(new getCounts());
        }
        #endregion
    }
    
    class confirmVacuumReady : AsyncReply, IUpdateDevice
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