using StockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHub
{
    public class StockSimulator
    {
        private SecurityInfo si;
        private DateTime startDate;
        private DateTime endDate;
        private DataMonitor dataMonitor;
        private int inteval;
        private DataReceiver dataReceiver;
        private bool needSimulate;
        public StockSimulator(SecurityInfo _si, DateTime _startDate, DateTime _endDate, DataMonitor _da, DataReceiver _receiver, int _inteval,bool _needSimulate = false)
        {
            this.si = _si;
            this.startDate = _startDate;
            this.endDate = _endDate;
            this.dataMonitor = _da;
            this.inteval = _inteval;
            this.dataReceiver = _receiver;
            this.needSimulate = _needSimulate;
        }
        public void Start()
        {
            dataMonitor.Simulator(si, startDate, endDate, dataReceiver, inteval, needSimulate);
        }
    }
}
