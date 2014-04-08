using System;

namespace Flavor.Common.Messaging.SevMorGeo {
    class SevMorGeoRealizer: Realizer<CommandCode> {
        readonly MessageQueueWithAutomatedStatusChecks<CommandCode> toSend;
        public SevMorGeoRealizer(PortLevel port, Generator<int> factor, Generator<double> interval)
            : this(new ModBus(port), factor, interval) { }
        SevMorGeoRealizer(IProtocol<CommandCode> protocol, Generator<int> factor, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                new StatusRequestGenerator(new requestStatus(), new getTurboPumpStatus(), factor),
                interval)) { }
        SevMorGeoRealizer(IProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) {
            toSend = queue;
        }
        class StatusRequestGenerator : IStatusRequestGenerator<CommandCode> {
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
        public override void SetOperationBlock(bool block) {
            toSend.Enqueue(new enableHighVoltage(block));
        }
        public override void SetOperationToggle(bool on) {
            if (on)
                toSend.Enqueue(new sendInit());
            else
                toSend.Enqueue(new sendShutdown());
        }
        public override void SetSettings() {
            toSend.Enqueue(new sendIVoltage());
            //And all sequence (see in dictionary)
        }
        public override void SetMeasureStep(ushort step) {
            toSend.Enqueue(new sendSVoltage(step));
        }
        public override void Connect(Action undo) {
            base.Connect(undo);
            toSend.Start();
        }
        public override void Disconnect(Action undo) {
            toSend.Stop();
            base.Disconnect(undo);
            onTheFly = true;
        }
        // TODO: move to abstract ancestor
        public void Reset() {
            toSend.Stop();
            toSend.Start();
        }
        bool onTheFly = true;
        protected override PackageDictionary<CommandCode> GetDictionary() {
            var d = new PackageDictionary<CommandCode>();
            Action<CommandCode, Action<ServicePacket<CommandCode>>> add = (code, action) => d[(byte)code] = new PackageRecord<CommandCode>(action);
            Action<ServicePacket<CommandCode>> asyncErrorAction = p => {
                string message = string.Format("Device says: {0}", ((AsyncError<CommandCode>)p).Message);
                OnSystemDown(true);
                OnAsyncReplyReceived(message);
                // TODO: subscribe in Config for event
                Config.logCrash(message);
            };
            Action<ServicePacket<CommandCode>> syncErrorAction = p => {
                toSend.Dequeue();
            };
            Action<ServicePacket<CommandCode>> sendAction = p => toSend.Enqueue(((IAutomatedReply)p).AutomatedReply() as UserRequest<CommandCode>);
            Action<ServicePacket<CommandCode>> updateDeviceAction = p => ((IUpdateDevice)p).UpdateDevice();
            Action<ServicePacket<CommandCode>, Action<ServicePacket<CommandCode>>> sync = (p, act) => {
                if (null == toSend.Peek((Sync<CommandCode>)p))
                    return;
                act(p);
            };
            
            add(CommandCode.InternalError, asyncErrorAction);
            add(CommandCode.InvalidSystemState, asyncErrorAction);
            add(CommandCode.VacuumCrash, asyncErrorAction);
            add(CommandCode.TurboPumpFailure, asyncErrorAction + updateDeviceAction);
            add(CommandCode.PowerFail, asyncErrorAction);
            add(CommandCode.InvalidVacuumState, asyncErrorAction);
            add(CommandCode.AdcPlaceIonSrc, asyncErrorAction);
            add(CommandCode.AdcPlaceScanv, asyncErrorAction);
            add(CommandCode.AdcPlaceControlm, asyncErrorAction);

            add(CommandCode.Measured, sendAction);
            add(CommandCode.VacuumReady, updateDeviceAction + (p => OnSystemReady()));
            add(CommandCode.SystemShutdowned, p => OnSystemDown(false));
            add(CommandCode.SystemReseted, p => {
                OnAsyncReplyReceived("Система переинициализировалась");
                OnSystemDown(true);
            });
            add(CommandCode.HighVoltageOff, p => OnOperationBlock(true));
            add(CommandCode.HighVoltageOn, p => {
                OnOperationBlock(false);
                toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                toSend.Enqueue(new sendIVoltage());// и остальные напряжения затем
            });
            
            add(CommandCode.InvalidCommand, syncErrorAction);
            add(CommandCode.InvalidChecksum, syncErrorAction);
            add(CommandCode.InvalidPacket, syncErrorAction);
            add(CommandCode.InvalidLength, syncErrorAction);
            add(CommandCode.InvalidData, syncErrorAction);
            add(CommandCode.InvalidState, syncErrorAction);

            add(CommandCode.GetState, p1 => sync(p1, updateDeviceAction/* + sendAction*/));
            add(CommandCode.GetStatus, p1 => sync(p1, updateDeviceAction + (p => {
                if (onTheFly) {
                    // waiting for fake counts reply
                    OnFirstStatus(() => toSend.Enqueue(new getCounts()));
                    onTheFly = false;
                }
            })));
            add(CommandCode.Shutdown, p1 => sync(p1, p => OnOperationToggle(false)));
            add(CommandCode.Init, p1 => sync(p1, p => OnOperationToggle(true)/* + sendAction*/));

            // settings sequence
            add(CommandCode.SetIonizationVoltage, p1 => sync(p1, sendAction));
            add(CommandCode.SetCapacitorVoltage, p1 => sync(p1, sendAction));
            add(CommandCode.heatCurrentEnable, p1 => sync(p1, sendAction));
            add(CommandCode.SetEmissionCurrent, p1 => sync(p1, sendAction));
            add(CommandCode.SetHeatCurrent, p1 => sync(p1, sendAction));
            add(CommandCode.SetFocusVoltage1, p1 => sync(p1, sendAction));
            add(CommandCode.SetFocusVoltage2, p1 => sync(p1, p => OnMeasurePreconfigured()));

            add(CommandCode.SetScanVoltage, p1 => sync(p1, p => OnMeasureSend((t1, t2) => toSend.Enqueue(new sendMeasure(t1, t2)))));

            add(CommandCode.Measure, p1 => sync(p1, p => { }));
            add(CommandCode.GetCounts, p1 => sync(p1, updateDeviceAction + (p => OnMeasureDone())));
            add(CommandCode.EnableHighVoltage, p1 => sync(p1, p => { }));
            add(CommandCode.GetTurboPumpStatus, p1 => sync(p1, updateDeviceAction));
            add(CommandCode.SetForvacuumLevel, p1 => sync(p1, p => { }));
            return d;
        }
    }
}
