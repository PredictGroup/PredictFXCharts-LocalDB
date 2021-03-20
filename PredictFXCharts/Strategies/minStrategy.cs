
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; // MethodImpl

namespace PredictFXCharts
{
    public class _minStrategy : ChartStrategy // minute strategy
    {

        Int64 stockID = 0;

        int currentTimeFrame = cfg.u._3minTimeFrameSize;
        int currentTimeFrameId = 3;

        // **********************************************************************

        protected static volatile IEnumerable<stocks> dbStocks;

        // **********************************************************************

        protected Dictionary<string, Dictionary<DateTime, StockDataElement>> volumeElements; // данные в потоке ticks
        protected Dictionary<string, Dictionary<DateTime, SettingDataElement>> settingElements;

        // **********************************************************************

        protected Dictionary<string, DateTime> lastDateTimeVolume;
        protected Dictionary<string, DateTime> lastDateTimeSetting;

        protected Dictionary<string, Dictionary<DateTime, StockDataElement>> VolumeVector { get { return volumeElements; } }
        protected Dictionary<string, Dictionary<DateTime, SettingDataElement>> SettingVector { get { return settingElements; } }

        // **********************************************************************

        protected static volatile consoledb2DataDataContext contextVol;
        protected static volatile consoledb2DataDataContext contextSet;
        protected static volatile consoledb2DataDataContext contextVaR;

        // **********************************************************************

        bool firstRound = true;

        bool firstLoopInCandle = true;

        bool lastCandleSend = false;

        // **********************************************************************

        DateTime lastTickDt;
        DateTime firstTickDt;

        // **********************************************************************
        protected DateTime lastProcessedDateTime = DateTime.MinValue;
        // **********************************************************************

        // **********************************************************************

        consoledb2DataDataContext cont1 = new consoledb2DataDataContext();

        // minutes - Id в таблице тайм фреймов
        public _minStrategy(int TimeFrameMinutes, int TimeFrameId, string StrategyName = "_3minStrategy")
            : base(StrategyName)
        {
            volumeElements = new Dictionary<string, Dictionary<DateTime, StockDataElement>>();
            settingElements = new Dictionary<string, Dictionary<DateTime, SettingDataElement>>();

            lastDateTimeVolume = new Dictionary<string, DateTime>();
            lastDateTimeSetting = new Dictionary<string, DateTime>();

            if (contextVol == null)
                contextVol = new consoledb2DataDataContext();
            if (contextSet == null)
                contextSet = new consoledb2DataDataContext();
            if (contextVaR == null)
                contextVaR = new consoledb2DataDataContext();

            currentTimeFrame = TimeFrameMinutes;
            currentTimeFrameId = TimeFrameId;

            firstTickDt = DateTime.Today.AddHours(10);
            lastTickDt = DateTime.Today.AddHours(23).AddMinutes(49).AddSeconds(59);

            // **********************************************************************
            dbStocks = (from s in contextVol.stocks
                        where s.active == true
                        select s).ToArray();

            // **********************************************************************
        }


        // **********************************************************************

        private async void createNewVolume(string SecCode, DateTime lastDT)
        {
            if (stockID == 0)
            {
                stockID = (from s in dbStocks
                           where s.ticker == SecCode
                           select s.id).FirstOrDefault();
            }

            bool found = false;

            DateTime lastCandleTime = DateTime.MinValue;
            if (lastDateTimeVolume.TryGetValue(SecCode, out lastCandleTime) == false)
            {
                // начальная установка даты и времени отсчета
                lastCandleTime = lastDT;
                lastDT = firstTickDt; // ставим на начало сессии

                lastDateTimeVolume[SecCode] = lastDT; // сохраняем
            }

            StockDataElement lastEl1 = new StockDataElement(SecCode, lastDT);

            lock (VolumeVector)
            {
                Dictionary<DateTime, StockDataElement> volumes = new Dictionary<DateTime, StockDataElement>();
                lock (volumes)
                {
                    if (VolumeVector.TryGetValue(SecCode, out volumes) == false)
                    {
                        volumes = new Dictionary<DateTime, StockDataElement>();
                        StockDataElement vds = new StockDataElement(SecCode, lastDT);

                        volumes[lastDT] = vds;
                        VolumeVector[SecCode] = volumes;
                    }
                    else
                    {
                        if (volumes.TryGetValue(lastCandleTime, out lastEl1) == true)
                            found = true;

                        StockDataElement vds = new StockDataElement(SecCode, lastDT);

                        volumes[lastDT] = vds;

                        VolumeVector[SecCode] = volumes;
                    }
                }
            }

            lastDateTimeVolume[SecCode] = lastDT;


            // запуск в отдельном процессе сохранения последнего элемента в базу
            if (found)
            {
                lastEl1.middle = lastEl1.PriceVolumeSum / lastEl1.Volume;
                lastEl1.date = lastCandleTime; // будет одной временной точкой
                await saveStockElementToDB(lastEl1);
                await createNew_VAT(SecCode);
            }
        }

