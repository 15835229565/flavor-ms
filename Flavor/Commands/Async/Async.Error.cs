using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    abstract class AsyncErrorReply : ServicePacket//, IUpdateUserMessage
    {
        public virtual string errorMessage
        {
            get{return "Асинхронный ответ об ошибке";}
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
            get { return "Внутренняя ошибка " + internalError.ToString(); }
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
            get { return "Неверное состояние системы"; }
        }
    }

    class logVacuumCrash : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Вакуум сдох " + vacState.ToString(); }
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
            get { return "Отказ турбонасоса"; }
        }
    }
    
    class logPowerFail : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Сбой питания прибора"; }
        }
    }
    
    class logInvalidVacuumState : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Неверное состояние вакуума"; }
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
