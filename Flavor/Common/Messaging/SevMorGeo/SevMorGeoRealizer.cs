using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging.SevMorGeo {
    class SevMorGeoRealizer: Realizer<CommandCode> {
        readonly MessageQueueWithAutomatedStatusChecks<CommandCode> queue;
        public SevMorGeoRealizer(PortLevel port, Generator<int> factor, Generator<double> interval)
            : base(new MessageQueueWithAutomatedStatusChecks<CommandCode>(new ModBus(port),
                new StatusRequestGenerator(new requestStatus(), new getTurboPumpStatus(), factor),
                interval)) {
        }
        class StatusRequestGenerator: IStatusRequestGenerator<CommandCode> {
            int i = 0;
            int f;
            readonly UserRequest<CommandCode> statusCheck, vacuumCheck;
            readonly Generator<int> factor;
            public UserRequest<CommandCode> Next {
                get {
                    UserRequest<CommandCode> res;
                    if (i == 0)
                        res = vacuumCheck;
                    else
                        res = statusCheck;
                    ++i;
                    i %= f;
                    return res;
                }
            }
            public void Reset() {
                f = factor();
            }
            public StatusRequestGenerator(UserRequest<CommandCode> statusCheck, UserRequest<CommandCode> vacuumCheck, Generator<int> factor) {
                this.factor = factor;
                this.statusCheck = statusCheck;
                this.vacuumCheck = vacuumCheck;
                Reset();
            }
        }
        public override void Connect() {
            base.Connect();
            queue.Start();
        }
        public override void Disconnect() {
            queue.Stop();
            base.Disconnect();
            onTheFly = true;
        }
        bool onTheFly = true;
        protected override PackageDictionary<CommandCode> GetDictionary() {
            var d = new PackageDictionary<CommandCode>();
            Action<CommandCode, Action<ServicePacket<CommandCode>>> add = (code, action) => d[(byte)code] = new PackageRecord<CommandCode>(action);
            Action<ServicePacket<CommandCode>> asyncErrorAction = p => {
                string message = string.Format("Device says: {0}", ((AsyncError<CommandCode>)p).Message);
                // TODO: system reset event, severe error

                OnAsyncReplyReceived(message);
                // TODO: subscribe in Config for event
                Config.logCrash(message);

                /*if (pState != ProgramStates.Start) {
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    MeasureCancelRequested = false;
                }*/
            };
            Action<ServicePacket<CommandCode>> syncErrorAction = p => {
                //Dequeue();
            };
            Action<ServicePacket<CommandCode>> sendAction = p => {
                Enqueue(((IAutomatedReply)p).AutomatedReply() as UserRequest<CommandCode>);
            };
            Action<ServicePacket<CommandCode>> updateDeviceAction = p => ((IUpdateDevice)p).UpdateDevice();
            Action<ServicePacket<CommandCode>, Action<ServicePacket<CommandCode>>> sync = (p, act) => {
                //if (null == Peek((Sync<CommandCode>)p))
                //    return;
                act(p);
            };
            
            add(CommandCode.InternalError, asyncErrorAction);
            add(CommandCode.InvalidSystemState, asyncErrorAction);
            add(CommandCode.VacuumCrash, asyncErrorAction);
            add(CommandCode.TurboPumpFailure, asyncErrorAction += updateDeviceAction);
            add(CommandCode.PowerFail, asyncErrorAction);
            add(CommandCode.InvalidVacuumState, asyncErrorAction);
            add(CommandCode.AdcPlaceIonSrc, asyncErrorAction);
            add(CommandCode.AdcPlaceScanv, asyncErrorAction);
            add(CommandCode.AdcPlaceControlm, asyncErrorAction);

            add(CommandCode.Measured, sendAction);
            add(CommandCode.VacuumReady, updateDeviceAction += p => {
                // TODO: ready event

                //if (hBlock) {
                //    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                //} else {
                //    setProgramStateWithoutUndo(ProgramStates.Ready);
                //}
            });
            add(CommandCode.SystemShutdowned, p => {
                // TODO: system off event
                
                OnLog("System is shutdowned");
                //setProgramStateWithoutUndo(ProgramStates.Start);
                //hBlock = true;
                //OnLog(pState.ToString());
                Device.Init();
            });
            add(CommandCode.SystemReseted, p => {
                // TODO: system reset event, severe error

                OnAsyncReplyReceived("Система переинициализировалась");
                //if (pState != ProgramStates.Start) {
                //    setProgramStateWithoutUndo(ProgramStates.Start);
                //    MeasureCancelRequested = false;
                //}
            });
            add(CommandCode.HighVoltageOff, p => {
                // TODO: block event (bool)

                //hBlock = true;
                //setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
            });
            add(CommandCode.HighVoltageOn, p => {
                // TODO: block event (bool)

                //hBlock = false;
                //if (pState == ProgramStates.WaitHighVoltage) {
                //    setProgramStateWithoutUndo(ProgramStates.Ready);
                //}
                Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                Enqueue(new sendIVoltage());// и остальные напряжения затем
            });
            
            add(CommandCode.InvalidCommand, syncErrorAction);
            add(CommandCode.InvalidChecksum, syncErrorAction);
            add(CommandCode.InvalidPacket, syncErrorAction);
            add(CommandCode.InvalidLength, syncErrorAction);
            add(CommandCode.InvalidData, syncErrorAction);
            add(CommandCode.InvalidState, syncErrorAction);

            add(CommandCode.GetState, p1 => sync(p1, updateDeviceAction/* += sendAction*/));
            add(CommandCode.GetStatus, p1 => sync(p1, updateDeviceAction += p => {
                // TODO: first status event

                if (onTheFly) {
                    switch (Device.sysState) {
                        case Device.DeviceStates.Init:
                        case Device.DeviceStates.VacuumInit:
                            //hBlock = true;
                            //setProgramStateWithoutUndo(ProgramStates.Init);
                            break;

                        case Device.DeviceStates.ShutdownInit:
                        case Device.DeviceStates.Shutdowning:
                            //hBlock = true;
                            //setProgramStateWithoutUndo(ProgramStates.Shutdown);
                            break;

                        case Device.DeviceStates.Measured:
                            Enqueue(new getCounts());
                            // waiting for fake counts reply
                            break;
                        case Device.DeviceStates.Measuring:
                            // async message here with auto send-back
                            // and waiting for fake counts reply
                            break;

                        case Device.DeviceStates.Ready:
                            //hBlock = false;
                            //setProgramStateWithoutUndo(ProgramStates.Ready);
                            break;
                        case Device.DeviceStates.WaitHighVoltage:
                            //hBlock = true;
                            //setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                            break;
                    }
                    //OnLog(pState.ToString());
                    onTheFly = false;
                }
            }));
            add(CommandCode.Shutdown, p1 => sync(p1, p => {
                // TODO: shutdown event

                OnLog("Shutdown request confirmed");
                //setProgramStateWithoutUndo(ProgramStates.Shutdown);
                //OnLog(pState.ToString());
            }));
            add(CommandCode.Init, p1 => sync(p1, p => {
                // TODO: init event

                OnLog("Init request confirmed");
                //setProgramStateWithoutUndo(ProgramStates.Init);
                //OnLog(pState.ToString());
            }/* += sendAction*/));

            // settings sequence
            add(CommandCode.SetIonizationVoltage, p1 => sync(p1, sendAction));
            add(CommandCode.SetCapacitorVoltage, p1 => sync(p1, sendAction));
            add(CommandCode.heatCurrentEnable, p1 => sync(p1, sendAction));
            add(CommandCode.SetEmissionCurrent, p1 => sync(p1, sendAction));
            add(CommandCode.SetHeatCurrent, p1 => sync(p1, sendAction));
            add(CommandCode.SetFocusVoltage1, p1 => sync(p1, sendAction));
            add(CommandCode.SetFocusVoltage2, p1 => sync(p1, p => {
                // TODO: measure start event

                //if (pState == ProgramStates.Measure ||
                //    pState == ProgramStates.WaitBackgroundMeasure) {
                //    if (!CurrentMeasureMode.Start()) {
                //        OnErrorOccured("Нет точек для измерения.");
                //    }
                //}
            }));

            add(CommandCode.SetScanVoltage, p1 => sync(p1, p => {
                // TODO: measure event

                //if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //    CurrentMeasureMode.NextMeasure((t1, t2) => Enqueue(new sendMeasure(t1, t2)));
                //}
            }));

            add(CommandCode.Measure, null);
            add(CommandCode.GetCounts, p1 => sync(p1, updateDeviceAction += p => {
                // TODO: graph update event (move to Device-Graph bound, otherwise order is important)

                //if (CurrentMeasureMode == null) {
                //    //error
                //    return;
                //}
                //CurrentMeasureMode.UpdateGraph();

                // TODO: fake reply/next step event

                //if (CurrentMeasureMode == null) {
                //    // fake reply caught here (in order to put device into proper state)
                //    hBlock = false;
                //    setProgramStateWithoutUndo(ProgramStates.Ready);
                //    return;
                //}
                //if (!CurrentMeasureMode.onUpdateCounts()) {
                //    OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                //}
            }));
            add(CommandCode.EnableHighVoltage, null);
            add(CommandCode.GetTurboPumpStatus, p1 => sync(p1, updateDeviceAction));
            add(CommandCode.SetForvacuumLevel, null);
            return d;
        }
    }
}
