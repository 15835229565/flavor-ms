using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common.Commands.Sync
{
    internal abstract class SyncErrorReply : SyncServicePacket
    {
    }

    internal class logInvalidCommand : SyncErrorReply
    {
        private byte[] command;
        
        internal logInvalidCommand(byte[] errorcommand)
        {
            command = errorcommand;
        }
        
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidCommand; }
        }
    }

    internal class logInvalidChecksum : SyncErrorReply
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidChecksum; }
        }
    }

    internal class logInvalidPacket : SyncErrorReply
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidPacket; }
        }
    }

    internal class logInvalidLength : SyncErrorReply
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidLength; }
        }
    }

    internal class logInvalidData : SyncErrorReply
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidData; }
        }
    }

    internal class logInvalidState : SyncErrorReply
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidState; }
        }
    }
}