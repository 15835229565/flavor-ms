using System;
using System.Drawing;
using System.Windows.Forms;

namespace Flavor.Forms {
    abstract class StatusTreeNode: TreeNode {
        public enum States {
            Ok,
            Warning,
            Error
        }
        States myState = States.Ok;
        public States State {
            protected get { return myState; }
            set {
                if (myState != value) {
                    OnPreNewState(value);
                    myState = value;
                }
            }
        }
        protected virtual void OnPreNewState(States state) { }
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
            protected override void OnPreNewState(States state) {
                switch (state) {
                    case States.Ok:
                        StateImageKey = "";
                        break;
                    case States.Warning:
                        StateImageKey = "warning";
                        break;
                    case States.Error:
                        StateImageKey = "error";
                        break;
                }
                if (Parent is Category) {
                    ((Category)Parent).ChildNewState(State, state);
                }
            }
            void ChildNewState(States previous, States current) {
                if (State < previous) {
                    // illegal state
                    throw new InvalidOperationException();
                }
                if (State < current) {
                    State = current;
                    return;
                }
                if (State > current) {
                    if (previous < current) {
                        return;
                    }
                    State = ComputeStateFromChildren(current);
                }
            }
            States ComputeStateFromChildren(States hint) {
                States result = hint;
                foreach (StatusTreeNode node in Nodes) {
                    if (result < node.State) {
                        result = node.State;
                        if (result == States.Error)
                            return result;
                    }
                }
                return result;
            }
        }
        class Key: StatusTreeNode { }
        class Value: StatusTreeNode {
            protected override void OnPreNewState(States state) {
                switch (state) {
                    case States.Ok:
                        ForeColor = Color.Green;
                        break;
                    case States.Warning:
                        ForeColor = Color.Blue;
                        break;
                    case States.Error:
                        ForeColor = Color.Red;
                        break;
                }
                if (Parent is Key) {
                    ((Key)Parent).State = state;
                }
            }
        }
    }
}
