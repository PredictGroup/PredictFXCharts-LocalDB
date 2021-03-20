using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace PredictFXCharts
{
    static class ChartsManager
    {
        public static ObservableCollection<ChartStrategy> ChartStrategies = new ObservableCollection<ChartStrategy>();

        // **************************************************************************

        static _minStrategy _1minStrg;
        //static _minStrategy _2minStrg;
        static _minStrategy _3minStrg;
        //static _minStrategy _4minStrg;
        static _minStrategy _5minStrg;
        static _minStrategy _10minStrg;
        static _minStrategy _15minStrg;
        static _minStrategy _20minStrg;
        static _minStrategy _30minStrg;
        static _minStrategy _60minStrg;

        static _orderStrategy _diver;

        // **************************************************************************

        public static DataManager dm;
        public static TermManager tm;

        // **************************************************************************


        static ChartsManager()
        {
            // **********************************************************************

            _1minStrg = new _minStrategy(cfg.u._1minTimeFrameSize, 1, "_1minStrategy");
            //_2minStrg = new _minStrategy(cfg.u._2minTimeFrameSize, 2, "_2minStrategy");
            _3minStrg = new _minStrategy(cfg.u._3minTimeFrameSize, 3, "_3minStrategy");
            //_4minStrg = new _minStrategy(cfg.u._4minTimeFrameSize, 4, "_4minStrategy");
            _5minStrg = new _minStrategy(cfg.u._5minTimeFrameSize, 5, "_5minStrategy");
            _10minStrg = new _minStrategy(cfg.u._10minTimeFrameSize, 6, "_10minStrategy");
            _15minStrg = new _minStrategy(cfg.u._15minTimeFrameSize, 7, "_15minStrategy");
            _20minStrg = new _minStrategy(cfg.u._20minTimeFrameSize, 8, "_20minStrategy");
            _30minStrg = new _minStrategy(cfg.u._30minTimeFrameSize, 9, "_30minStrategy");
            _60minStrg = new _minStrategy(cfg.u._60minTimeFrameSize, 10, "_60minStrategy");

            _diver = new _orderStrategy();

            ChartStrategies.Add(_1minStrg);
            //ChartStrategies.Add(_2minStrg);
            ChartStrategies.Add(_3minStrg);
            //ChartStrategies.Add(_4minStrg);
            ChartStrategies.Add(_5minStrg);
            ChartStrategies.Add(_10minStrg);
            ChartStrategies.Add(_15minStrg);
            ChartStrategies.Add(_20minStrg);
            ChartStrategies.Add(_30minStrg);
            ChartStrategies.Add(_60minStrg);

            ChartStrategies.Add(_diver);
        }

        public static void SetDataManager(DataManager _dm)
        {
            dm = _dm;
        }

        public static void SetTermManager(TermManager _tm)
        {
            tm = _tm;
        }

        public static void Activate()
        {
            int i = 0;
            foreach (ChartStrategy strategy in ChartStrategies)
            {
                Activate(i);
                i++;
            }
        }

        public static void Deactivate()
        {
            int i = 0;
            foreach (ChartStrategy strategy in ChartStrategies)
            {
                Deactivate(i);
                i++;
            }
        }

        public static void Activate(int idx)
        {
            if (!ChartStrategies[idx].Active)
            {
                ChartStrategies[idx].Start();
                dm.TicksQueue.RegisterHandler(ChartStrategies[idx].ProcessTick);
                dm.SettingsQueue.RegisterHandler(ChartStrategies[idx].ProcessSetting);
                dm.TradesQueue.RegisterHandler(ChartStrategies[idx].ProcessTrade);
            }
        }

        public static void Deactivate(int idx)
        {
            if (ChartStrategies[idx].Active)
            {
                ChartStrategies[idx].Stop();
                dm.TicksQueue.UnregisterHandler(ChartStrategies[idx].ProcessTick);
                dm.SettingsQueue.UnregisterHandler(ChartStrategies[idx].ProcessSetting);
                dm.TradesQueue.UnregisterHandler(ChartStrategies[idx].ProcessTrade);
            }
        }
    }
}
