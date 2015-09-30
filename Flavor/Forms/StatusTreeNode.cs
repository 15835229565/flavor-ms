using System;
using System.Drawing;
using System.Windows.Forms;

namespace Flavor.Forms {
    abstract class StatusTreeNode: TreeNode {
        public enum AlertLevel {
            NA,
            Ok,
            Warning,
            Error
        }
        AlertLevel _state = AlertLevel.NA;
        public AlertLevel State {
            protected get { return _state; }
            set {
                if (_state != value) {
                    AlertLevel old = _state;
                    _state = value;
                    OnStateChanged(old, value);
                }
            }
        }
        protected virtual void OnStateChanged(AlertLevel old, AlertLevel current) { }
        public static StatusTreeNode newLeaf() {
            return new Value() { Text = "---" };
        }
        public static TreeNode newNode(string text, params TreeNode[] nodes) {
            TreeNode result;
            if (nodes.Length == 0)
                result = new Value();
            else if (nodes.Length == 1 && nodes[0] is Value)
                result = new Key();
            else
                result = new Category();
            result.Text = text;
            result.Nodes.AddRange(nodes);
            return result;
        }
        class Category: StatusTreeNode {
            protected override void OnStateChanged(AlertLevel old, AlertLevel current) {
                switch (current) {
                    case AlertLevel.NA:
                    case AlertLevel.Ok:
                        StateImageKey = "";
                        break;
                    case AlertLevel.Warning:
                        StateImageKey = "warning";
                        break;
                    case AlertLevel.Error:
                        StateImageKey = "error";
                        break;
                }
                if (Parent is Category) {
                    ((Category)Parent).ChildNewState(old, current);
                }
            }
            public void ChildNewState(AlertLevel childPrevious, AlertLevel childCurrent) {
                AlertLevel state = State;
                if (state < childPrevious) {
                    // illegal previous state: child alert level was more critical than parent. try to heal!
                    State = ComputeStateFromChildren(childCurrent);
                    return;
                }
                if (state < childCurrent) {
                    // rise alert level up to child's
                    State = childCurrent;
                    return;
                }
                if (state > childCurrent && state == childPrevious) {
                    // child alert level is lowering, recount
                    State = ComputeStateFromChildren(childCurrent);
                }
            }
            AlertLevel ComputeStateFromChildren(AlertLevel hint) {
                AlertLevel result = hint;
                foreach (StatusTreeNode node in Nodes) {
                    if (result < node.State) {
                        result = node.State;
                        if (result == AlertLevel.Error)
                            break;
                    }
                }
                return result;
            }
        }
        class Key: StatusTreeNode {
            protected override void OnStateChanged(AlertLevel old, AlertLevel current) {
                if (Parent is Category) {
                    ((Category)Parent).ChildNewState(old, current);
                }
            }
        }
        class Value: StatusTreeNode {
            protected override void OnStateChanged(AlertLevel old, AlertLevel current) {
                switch (current) {
                    case AlertLevel.Ok:
                        ForeColor = Color.Green;
                        break;
                    case AlertLevel.Warning:
                        ForeColor = Color.Blue;
                        break;
                    case AlertLevel.Error:
                        ForeColor = Color.Red;
                        break;
                }
                if (Parent is Key) {
                    ((Key)Parent).State = current;
                }
            }
        }
    }
}
