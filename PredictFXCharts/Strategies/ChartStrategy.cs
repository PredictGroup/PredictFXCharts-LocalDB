
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictFXCharts
{
    public abstract class ChartStrategy
    {
        
        private string _name;
        private bool _active;


        protected Dictionary<DateTime, TradeOp> results;

        // **********************************************************************

        protected const string uaDescr = "User";

        // **********************************************************************

        public ChartStrategy(string name)
        {
            _name = name;

            _active = false;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool Active
        {
            get
            {
                return _active;
            }
        }

        // **********************************************************************

        public abstract void ProcessTick(Tick tick); // получение данных

        public abstract void ProcessSetting(Setting setting); // получение данных

        public abstract void ProcessTrade(Trade trade); // получение сделок

        // **********************************************************************

        public void Start()
        {
            _active = true;
        }

        public void Stop()
        {
            _active = false;
        }
    }
}
