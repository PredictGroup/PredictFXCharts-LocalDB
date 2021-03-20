
using System;
using System.Collections.Generic;
using System.Threading;

using PredictFXCharts.Market.Internals;

using QuikDdeConnector;

namespace PredictFXCharts.Market
{
    static class MarketProvider
    {
        // **********************************************************************

        public static double AskPrice { get; private set; }
        public static double BidPrice { get; private set; }


        public static readonly ProviderStatus StockStatus = new ProviderStatus("Биржевой стакан");
        public static readonly ProviderStatus TicksStatus = new ProviderStatus("Тики всех сделок");
        public static readonly ProviderStatus SettingsStatus = new ProviderStatus("Параметры инструментов");
        public static readonly ProviderStatus TradeStatus = new ProviderStatus("Сделки");

        public static event LastPriceHandler LastPriceHandler;

        public static IDataReceiver[] Receiver { get; private set; }

        // **********************************************************************

        static bool isReplayMode = false;
        static bool isNullTrader = false;

        // **********************************************************************

        static IConnector quikDde = new QuikDde();

        static IStockProvider stockProvider = quikDde as IStockProvider;

        static IConnector[] connectors = new IConnector[] {
                                                          quikDde};

        // **********************************************************************

        public static bool Activated { get; private set; }

        // **********************************************************************

        static MarketProvider()
        {
            foreach (IConnector connector in connectors)
            {
                IStockProvider sp = connector as IStockProvider;
                if (sp != null)
                {
                    sp.Connected += new StatusUpdateHandler(StockConnected);
                    sp.Disconnected += StockStatus.Disconnected;
                    sp.Broken += StockStatus.SetError;
                    sp.StockHandler += new StockHandler(StockHandler);
                }

                ITicksProvider tp = connector as ITicksProvider;
                if (tp != null)
                {
                    tp.Connected += TicksStatus.Connected;
                    tp.Disconnected += TicksStatus.Disconnected;
                    tp.Broken += TicksStatus.SetError;
                    tp.TickHandler += new TickHandler(TickHandler);
                }

                ISettingsProvider stp = connector as ISettingsProvider;
                if (stp != null)
                {
                    stp.Connected += SettingsStatus.Connected;
                    stp.Disconnected += SettingsStatus.Disconnected;
                    stp.Broken += SettingsStatus.SetError;
                    stp.SettingsHandler += new SettingsHandler(SettingsHandler);
                }

                ITradesProvider trp = connector as ITradesProvider;
                if (trp != null)
                {
                    trp.Connected += TradeStatus.Connected;
                    trp.Disconnected += TradeStatus.Disconnected;
                    trp.Broken += TradeStatus.SetError;
                    trp.TradesHandler += new TradesHandler(TradesHandler);
                }
            }

            Receiver = new IDataReceiver[1];
            Receiver[0] = new NullReceiver();

            Activated = false;
        }

        // **********************************************************************

        static void StockConnected(string text)
        {
            if (StockStatus.IsGood)
            {
                foreach (IDataReceiver rcvr in Receiver)
                {
                    rcvr.PutMessage(new Message(
                      "Внимание!\nВозможен запуск только одного экземпляра программы."));
                }
            }

            StockStatus.Connected(text);
        }

        // **********************************************************************

        static void StockHandler(Quote[] quotes, Spread spread)
        {
            StockStatus.DataReceived = DateTime.UtcNow;

            AskPrice = spread.Ask;
            BidPrice = spread.Bid;

            foreach (IDataReceiver rcvr in Receiver)
            {
                rcvr.PutStock(quotes, spread);
            }
        }

        // **********************************************************************

        static void TickHandler(Tick tick)
        {
            TicksStatus.DataReceived = DateTime.UtcNow;

            tick.IntPrice = tick.RawPrice; 

            if (LastPriceHandler != null)
                LastPriceHandler(tick.IntPrice);

            foreach (IDataReceiver rcvr in Receiver)
            {
                rcvr.PutTick(tick);
            }
        }

        // **********************************************************************
        static void SettingsHandler(Setting setting)
        {
            SettingsStatus.DataReceived = DateTime.UtcNow;

            foreach (IDataReceiver rcvr in Receiver)
            {
                rcvr.PutSetting(setting);
            }
        }

