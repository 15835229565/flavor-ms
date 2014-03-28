namespace Flavor.Common.Messaging.Commands
{
    internal abstract class AsyncReply: ServicePacket
    {
        internal class requestCounts: AsyncReply, IAutomatedReply {
            #region IReply Members
            public UserRequest AutomatedReply() {
                //хорошо бы сюда на автомате очистку Commander.CustomMeasure...
                return new UserRequest.getCounts();
            }
            #endregion
        }

        internal class confirmVacuumReady: AsyncReply, IUpdateDevice {
            #region IUpdateDevice Members
            public void UpdateDevice() {
            }
            #endregion
        }

        internal class confirmShutdowned: AsyncReply {
        }

        internal class SystemReseted: AsyncReply {
        }

        internal class confirmHighVoltageOff: AsyncReply {
        }

        internal class confirmHighVoltageOn: AsyncReply {
        }
    }
}