        private async void createNewSetting(string SecCode, DateTime lastDT)
        {
            if (stockID == 0)
            {
                stockID = (from s in dbStocks
                           where s.ticker == SecCode
                           select s.id).FirstOrDefault();
            }

            bool found = false;

            DateTime lastCandleTime = DateTime.MinValue;
            if (lastDateTimeSetting.TryGetValue(SecCode, out lastCandleTime) == false)
            {
                // начальная установка даты и времени отсчета
                lastCandleTime = lastDT;
                lastDT = firstTickDt; // ставим на начало сессии

                lastDateTimeSetting[SecCode] = lastDT; // сохраняем!
            }

            SettingDataElement lastEl2 = new SettingDataElement(SecCode, lastDT);

            lock (SettingVector)
            {
                Dictionary<DateTime, SettingDataElement> settings = new Dictionary<DateTime, SettingDataElement>();
                lock (settings)
                {
                    if (SettingVector.TryGetValue(SecCode, out settings) == false) // ищем по текущему Коду SecCode есть ли уже значение settings оно одно для каждого элемента
                    {
                        settings = new Dictionary<DateTime, SettingDataElement>();
                        SettingDataElement sde = new SettingDataElement(SecCode, lastDT);
                        settings[lastDT] = sde;
                        SettingVector[SecCode] = settings;
                    }
                    else
                    {
                        if (settings.TryGetValue(lastCandleTime, out lastEl2) == true)
                            found = true;

                        SettingDataElement sde = new SettingDataElement(SecCode, lastDT);
                        settings[lastDT] = sde;
                        SettingVector[SecCode] = settings;

                    }
                }
            }

            lastDateTimeSetting[SecCode] = lastDT; // сохраняем!

            if (found)
            {
                lastEl2.DateTime = lastCandleTime; // будет одной временной точкой
                await saveSettingElementToDB(lastEl2);
            }

        }

        // **********************************************************************

        public override void ProcessTick(Tick tick)
        {
            if (tick.DateTime < firstTickDt)
                return;

            lock (VolumeVector)
            {
                DateTime lastDT = DateTime.MinValue;
                if (lastDateTimeVolume.TryGetValue(tick.SecCode, out lastDT) == false)
                {
                    // начальная установка даты и времени отсчета
                    lastDT = tick.DateTime;
                }

                Dictionary<DateTime, StockDataElement> volumes = new Dictionary<DateTime, StockDataElement>();
                if (VolumeVector.TryGetValue(tick.SecCode, out volumes) == false)
                {
                    createNewVolume(tick.SecCode, lastDT);

                    VolumeVector.TryGetValue(tick.SecCode, out volumes);
                }

                StockDataElement vds = new StockDataElement(tick.SecCode, lastDT);
                volumes.TryGetValue(lastDT, out vds);


                // если время больше XXX мин, делаем новую группу 
                long csTicks = currentTimeFrame * TimeSpan.TicksPerSecond;
                if (tick.DateTime.Ticks - lastDT.Ticks >= csTicks)
                { // новая группа 

                    //--------------------

                    //+ old
                    lastDT = tick.DateTime;
                    createNewVolume(tick.SecCode, lastDT);
                    volumes.TryGetValue(lastDT, out vds);
                    //- old

                    vds.high = tick.IntPrice;
                    vds.low = tick.IntPrice;

                    vds.open = tick.IntPrice;
                    vds.close = tick.IntPrice;

                    firstLoopInCandle = true;
                }

                if (firstRound)
                {
                    vds.high = tick.IntPrice;
                    vds.low = tick.IntPrice;

                    vds.open = tick.IntPrice;
                    vds.close = tick.IntPrice;
                }
                //+ 02082015 Bug fix
                if (vds.open == 0)
                    vds.open = tick.IntPrice;
                //+ 02082015 Bug fix

                if (tick.IntPrice > vds.high)
                    vds.high = tick.IntPrice;

                if (tick.IntPrice < vds.low)
                    vds.low = tick.IntPrice;

                vds.close = tick.IntPrice;

                vds.Volume += tick.Volume;
                vds.PriceVolumeSum += (tick.IntPrice * tick.Volume);

                if (tick.Op == TradeOp.Sell)
                {
                    vds.VolumeSell += tick.Volume;
                }
                else
                {
                    vds.VolumeBuy += tick.Volume;
                }

                firstRound = false;
                firstLoopInCandle = false;

                volumes[lastDT] = vds;
                VolumeVector[tick.SecCode] = volumes;

                // Save last tick
                if (vds.date >= lastTickDt && !lastCandleSend)
                {
                    vds.middle = vds.PriceVolumeSum / vds.Volume;
                    saveStockElementToDB(vds);

                    lastCandleSend = true;
                }
            }
        }


