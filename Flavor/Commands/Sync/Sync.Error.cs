using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    abstract class SyncErrorReply: SyncServicePacket
    {
    }
    
    class logInvalidCommand : SyncErrorReply
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

    class logInvalidChecksum : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidChecksum; }
        }
        
    }
    
    class logInvalidPacket : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidPacket; }
        }
        
    }
    
    class logInvalidLength : SyncErrorReply

    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidLength; }
        }
        
    }
    
    class logInvalidData : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidData; }
        }
        
    }
    
    class logInvalidState : SyncErrorReply
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.InvalidState; }
        }
        
    }
}