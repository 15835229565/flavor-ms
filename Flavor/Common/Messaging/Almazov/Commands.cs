namespace Flavor.Xmega32A4U_testBoard
{
    public struct Command
    {
        //СТРУКТУРА: Хранилище констант - кодов команд
        #region Chip
        public enum Chip: byte
        {
            //Коды команд микросхемы микроконтроллера
            getVersion = 1,
            getBirthday = 2,
            getCPUfrequency = 3,
            getStatus = 20,
            reset = 4,
            wait = 5,
        }
        #endregion
        #region RTC
        public enum RTC: byte
        {
            //Коды команд счётчиков
            startMeasure = 30,
            stopMeasure = 31,
            receiveResults = 32,
        }
        #endregion
        #region TIC
        public enum TIC: byte
        {
            retransmit = 50,
            set_Gauges = 51,
            get_TIC_MEM = 53,
        }
        #endregion
        #region SPI
        public struct SPI
        {
            //Коды команд для SPI устройств
            public struct PSIS
            {
                //Коды команд Ионного Источника

                public const byte setVoltage = 40;
                public const byte getVoltage = 60;

            }
            public struct DPS
            {
                //Коды команд Детектора
                public const byte setVoltage = 41;
                public const byte getVoltage = 61;
            }
            public struct PSInl
            {
                //Коды команд Натекателя
                public const byte setVoltage = 44;
                public const byte getVoltage = 63;
            }
            public struct Scaner
            {
                //Коды команд Сканера
                public const byte setVoltage = 42;
                public const byte getVoltage = 62;

            }
            public struct Condensator
            {
                //Коды команд Конденсатора
                public const byte setVoltage = 43;
                public const byte getVoltage = 62;
            }
        }
        #endregion
        public const byte LOCK = 13;
        public const byte KEY = 58;
        #region Service
        public enum Service: byte
        {
            //Коды команд отладки
            showMeByte = 21,
            checkCommandStack = 8,
        }
        #endregion
        #region Flags
        public enum Flags: byte
        {
            HVE = 71,
            PRGE = 72,
            EDCD = 73,
            SEMV1 = 74,
            SEMV2 = 75,
            SEMV3 = 76,
            SPUMP = 77,
        }
        #endregion
    }
}