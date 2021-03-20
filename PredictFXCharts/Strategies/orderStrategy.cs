
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Runtime.CompilerServices; // MethodImpl


namespace PredictFXCharts
{
    public class _orderStrategy : ChartStrategy // strategy
    {
        TimeSpan lostConnectionInterval = new TimeSpan(0, 0, 10, 0, 000);


        protected static volatile consoledb2DataDataContext contextDb = new consoledb2DataDataContext();

        DispatcherTimer refreshing;
        static bool working;

        double lastPrice;

        public _orderStrategy(string StrategyName = "_orderStrategy")
            : base(StrategyName)
        {
            working = false;

            refreshing = new DispatcherTimer();
            refreshing.Interval = cfg.SbUpdateInterval;
            refreshing.Tick += new EventHandler(ProcessStrategy);

            refreshing.Start();

        }

        // **********************************************************************

        public override void ProcessTick(Tick tick)
        {
            lastPrice = tick.IntPrice;
        } // получение данных

        public override void ProcessSetting(Setting setting) { } // получение данных

        public override void ProcessTrade(Trade trade)
        {
            // получение данных из таблицы сделок
            while (working)
            { } // wait for order will be processed

            var order = (from o in contextDb.orders
                         where o.OrderNum == trade.OrderNum
                         && o.TradeNum == null
                         select o).FirstOrDefault();

            if (order == null)
            {
                //MessageBox.Show("В базе данных нет ордеров с номером - " + trade.OrderNum + ".", "Ошибка в программе " + cfg.FullProgName);
                return;
            }


            var posit = (from p in contextDb.positions
                         where p.stocks.ticker == trade.SecCode
                         && p.active == true
                         select p).FirstOrDefault();

            // open position
            if (trade.Op == TradeOp.Buy)
            {
                if (posit != null) // position
                {
                    if (posit.tradetypeid == 1) //bought
                    {
                        posit.priceopen = (posit.priceopen * posit.qty + trade.RawPrice * trade.Quantity) / (posit.qty + trade.Quantity);
                        posit.qty += trade.Quantity;
                    }
                    else
                        if (posit.tradetypeid == 2) //sold
                    {
                        if (posit.qty - trade.Quantity == 0) // close
                        {
                            posit.priceclose = trade.RawPrice;
                            posit.qty = 0;
                            posit.closed = trade.DateTime;
                            posit.active = false;
                        }
                        else // downgrade position qty
                        {
                            posit.priceopen = (posit.priceopen * posit.qty.Value + trade.RawPrice * trade.Quantity) / (posit.qty.Value + trade.Quantity);
                            posit.qty -= trade.Quantity;
                            if (posit.qty < 0)
                            {
                                posit.tradetypeid = 1; // если позиция перевернулась
                                posit.qty = posit.qty * -1;
                            }
                        }
                    }
                }
                else // new position
                {
                    positions positNew = new positions();
                    var stock = (from s in contextDb.stocks
                                 where s.ticker == trade.SecCode
                                 select s).FirstOrDefault();
                    positNew.stock = stock.id;
                    positNew.tradetypeid = 1;
                    positNew.opened = trade.DateTime;
                    positNew.priceopen = trade.RawPrice;
                    positNew.qty = trade.Quantity;
                    positNew.openqty = trade.Quantity;
                    positNew.active = true;
                    contextDb.positions.InsertOnSubmit(positNew);
                }
            }
            else
            //close position
            if (trade.Op == TradeOp.Sell)
            {
                if (posit != null) // position
                {
                    if (posit.tradetypeid == 1) //bought
                    {
                        if (posit.qty - trade.Quantity == 0) // close
                        {
                            posit.priceclose = trade.RawPrice;
                            posit.qty = 0;
                            posit.closed = trade.DateTime;
                            posit.active = false;
                        }
                        else // downgrade position qty
                        {
                            posit.priceopen = (posit.priceopen * posit.qty + trade.RawPrice * trade.Quantity) / (posit.qty + trade.Quantity);
                            posit.qty -= trade.Quantity;
                            if (posit.qty < 0)
                            {
                                posit.tradetypeid = 2; // если позиция перевернулась
                                posit.qty = posit.qty * -1;
                            }
                        }
                    }
                    else
                        if (posit.tradetypeid == 2) //sold
                    {
                        posit.priceopen = (posit.priceopen * posit.qty + trade.RawPrice * trade.Quantity) / (posit.qty + trade.Quantity);
                        posit.qty += trade.Quantity;
                    }
                }
                else // new position
                {
                    positions positNew = new positions();
                    var stock = (from s in contextDb.stocks
                                 where s.ticker == trade.SecCode
                                 select s).FirstOrDefault();
                    positNew.stock = stock.id;
                    positNew.tradetypeid = 2;
                    positNew.opened = trade.DateTime;
                    positNew.priceopen = trade.RawPrice;
                    positNew.qty = trade.Quantity;
                    positNew.openqty = trade.Quantity;
                    positNew.active = true;
                    contextDb.positions.InsertOnSubmit(positNew);
                }
            }
            else
            {
                MessageBox.Show("Транзакция из QUIK с типом - " + trade.Op.ToString() + " не реализована.", "Ошибка в программе " + cfg.FullProgName);
                return;
            }

            long trdNum = 0;
            Int64.TryParse(trade.TradeNum.ToString(), out trdNum);
            order.TradeNum = trdNum;

            contextDb.SubmitChanges();
        }

