namespace Flavor.Common.Messaging.SevMorGeo {
    enum CommandCode: byte {
        None = 0x00,// & min length
        //sync
        GetState = 0x01,
        GetStatus = 0x02,
        Shutdown = 0x03,
        Init = 0x04,
        SetHeatCurrent = 0x05,
        SetEmissionCurrent = 0x06,
        SetIonizationVoltage = 0x07,
        SetFocusVoltage1 = 0x08,
        SetFocusVoltage2 = 0x09,
        SetScanVoltage = 0x0A,
        SetCapacitorVoltage = 0x0B,
        Measure = 0x0C,
        GetCounts = 0x0D,
        //heatCurrentEnable = 0x0E,
        //emissionCurrentEnable = 0x0F,
        heatCurrentEnable = 0x11,
        EnableHighVoltage = 0x14,
        GetTurboPumpStatus = 0x15,
        SetForvacuumLevel = 0x16,
        //syncerr
        InvalidCommand = 0x40,
        InvalidChecksum = 0x80,
        InvalidPacket = 0x81,
        InvalidLength = 0x82,
        InvalidData = 0x83,
        InvalidState = 0x84,
        //asyncerr
        InternalError = 0xC0,
        InvalidSystemState = 0xC1,
        VacuumCrash = 0xC2,//+ еще что-то..
        TurboPumpFailure = 0xC3,
        PowerFail = 0xC4,
        InvalidVacuumState = 0xC5,
        AdcPlaceIonSrc = 0xC8,
        AdcPlaceScanv = 0xC9,
        AdcPlaceControlm = 0xCA,
        //async
        Measured = 0xE0,
        VacuumReady = 0xE1,
        SystemShutdowned = 0xE2,
        SystemReseted = 0xE3,//+ еще что-то..
        HighVoltageOff = 0xE5,
        HighVoltageOn = 0xE6
    }
}