
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using PredictFXCharts;
using PredictFXCharts.Market;
using QuikDdeConnector.Internals;
using XlDde;

namespace QuikDdeConnector
{
    sealed class QuikDde : IStockProvider, ITicksProvider, ISettingsProvider, ITradesProvider, ISecListProvider
    {
        // **********************************************************************

        StockChannel stockChannel = new StockChannel();
        TicksChannel ticksChannel = new TicksChannel();
        SettingsChannel settingsChannel = new SettingsChannel();
        TradesChannel tradesChannel = new TradesChannel();

        bool stockActive;
        bool ticksActive;
        bool settingsActive;
        bool tradesActive;

        string service;
        XlDdeServer server;

        StatusUpdateHandler errorHandler;

        // **********************************************************************

        void CreateServer()
        {
            if (server == null)
                try
                {
                    server = new XlDdeServer(service);
                    server.Register();
                }
                catch (Exception e)
                {
                    if (errorHandler != null)
                        errorHandler.Invoke("Ошибка создания сервера DDE: " + e.Message);
                }
        }

        // **********************************************************************

        void DisposeServer()
        {
            if (server != null)
            {
                try
                {
                    server.Disconnect();
                    server.Dispose();
                }
                catch (Exception e)
                {
                    if (errorHandler != null)
                        errorHandler.Invoke("Ошибка удаления сервера DDE: " + e.Message);
                }

                server = null;

                stockActive = false;
                ticksActive = false;
                tradesActive = false;
                tradesActive = false;
            }
        }

        // **********************************************************************

        string IConnector.Name { get { return "Quik DDE"; } }

        // **********************************************************************

        void IConnector.Setup()
        {
            if (service != cfg.u.DdeServerName)
            {
                DisposeServer();
                service = cfg.u.DdeServerName;
            }
        }

        // **********************************************************************

        event StatusUpdateHandler IStockProvider.Connected
        {
            add { stockChannel.Connected += value; }
            remove { stockChannel.Connected -= value; }
        }

        event StatusUpdateHandler IStockProvider.Disconnected
        {
            add { stockChannel.Disconnected += value; }
            remove { stockChannel.Disconnected -= value; }
        }

        event StatusUpdateHandler IStockProvider.Broken
        {
            add { stockChannel.Broken += value; errorHandler += value; }
            remove { stockChannel.Broken -= value; errorHandler -= value; }
        }

        event StockHandler IStockProvider.StockHandler
        {
            add { stockChannel.StockHandler += value; }
            remove { stockChannel.StockHandler -= value; }
        }

        // **********************************************************************

        void IStockProvider.Subscribe(string symbol)
        {
            CreateServer();

            if (!stockActive && server != null)
            {
                stockActive = true;
                server.AddChannel(stockChannel);
            }
        }

        // **********************************************************************

        void IStockProvider.Unsubscribe()
        {
            if (stockActive && server != null)
            {
                stockActive = false;
                server.RmvChannel(stockChannel);

                if (!ticksActive)
                    DisposeServer();
            }
        }

        // **********************************************************************

        event StatusUpdateHandler ITicksProvider.Connected
        {
            add { ticksChannel.Connected += value; }
            remove { ticksChannel.Connected -= value; }
        }

        event StatusUpdateHandler ITicksProvider.Disconnected
        {
            add { ticksChannel.Disconnected += value; }
            remove { ticksChannel.Disconnected -= value; }
        }

        event StatusUpdateHandler ITicksProvider.Broken
        {
            add { ticksChannel.Broken += value; errorHandler += value; }
            remove { ticksChannel.Broken -= value; errorHandler -= value; }
        }

        event TickHandler ITicksProvider.TickHandler
        {
            add { ticksChannel.TickHandler += value; }
            remove { ticksChannel.TickHandler -= value; }
        }

        // **********************************************************************

        void ITicksProvider.Subscribe()
        {
            CreateServer();

            if (!ticksActive && server != null)
            {
                ticksActive = true;
                server.AddChannel(ticksChannel);
            }
        }