        // **********************************************************************

        public override void ProcessSetting(Setting setting)
        {
            if (setting.DateTime < firstTickDt)
                return;

            lock (SettingVector)
            {
                string SecCode = setting.SecCode;

                DateTime lastDT = DateTime.MinValue;
                if (lastDateTimeSetting.TryGetValue(SecCode, out lastDT) == false)
                {
                    // начальная установка даты и времени отсчета
                    lastDT = setting.DateTime;
                }

                Dictionary<DateTime, SettingDataElement> settings = new Dictionary<DateTime, SettingDataElement>();
                if (SettingVector.TryGetValue(SecCode, out settings) == false)
                {
                    createNewSetting(SecCode, lastDT);

                    SettingVector.TryGetValue(SecCode, out settings);
                }

                SettingDataElement sde = new SettingDataElement(SecCode, lastDT);
                settings.TryGetValue(lastDT, out sde);

                // если время больше XXX мин, делаем новую группу 
                long csTicks = currentTimeFrame * TimeSpan.TicksPerSecond;
                if (setting.DateTime.Ticks - lastDT.Ticks >= csTicks)
                { // новая группа 

                    lastDT = setting.DateTime;
                    createNewSetting(SecCode, lastDT);
                    settings.TryGetValue(lastDT, out sde);
                }

                sde.ClassCode = setting.ClassCode; // код класса
                sde.ClassName = setting.ClassName; // класс бумаги
                sde.OptionBase = setting.OptionBase; // +1 базовый актив опциона
                sde.MatDate = setting.MatDate; // +1 дата погашения
                sde.DaysToMatDate = setting.DaysToMatDate; // дней до погашения

                if (sde.NumbidsOpen == 0)
                    sde.NumbidsOpen = setting.Numbids;
                if (sde.NumbidsHigh == 0)
                    sde.NumbidsHigh = setting.Numbids;
                else
                    if (sde.NumbidsHigh < setting.Numbids)
                    sde.NumbidsHigh = setting.Numbids;
                if (sde.NumbidsLow == 0)
                    sde.NumbidsLow = setting.Numbids;
                else
                    if (sde.NumbidsLow > setting.Numbids)
                    sde.NumbidsLow = setting.Numbids;
                sde.NumbidsClose = setting.Numbids;

                if (sde.NumoffersOpen == 0)
                    sde.NumoffersOpen = setting.Numoffers;
                if (sde.NumoffersHigh == 0)
                    sde.NumoffersHigh = setting.Numoffers;
                else
                    if (sde.NumoffersHigh < setting.Numoffers)
                    sde.NumoffersHigh = setting.Numoffers;
                if (sde.NumoffersLow == 0)
                    sde.NumoffersLow = setting.Numoffers;
                else
                    if (sde.NumoffersLow > setting.Numoffers)
                    sde.NumoffersLow = setting.Numoffers;
                sde.NumoffersClose = setting.Numoffers;

                if (sde.BiddepthtOpen == 0)
                    sde.BiddepthtOpen = setting.Biddeptht;
                if (sde.BiddepthtHigh == 0)
                    sde.BiddepthtHigh = setting.Biddeptht;
                else
                    if (sde.BiddepthtHigh < setting.Biddeptht)
                    sde.BiddepthtHigh = setting.Biddeptht;
                if (sde.BiddepthtLow == 0)
                    sde.BiddepthtLow = setting.Biddeptht;
                else
                    if (sde.BiddepthtLow > setting.Biddeptht)
                    sde.BiddepthtLow = setting.Biddeptht;
                sde.BiddepthtClose = setting.Biddeptht;

                if (sde.OfferdepthtOpen == 0)
                    sde.OfferdepthtOpen = setting.Offerdeptht;
                if (sde.OfferdepthtHigh == 0)
                    sde.OfferdepthtHigh = setting.Offerdeptht;
                else
                    if (sde.OfferdepthtHigh < setting.Offerdeptht)
                    sde.OfferdepthtHigh = setting.Offerdeptht;
                if (sde.OfferdepthtLow == 0)
                    sde.OfferdepthtLow = setting.Offerdeptht;
                else
                    if (sde.OfferdepthtLow > setting.Offerdeptht)
                    sde.OfferdepthtLow = setting.Offerdeptht;
                sde.OfferdepthtClose = setting.Offerdeptht;

                sde.Voltoday = setting.Voltoday; // количество во всех сделках 
                sde.Valtoday = setting.Valtoday; // текущий оборот в деньгах
                sde.Numtrades = setting.Numtrades; // количество сделок за сегодня

                if (sde.NumcontractsOpen == 0)
                    sde.NumcontractsOpen = setting.Numcontracts;
                if (sde.NumcontractsHigh == 0)
                    sde.NumcontractsHigh = setting.Numcontracts;
                else
                    if (sde.NumcontractsHigh < setting.Numcontracts)
                    sde.NumcontractsHigh = setting.Numcontracts;
                if (sde.NumcontractsLow == 0)
                    sde.NumcontractsLow = setting.Numcontracts;
                else
                    if (sde.NumcontractsLow > setting.Numcontracts)
                    sde.NumcontractsLow = setting.Numcontracts;
                sde.NumcontractsClose = setting.Numcontracts;

                sde.Selldepo = setting.Selldepo; // ГО продавца
                sde.Buydepo = setting.Buydepo; // ГО покупателя
                sde.Strike = setting.Strike; // +1 цена страйк
                sde.OptionType = setting.OptionType; // +1 тип опциона CALL PUT
                sde.Volatility = setting.Volatility; // +1 волатильность опциона

                settings[lastDT] = sde;
                SettingVector[SecCode] = settings;
            }
        }

