
using System;
using System.Windows;
using System.Windows.Threading;

namespace PredictFXCharts
{
    sealed class DataManager : IDataReceiver
    {
        DispatcherTimer refreshing;

        // **********************************************************************

        public DataQueue<Message> MsgQueue { get; private set; }
        public DataQueue<Spread> SpreadsQueue { get; private set; }
        public TicksQueue TicksQueue { get; private set; }
        public DataQueue<Setting> SettingsQueue { get; private set; }

        public DataQueue<Trade> TradesQueue { get; protected set; }
        public OrdersList OrdersList { get; protected set; }

        // **********************************************************************

        public DataManager()
        {
            MsgQueue = new DataQueue<Message>();
            SpreadsQueue = new DataQueue<Spread>();
            TicksQueue = new TicksQueue();
            SettingsQueue = new DataQueue<Setting>();

            TradesQueue = new DataQueue<Trade>();
            OrdersList = new OrdersList();

            refreshing = new DispatcherTimer();
            refreshing.Interval = cfg.RefreshInterval;
            refreshing.Tick += new EventHandler(RefreshTick);

            refreshing.Start();
        }

        // **********************************************************************

        void RefreshTick(object sender, EventArgs e)
        {

            if (MsgQueue.Length > 0)
                MsgQueue.Process();

            if (SpreadsQueue.Length > 0)
                SpreadsQueue.Process();

            if (TicksQueue.Length > 0)
                TicksQueue.Process();

            if (SettingsQueue.Length > 0)
                SettingsQueue.Process();

            if (TradesQueue.Length > 0)
                TradesQueue.Process();

            if (OrdersList.QueueLength > 0)
                OrdersList.UpdateHandlers();
        }

        // **********************************************************************

        public void PutMessage(Message msg)
        {
            MsgQueue.Enqueue(msg);
        }

        // **********************************************************************

        public void PutStock(Quote[] quotes, Spread spread) { }

        // **********************************************************************

        public void PutTick(Tick tick)
        {
            TicksQueue.Enqueue(tick);
        }

        // **********************************************************************

        public void PutSetting(Setting setting)
        {
            SettingsQueue.Enqueue(setting);
        }

        // **********************************************************************
        public void PutTrade(Trade trade)
        {
            TradesQueue.Enqueue(trade);
        }

        // **********************************************************************

        public void PutOwnOrder(OwnOrder order)
        {
            OrdersList.Enqueue(order);
        }

        // **********************************************************************

        public void PutPosition(int quantity, double price)
        {
            throw new NotImplementedException();
        }

        // **********************************************************************

    }
}
