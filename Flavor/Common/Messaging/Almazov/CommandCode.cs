﻿namespace Flavor.Common.Messaging.Almazov {
    enum CommandCode: byte {
        CPU_Version = 1,
        CPU_Birthday = 2,
        CPU_Frequency = 3,
        CPU_Status = 20,
        CPU_Reset = 4,
        CPU_Wait = 5,

        RTC_StartMeasure = 30,
        RTC_StopMeasure = 31,
        RTC_ReceiveResults = 32,
        RTC_DelayedStart = 33,

        TIC_Retransmit = 50,
        TIC_GetStatus = 51,
        TIC_GetMem = 53,

        SPI_PSIS_SetVoltage = 40,
        SPI_DPS_SetVoltage = 41,
        SPI_Scan_SetVoltage = 42,
        SPI_CP_SetVoltage = 43,
        SPI_PSInl_SetVoltage = 44,

        SPI_PSIS_GetVoltage = 60,
        SPI_DPS_GetVoltage = 61,
        SPI_Scan_GetVoltage = 62,
        SPI_PSInl_GetVoltage = 63,
        
        SPI_GetAllVoltages = 64,

        HVE = 71,
        PRGE = 72,
        EDCD = 73,
        SEMV1 = 74,
        SEMV2 = 75,
        SEMV3 = 76,
        SPUMP = 77,
        
        Service_Message = 0,
    }
}