        // **********************************************************************
        [MethodImpl(MethodImplOptions.Synchronized)]
        private Task createNew_VAT(string SecCode)
        {
            return Task.Run(() =>
            {

                lock (contextVaR)
                {

                    // 2. Value-At-Risk

                    var lastVaR = (from v in contextVaR.valueatrisk
                                   where v.stock == stockID
                                   && v.timeframe == currentTimeFrameId
                                   orderby v.datetime descending
                                   select v).FirstOrDefault();

                    Int64 varIDforAllSelect = 0;
                    Int64 lastVarQuoteID = 0;
                    DateTime lastVarDateTime = DateTime.Today;
                    if (lastVaR != null)
                    {
                        varIDforAllSelect = 1000;
                        if (lastVaR.volquoteid < 1000)
                            varIDforAllSelect = lastVaR.volquoteid;

                        lastVarQuoteID = lastVaR.volquoteid;
                        lastVarDateTime = lastVaR.datetime.Value;
                    }

                    var allQuotes2_tmp = from q in contextVaR.volquotes
                                         where q.stock == stockID
                                         && q.timeframe == currentTimeFrameId
                                         && q.id > varIDforAllSelect
                                         orderby q.id ascending
                                         select q;
                    var allQuotes2 = allQuotes2_tmp.ToArray();

                    var countQuotes2_tmp = from q in contextVaR.volquotes
                                           where q.stock == stockID
                                           && q.timeframe == currentTimeFrameId
                                           && q.id > lastVarQuoteID
                                           orderby q.id ascending
                                           select q;

                    var setting_tmp = from s in contextVaR.settingquotes
                                      where s.stock == stockID
                                      && s.timeframe == currentTimeFrameId
                                      && s.datetime > lastVarDateTime
                                      select s;
                    if (setting_tmp.Count() == 0)
                        return;

                    foreach (volquotes quote in countQuotes2_tmp)
                    {
                        var allQuotes3 = from q in allQuotes2
                                         where q.datetime <= quote.datetime
                                         orderby q.datetime ascending
                                         select q;
                        double[] close = new double[allQuotes3.Count()];
                        int i = 0;
                        foreach (volquotes quote2 in allQuotes3)
                        {
                            close[i] = quote2.close.Value;
                            i++;
                        }

                        var setting = (from s in setting_tmp
                                       where s.stock == stockID
                                       && s.datetime <= quote.datetime
                                       select s).FirstOrDefault();
                        if (setting == null)
                            continue; // не создаем Value At Risk
                        double ValueSum = setting.buydepo.Value;

                        int begIdx = 0;
                        int NBElement = 0;
                        double[] real = null;

                        ValueAtRiskUtils.stdDev((close.Count() - 35), (close.Count() - 1), close, 10, 1, out begIdx, out NBElement, out real);

                        int time = 1; // 1 день

                        double W = real[34] / close[close.Count() - 1];
                        double VaR = ValueAtRiskUtils.getStockVaR(W, ValueSum, time);

                        valueatrisk vAr = new valueatrisk();
                        vAr.stock = stockID;
                        vAr.timeframe = currentTimeFrameId;
                        vAr.datetime = quote.datetime;
                        vAr.volquoteid = quote.id;
                        vAr.value = VaR;
                        vAr.buydepo = ValueSum;
                        contextVaR.valueatrisk.InsertOnSubmit(vAr);
                        contextVaR.SubmitChanges();
                    }

                }

            });

        }