        // **********************************************************************

        void ProcessStrategy(object sender, EventArgs e)
        {
            if (working)
                return;

            working = true;

            if (ChartsManager.tm.Connected == TermConnection.None)
            {
                working = false;
                return;
            }

            var orders = from o in contextDb.orders
                         where o.active == true
                         orderby o.orderdatetime descending
                         select o;

            if (orders.Count() == 0)
            {
                working = false;
                return;
            }

            if (orders.Count() > 1)
            {
                MessageBox.Show("В базе данных слишком много ордеров количество - " + orders.Count().ToString() + ", должно быть - 1.", "Ошибка в программе " + cfg.FullProgName);
                working = false;
                return;
            }



            DateTime dtNow = DateTime.Now;

            var order = orders.First();

            if (order.stocks.ticker != cfg.u.SecCode)
            {
                working = false;
                return;
            }

            if (!isDateTimeValid(order.orderdatetime.Value)) // если данные не актуальные, возврат
            {
                working = false;
                return;
            }

            double priceValue = 0;
            if (order.price == 0)
            {
                if (lastPrice == 0)
                {
                    MessageBox.Show("В базе данных нет цены и lastPrice = 0.", "Ошибка в программе " + cfg.FullProgName);
                    working = false;
                    return;
                }
                else
                {
                    priceValue = lastPrice;
                    if (order.tradetypeid == 1)
                        priceValue = priceValue + cfg.u.PriceStep * 1;
                    else
                        if (order.tradetypeid == 2)
                        priceValue = priceValue - cfg.u.PriceStep * 1;
                }
            }
            else
            {
                priceValue = order.price.Value;
            }

            int qty = 0;
            if (order.qty == 0 || order.qty == null)
                qty = 1;
            else
                qty = (int)order.qty.Value;

            ChartsManager.tm.ExecAction(new OwnAction(
                                                          TradeOp.Cancel,
                                                          BaseQuote.None,
                                                          0,
                                                          0)); // отменяем ордер


            long OrderID = 0;
            switch (order.tradetypeid)
            {
                case 1: // buy

                    TermManager.Transaction tr = ChartsManager.tm.ExecAction(new OwnAction(
                                                                                              TradeOp.Buy,
                                                                                              BaseQuote.Absolute,
                                                                                              priceValue,
                                                                                              qty));
                    while (tr.OId == 0 || OrderID == 0)
                    {
                        OrderID = tr.OId;
                    }
                    break;

                case 2: // sell

                    TermManager.Transaction tr1 = ChartsManager.tm.ExecAction(new OwnAction(
                                                                                              TradeOp.Sell,
                                                                                              BaseQuote.Absolute,
                                                                                              priceValue,
                                                                                              qty));
                    while (tr1.OId == 0 || OrderID == 0)
                    {
                        OrderID = tr1.OId;
                    }
                    break;

                case 3: // close

                    {

                        var posit = (from p in contextDb.positions
                                     where p.active == true
                                     select p).FirstOrDefault();

                        if (posit == null)
                        {
                            working = false;
                            return;
                        }

                        if (posit.tradetypeid == 1) // buy -> need sell
                        {
                            priceValue = priceValue - cfg.u.PriceStep * 1;
                            TermManager.Transaction tr2 = ChartsManager.tm.ExecAction(new OwnAction(
                                                                                                      TradeOp.Sell,
                                                                                                      BaseQuote.Absolute,
                                                                                                      priceValue,
                                                                                                      qty));
                            while (tr2.OId == 0 || OrderID == 0)
                            {
                                OrderID = tr2.OId;
                            }
                        }
                        else if (posit.tradetypeid == 2) // sell -> need buy
                        {
                            priceValue = priceValue + cfg.u.PriceStep * 1;
                            TermManager.Transaction tr3 = ChartsManager.tm.ExecAction(new OwnAction(
                                                                                                      TradeOp.Buy,
                                                                                                      BaseQuote.Absolute,
                                                                                                      priceValue,
                                                                                                      qty));
                            while (tr3.OId == 0 || OrderID == 0)
                            {
                                OrderID = tr3.OId;
                            }
                        }
                    }

                    break;

                default:
                    MessageBox.Show("Тип ордера - не реализовано!", "Ошибка в программе " + cfg.FullProgName);
                    return;
                    break;
            }

            order.OrderNum = OrderID;
            order.executeddatetime = dtNow;
            order.active = false;

            contextDb.SubmitChanges();

            working = false;
        }


        // ------------------------------------------------------------

        bool isDateTimeValid(DateTime dt)
        {
            bool ret = false;

            // переданное время должно быть в диапазоне от -7 минут назад до текущего времени
            if (dt.Ticks >= DateTime.Now.Ticks - lostConnectionInterval.Ticks)
                ret = true; // валидно

            return ret;
        }


    }
}
