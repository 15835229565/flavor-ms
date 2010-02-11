using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;
using Flavor.Common.Commands.UI;

namespace Flavor.Common.Commands.Async
{
    public abstract class AsyncReply: ServicePacket
    {
    }

    public class requestCounts : AsyncReply, IAutomatedReply
    {
        #region IReply Members
        public void AutomatedReply()
        {
            //хорошо бы сюда на автомате очистку Commander.CustomMeasure...
            Commander.AddToSend(new getCounts());
        }
        #endregion
    }

    public class confirmVacuumReady : AsyncReply, IUpdateDevice
    {
        #region IUpdateDevice Members
        public void UpdateDevice()
        {
        }
        #endregion
    }

    public class confirmShutdowned : AsyncReply
    {
    }

    public class SystemReseted : AsyncReply
    {
    }

    public class confirmHighVoltageOff : AsyncReply
    {
    }

    public class confirmHighVoltageOn : AsyncReply
    {
    }
}