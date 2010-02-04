using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    abstract public class MeasureMode
    {
        protected object locker = new object();
        private bool operating = false;
        public bool isOperating
        {
            get { return operating; }
            /*protected set
            {
                lock (locker)
                {
                    if (value != operating)
                    {
                        operating = value;
                        //toggleOperation();
                    }
                }
            }*/
        }
        public virtual void onUpdateCounts() 
        {
            operating = false;
        }
        public virtual void start()
        {
            operating = true;
        }
    }
}