        // **********************************************************************
        // **********************************************************************

        async void saveAllVolumeDataToDB()
        {
            foreach (var values in VolumeVector.Values)
            {
                foreach (StockDataElement element in values.Values)
                {
                    await saveStockElementToDB(element);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Task saveStockElementToDB(StockDataElement element)
        {
            return Task.Run(() =>
            {
                lock (contextVol)
                {
                    //Int64 stockID = (from s in dbStocks
                    //                 where s.ticker == element.ticker
                    //                 select s.id).FirstOrDefault();
                    //if (stockID == 0)
                    //{
                    //    stocks ns = new stocks();
                    //    ns.ticker = element.ticker;
                    //    ns.enddate = null;
                    //    ns.pricestep = 100;
                    //    ns.active = true;
                    //    contextVol.stocks.InsertOnSubmit(ns);
                    //    contextVol.SubmitChanges();

                    //    dbStocks = from s in contextVol.stocks
                    //               where s.active == true
                    //               select s;
                    //    stockID = (from s in dbStocks
                    //               where s.ticker == element.ticker
                    //               select s.id).FirstOrDefault();
                    //}

                    volquotes nq = new volquotes();

                    nq.stock = stockID;
                    nq.timeframe = currentTimeFrameId;
                    nq.datetime = element.date;

                    nq.high = element.high;
                    nq.low = element.low;

                    nq.open = element.open;
                    nq.close = element.close;

                    nq.middle = element.middle;

                    nq.volume = element.Volume;
                    nq.volumebuy = element.VolumeBuy;
                    nq.volumesell = element.VolumeSell;

                    contextVol.volquotes.InsertOnSubmit(nq);
                    contextVol.SubmitChanges();
                }

            });
        }

        // **********************************************************************

        async void saveAllSettingDataToDB()
        {
            foreach (var values in SettingVector.Values)
            {
                foreach (SettingDataElement element in values.Values)
                {
                    await saveSettingElementToDB(element);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Task saveSettingElementToDB(SettingDataElement element)
        {
            return Task.Run(() =>
            {
                if (element.MatDate == null)
                    return;

                lock (contextSet)
                {
                    //Int64 stockID = (from s in dbStocks
                    //                 where s.ticker == element.SecCode
                    //                 select s.id).FirstOrDefault();
                    //if (stockID == 0)
                    //{
                    //    stocks ns = new stocks();
                    //    ns.ticker = element.SecCode;
                    //    ns.enddate = null;
                    //    ns.pricestep = 50;
                    //    ns.active = true;
                    //    contextSet.stocks.InsertOnSubmit(ns);
                    //    contextSet.SubmitChanges();

                    //    dbStocks = from s in contextSet.stocks
                    //               where s.active == true
                    //               select s;
                    //    stockID = (from s in dbStocks
                    //               where s.ticker == element.SecCode
                    //               select s.id).FirstOrDefault();
                    //}

                    settingquotes nq = new settingquotes();

                    nq.stock = stockID;
                    nq.timeframe = currentTimeFrameId;
                    nq.datetime = element.DateTime;

                    nq.seccode = element.SecCode; // код бумаги
                    nq.classcode = element.ClassCode; // код класса
                    nq.classname = element.ClassName; // класс бумаги
                    nq.optionbase = element.OptionBase; // +1 базовый актив опциона
                    nq.matdate = element.MatDate; // +1 дата погашения
                    nq.daystomatdate = element.DaysToMatDate; // дней до погашения

                    nq.numbidsopen = element.NumbidsOpen; // количество заявкок на покупку
                    nq.numbidshigh = element.NumbidsHigh; // количество заявкок на покупку
                    nq.numbidslow = element.NumbidsLow; // количество заявкок на покупку
                    nq.numbidsclose = element.NumbidsClose; // количество заявкок на покупку

                    nq.numoffersopen = element.NumoffersOpen; // количество заявок на продажу
                    nq.numoffershigh = element.NumoffersHigh; // количество заявок на продажу
                    nq.numofferslow = element.NumoffersLow; // количество заявок на продажу
                    nq.numoffersclose = element.NumoffersClose; // количество заявок на продажу

                    nq.biddepthtopen = element.BiddepthtOpen; // суммарный спрос контрактов
                    nq.biddepththigh = element.BiddepthtHigh; // суммарный спрос контрактов
                    nq.biddepthtlow = element.BiddepthtLow; // суммарный спрос контрактов
                    nq.biddepthtclose = element.BiddepthtClose; // суммарный спрос контрактов

                    nq.offerdepthtopen = element.OfferdepthtOpen; // суммарное предложение контрактов
                    nq.offerdepththigh = element.OfferdepthtHigh; // суммарное предложение контрактов
                    nq.offerdepthtlow = element.OfferdepthtLow; // суммарное предложение контрактов
                    nq.offerdepthtclose = element.OfferdepthtClose; // суммарное предложение контрактов

                    nq.voltoday = element.Voltoday; // количество во всех сделках 
                    nq.valtoday = element.Valtoday; // текущий оборот в деньгах
                    nq.numtrades = element.Numtrades; // количество сделок за сегодня

                    nq.numcontractsopen = element.NumcontractsOpen; // количество открытых позиций (Открытый Интерес)
                    nq.numcontractshigh = element.NumcontractsHigh; // количество открытых позиций (Открытый Интерес)
                    nq.numcontractslow = element.NumcontractsLow; // количество открытых позиций (Открытый Интерес)
                    nq.numcontractsclose = element.NumcontractsClose; // количество открытых позиций (Открытый Интерес)

                    nq.selldepo = element.Selldepo; // ГО продавца
                    nq.buydepo = element.Buydepo; // ГО покупателя
                    nq.strike = element.Strike; // +1 цена страйк
                    nq.optiontype = element.OptionType; // +1 тип опциона CALL PUT
                    nq.volatility = element.Volatility; // +1 волатильность опциона


                    contextSet.settingquotes.InsertOnSubmit(nq);
                    contextSet.SubmitChanges();
                }

            });
        }

        // **********************************************************************

        public override void ProcessTrade(Trade trade) { }

    }
}