        // **********************************************************************

        void ITicksProvider.Unsubscribe()
        {
            if (ticksActive && server != null)
            {
                ticksActive = false;
                server.RmvChannel(ticksChannel);

                if (!stockActive)
                    DisposeServer();
            }
        }

        // **********************************************************************

        event StatusUpdateHandler ISettingsProvider.Connected
        {
            add { settingsChannel.Connected += value; }
            remove { settingsChannel.Connected -= value; }
        }

        event StatusUpdateHandler ISettingsProvider.Disconnected
        {
            add { settingsChannel.Disconnected += value; }
            remove { settingsChannel.Disconnected -= value; }
        }

        event StatusUpdateHandler ISettingsProvider.Broken
        {
            add { settingsChannel.Broken += value; errorHandler += value; }
            remove { settingsChannel.Broken -= value; errorHandler -= value; }
        }

        event SettingsHandler ISettingsProvider.SettingsHandler
        {
            add { settingsChannel.SettingsHandler += value; }
            remove { settingsChannel.SettingsHandler -= value; }
        }

        // **********************************************************************

        void ISettingsProvider.Subscribe()
        {
            CreateServer();

            if (!settingsActive && server != null)
            {
                settingsActive = true;
                server.AddChannel(settingsChannel);
            }
        }

        // **********************************************************************

        void ISettingsProvider.Unsubscribe()
        {
            if (settingsActive && server != null)
            {
                settingsActive = false;
                server.RmvChannel(settingsChannel);

                if (!settingsActive)
                    DisposeServer();
            }
        }

        // **********************************************************************
        // **********************************************************************

        event StatusUpdateHandler ITradesProvider.Connected
        {
            add { tradesChannel.Connected += value; }
            remove { tradesChannel.Connected -= value; }
        }

        event StatusUpdateHandler ITradesProvider.Disconnected
        {
            add { tradesChannel.Disconnected += value; }
            remove { tradesChannel.Disconnected -= value; }
        }

        event StatusUpdateHandler ITradesProvider.Broken
        {
            add { tradesChannel.Broken += value; errorHandler += value; }
            remove { tradesChannel.Broken -= value; errorHandler -= value; }
        }

        event TradesHandler ITradesProvider.TradesHandler
        {
            add { tradesChannel.TradesHandler += value; }
            remove { tradesChannel.TradesHandler -= value; }
        }

        // **********************************************************************

        void ITradesProvider.Subscribe()
        {
            CreateServer();

            if (!tradesActive && server != null)
            {
                tradesActive = true;
                server.AddChannel(tradesChannel);
            }
        }

        // **********************************************************************

        void ITradesProvider.Unsubscribe()
        {
            if (tradesActive && server != null)
            {
                tradesActive = false;
                server.RmvChannel(tradesChannel);

                if (!tradesActive)
                    DisposeServer();
            }
        }

        // **********************************************************************

        void ISecListProvider.GetSecList(Action<SecList> callback)
        {
            const string secListFileName = "seclist.csv";

            const int classNameIndex = 0;
            const int classCodeIndex = 1;
            const int secNameIndex = 2;
            const int secCodeIndex = 3;
            const int priceStepIndex = 4;

            SecList secList = new SecList();

            try
            {
                using (StreamReader stream = new StreamReader(cfg.AsmPath + secListFileName))
                {
                    char[] delimiter = new char[] { ';' };
                    string line;

                    while ((line = stream.ReadLine()) != null)
                    {
                        string[] str = line.Split(delimiter);
                        double step;

                        if (str.Length < 5 || !double.TryParse(str[priceStepIndex],
                          NumberStyles.Float, NumberFormatInfo.InvariantInfo, out step))
                            throw new FormatException("Неверный формат файла.");

                        secList.Add(str[secNameIndex], str[secCodeIndex],
                          str[classNameIndex], str[classCodeIndex], step);
                    }
                }
            }
            catch (Exception e)
            {
                secList.Error = e.Message;
            }

            callback.Invoke(secList);
        }

        // **********************************************************************
    }
}
