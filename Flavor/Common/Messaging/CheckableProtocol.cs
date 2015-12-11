using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    abstract class CheckableProtocol<T>: Protocol<T>
        where T: struct, IConvertible, IComparable {
        protected CheckableProtocol()
            : base() {
            trim = l => {
                l = base.trim(l);
                // remove checksum at last position (is formed so as check must return 0)
                l.RemoveAt(l.Count - 1);
                return l;
            };
        }
        protected readonly new Processor<IList<byte>> trim;
        protected override bool CheckPassed(IList<byte> rawCommand) {
            if (ComputeCS(rawCommand) != 0) {
                OnErrorCommand(rawCommand, "Неверная контрольная сумма");
                return false;
            }
            return true;
        }
        protected abstract byte ComputeCS(IList<byte> data);
    }
}
