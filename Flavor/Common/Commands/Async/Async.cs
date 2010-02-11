using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;
using Flavor.Common.Commands.UI;

namespace Flavor.Common.Commands.Async
{
    internal abstract class AsyncReply: ServicePacket
    {
    }

    internal class requestCounts : AsyncReply, IAutomatedReply
    {
        #region IReply Members
        public void AutomatedReply()
        {
            //хорошо бы сюда на автомате очистку Commander.CustomMeasure...
            Commander.AddToSend(new getCounts());
        }
        #endregion
    }

    internal class confirmVacuumReady : AsyncReply, IUpdateDevice
    {
        #region IUpdateDevice Members
        public void UpdateDevice()
        {
        }
        #endregion
    }

    internal class confirmShutdowned : AsyncReply
    {
    }

    internal class SystemReseted : AsyncReply
    {
    }

    internal class confirmHighVoltageOff : AsyncReply
    {
    }

    internal class confirmHighVoltageOn : AsyncReply
    {
    }
}