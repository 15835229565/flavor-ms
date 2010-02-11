using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common.Commands.Sync
{
    public abstract class SyncErrorReply : SyncServicePacket
    {
    }

    public class logInvalidCommand : SyncErrorReply
    {
        private byte[] command;
        
        public logInvalidCommand(byte[] errorcommand)
        {
            command = errorcommand;
        }
        
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidCommand; }
        }
    }

    public class logInvalidChecksum : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidChecksum; }
        }
    }

    public class logInvalidPacket : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidPacket; }
        }
    }

    public class logInvalidLength : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidLength; }
        }
    }

    public class logInvalidData : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidData; }
        }
    }

    public class logInvalidState : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidState; }
        }
    }
}