        // *********************************************************************

        static void TradesHandler(Trade trade)
        {
            TradeStatus.DataReceived = DateTime.UtcNow;

            foreach (IDataReceiver rcvr in Receiver)
            {
                rcvr.PutTrade(trade);
            }
        }

        // **********************************************************************

        public static void SetReceiver(IDataReceiver receiver)
        {
            int reciverIdxs = MarketProvider.Receiver.Length + 1;
            IDataReceiver[] tmpReciver = new IDataReceiver[reciverIdxs];
            MarketProvider.Receiver.CopyTo(tmpReciver, 0);
            MarketProvider.Receiver = tmpReciver;

            MarketProvider.Receiver[reciverIdxs - 1] = receiver;
        }

        // **********************************************************************

        public static void RemoveReceiver(IDataReceiver receiver)
        {
            IDataReceiver[] ReceiverTmp = new IDataReceiver[MarketProvider.Receiver.Length - 1];

            int reciverIdxs = 0;
            for (int i = 0; i < MarketProvider.Receiver.Length; i++)
            {
                if (MarketProvider.Receiver.GetValue(i).Equals(receiver))
                    continue;

                ReceiverTmp[reciverIdxs] = MarketProvider.Receiver[i];

                reciverIdxs++;
            }

            MarketProvider.Receiver = ReceiverTmp;
        }
        // **********************************************************************

        public static void Activate()
        {
            if (!isReplayMode)
            {
                foreach (IConnector connector in connectors)
                {
                    connector.Setup();

                    IStockProvider sp = connector as IStockProvider;
                    if (sp != null)
                    {
                        if (sp == stockProvider)
                            sp.Subscribe(cfg.u.SecCode);
                        else
                            sp.Unsubscribe();
                    }

                    ITicksProvider tp = connector as ITicksProvider;
                    if (tp != null)
                        tp.Subscribe();

                    ISettingsProvider stp = connector as ISettingsProvider;
                    if (stp != null)
                        stp.Subscribe();

                    ITradesProvider trp = connector as ITradesProvider;
                    if (trp != null)
                        trp.Subscribe();

                }
            }

            Activated = true;
        }

        // **********************************************************************

        public static void Deactivate()
        {
            foreach (IConnector connector in connectors)
            {
                IStockProvider sp = connector as IStockProvider;
                if (sp != null)
                    sp.Unsubscribe();

                ITicksProvider tp = connector as ITicksProvider;
                if (tp != null)
                    tp.Unsubscribe();

                ISettingsProvider stp = connector as ISettingsProvider;
                if (stp != null)
                    stp.Unsubscribe();

                ITradesProvider trp = connector as ITradesProvider;
                if (trp != null)
                    trp.Unsubscribe();
            }

            Activated = false;
        }

        // **********************************************************************

        public static void GetSecList(Action<SecList> callback)
        {
            ISecListProvider slp = quikDde as ISecListProvider;

            if (slp == null)
            {
                SecList list = new SecList();
                list.Error = "Отсутствует источник данных.";
                callback.Invoke(list);
            }
            else
            {
                Thread t = new Thread(() => { slp.GetSecList(callback); });
                t.Name = "GetSecList";
                t.IsBackground = true;
                t.Start();
            }
        }


        // **********************************************************************

        public static void SetMode(bool replay, bool nullTader)
        {
            if (isReplayMode != replay)
                if (isReplayMode = replay)
                    foreach (IConnector connector in connectors)
                    {
                        IStockProvider sp = connector as IStockProvider;
                        if (sp != null)
                            sp.Unsubscribe();

                        ITicksProvider tp = connector as ITicksProvider;
                        if (tp != null)
                            tp.Unsubscribe();

                        ISettingsProvider stp = connector as ISettingsProvider;
                        if (stp != null)
                            stp.Unsubscribe();

                        ITradesProvider trp = connector as ITradesProvider;
                        if (trp != null)
                            trp.Unsubscribe();
                    }

            isNullTrader = nullTader;

            Activate();
        }

        // **********************************************************************

        public static void PutMessage(Message message)
        {
            foreach (IDataReceiver rcvr in Receiver)
            {
                rcvr.PutMessage(message);
            }
        }

        // **********************************************************************

    }
}
