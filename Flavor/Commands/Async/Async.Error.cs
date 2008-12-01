using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    abstract class AsyncErrorReply : ServicePacket//, IUpdateUserMessage
    {
        public virtual string errorMessage
        {
            get{return "����������� ����� �� ������";}
        }
        /*
        #region IUpdateUserMessage Members

        public void UpdateUserMessage()
        {
        }

        #endregion
        */
    }
    
    class logInternalError : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "���������� ������ " + internalError.ToString(); }
        }
        
        private byte internalError;

        public logInternalError(byte error) 
        {
            internalError = error;
        }
    }
    
    class logInvalidSystemState : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "�������� ��������� �������"; }
        }
    }

    class logVacuumCrash : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "������ ���� " + vacState.ToString(); }
        }
        
        byte vacState;
        public logVacuumCrash(byte vs) 
        {
            vacState = vs;
        }
    }

    class logTurboPumpFailure : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "����� �����������"; }
        }
    }
    
    class logPowerFail : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "���� ������� �������"; }
        }
    }
    
    class logInvalidVacuumState : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "�������� ��������� �������"; }
        }
    }

    class logAdcPlaceIonSrc : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceIonSrc"; }
        }
    }
    
    class logAdcPlaceScanv : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceScanv"; }
        }
    }
    
    class logAdcPlaceControlm : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceControlm"; }
        }
    }
}
