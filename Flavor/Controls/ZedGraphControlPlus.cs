using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ZedGraph;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls
{
    public partial class ZedGraphControlPlus: ZedGraphControl {
        public class ContextMenuBuilderEventArgs: EventArgs {
            private ContextMenuStrip menuStrip;
            public ContextMenuStrip MenuStrip {
                get {
                    return this.menuStrip;
                }
            }
            private ContextMenuObjectState objState;
            private bool isNearPoint;
            public bool IsNearPoint {
                get {
                    return this.isNearPoint;
                }
            }
            public ContextMenuBuilderEventArgs(ContextMenuStrip menuStrip, ContextMenuObjectState objState, bool isNearPoint) {
                this.menuStrip = menuStrip;
                this.objState = objState;
                this.isNearPoint = isNearPoint;
            }
        }
        public delegate void DiffOnPointEventHandler(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference);
        public event DiffOnPointEventHandler OnDiffOnPoint;
        
        public new event EventHandler<ContextMenuBuilderEventArgs> ContextMenuBuilder;

        public delegate void PointEventHandler(ushort step, byte colNumber);
        public event PointEventHandler OnPoint;

        private bool isFirstCollector;
        public bool IsFirstCollector {
            set { isFirstCollector = value; }
        }

        private ToolStripMenuItem pointItem;
        private ToolStripMenuItem peakItem;

        public ZedGraphControlPlus()
            : base() {
            InitializeComponent();
            base.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(ZedGraphControlPlus_ContextMenuBuilder);
        }

        private void ZedGraphControlPlus_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState) {
            if (sender != this)
                return;
            GraphPane pane = MasterPane.FindChartRect(mousePt);
            CurveItem nearestCurve;
            bool isNearPoint = false;
            int pointIndex;
            ToolStripItemCollection items = menuStrip.Items;

            if ((pane != null) && pane.FindNearestPoint(mousePt, out nearestCurve, out pointIndex)) {
                isNearPoint = true;

                ToolStripItem item = new ToolStripSeparator();
                items.Add(item);

                {
                    // can be NullPointerExceptions here..
                    Graph.pListScaled pls = (nearestCurve.Points as PointPairListPlus).PLSreference;
                    ushort step = (ushort)pls.Step[pointIndex].X;
                    byte isFirst = isFirstCollector ? (byte)1 : (byte)2;

                    item = new ToolStripMenuItem();
                    item.Text = "Добавить точку в редактор";
                    item.Click += new System.EventHandler((s, e) => {
                        // TODO: raise event here and move code below to mainform
                        new AddPointForm(step, isFirst).ShowDialog();
                    });
                    items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Коэффициент коллектора" + (isFirstCollector ? " 1" : " 2");
                    item.Click += new System.EventHandler((s, e) => {
                        // TODO: raise event here and move code below to CollectorsForm
                        OnPoint(step, isFirst);
                        //new SetScalingCoeffForm(step, isFirst).ShowDialog();
                    });
                    items.Add(item);

                    {
                        // can be NullPointerExceptions here..
                        Utility.PreciseEditorData ped = pls.PEDreference;

                        pointItem = new ToolStripMenuItem();
                        pointItem.Visible = false;
                        pointItem.Text = "Вычесть из текущего с перенормировкой на точку";
                        pointItem.Click += new System.EventHandler((s, e) => {
                            OnDiffOnPoint(step, pls, ped);
                        });
                        items.Add(pointItem);

                        peakItem = new ToolStripMenuItem();
                        peakItem.Visible = false;
                        peakItem.Text = "Вычесть из текущего с перенормировкой на интеграл пика";
                        peakItem.Click += new System.EventHandler((s, e) => {
                            OnDiffOnPoint(ushort.MaxValue, null, ped);
                        });
                        items.Add(peakItem);
                    }
                }
            }
            //raise new event
            if (ContextMenuBuilder != null)
                ContextMenuBuilder(this, new ContextMenuBuilderEventArgs(menuStrip, objState, isNearPoint));
        }
        internal void setVisibility(bool pointItemVisible, bool peakItemVisible) {
            pointItem.Visible = pointItemVisible;
            peakItem.Visible = peakItemVisible;
        }
    }
}

