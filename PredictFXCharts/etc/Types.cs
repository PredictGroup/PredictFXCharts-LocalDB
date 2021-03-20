
using System;
using System.Windows;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Input;

namespace PredictFXCharts
{
    // ************************************************************************


    public enum TradeOp { None, Buy, Sell, Upsize, Close, Pause, Wait, Kill, Cancel, Downsize, Reverse }
    public enum BaseQuote { None, Counter, Similar, Absolute, Position }
    public enum QtyType { None, Absolute, WorkSize, Position }
    public enum QuoteType { Unknown, Free, Spread, Ask, Bid, BestAsk, BestBid }


    public enum MessageType { None, Info, Warning, Error }

    // ************************************************************************

    struct Security
    {
        public string SecCode;
        public string ClassCode;
        //+ 25.08.2010 GAS
        public string ClassName;
        //+ 25.08.2010 GAS

        public Security(string secCode,
                        string classCode,
                        string className //+ 25.08.2010 GAS
                                            )
        {
            this.SecCode = secCode;
            this.ClassCode = classCode;
            this.ClassName = className; //+ 25.08.2010 GAS
        }

        public override string ToString()
        {
            if (ClassCode == null)
                return SecCode == null ? "{none}" : SecCode;
            else
                return SecCode + "." + ClassCode;
        }

        public int GetKey()
        {
            return GetKey(SecCode == null ? string.Empty : SecCode,
              ClassCode == null ? string.Empty : ClassCode);
        }

        public static int GetKey(string secCode, string classCode)
        {
            return secCode.GetHashCode() ^ ~classCode.GetHashCode();
        }
    }

    // ************************************************************************

    public class KeyBinding
    {
        [XmlAttributeAttribute]
        public Key Key { get; set; }

        [XmlAttributeAttribute]
        public bool OnKeyDown { get; set; }

        public OwnAction Action { get; set; }

        public KeyBinding() { }

        public KeyBinding(Key key, bool onKeyDown,
          TradeOp operation, BaseQuote quote, double value, int quantity)
        {
            this.Key = key;
            this.OnKeyDown = onKeyDown;
            this.Action = new OwnAction(operation, quote, value, quantity);
        }
    }

    public class OwnAction
    {
        [XmlAttributeAttribute]
        public TradeOp Operation { get; set; }

        [XmlAttributeAttribute]
        public BaseQuote Quote { get; set; }

        [XmlAttributeAttribute]
        public double Value { get; set; }

        [XmlAttributeAttribute]
        public int Quantity { get; set; }

        public OwnAction() { }

        public OwnAction(TradeOp operation, BaseQuote quote, double value, int quantity)
        {
            this.Operation = operation;
            this.Quote = quote;
            this.Value = value;
            this.Quantity = quantity;
        }
    }

    // ************************************************************************

    // ************************************************************************
    // *                             Data types                               *
    // ************************************************************************

    struct Message
    {
        public readonly DateTime DateTime;
        public readonly string Text;

        public Message(string text)
        {
            this.DateTime = DateTime.Now;
            this.Text = text;
        }

        public Message(DateTime dateTime, MessageType type, string text)
        {
            this.DateTime = dateTime;
            this.Text = text;
        }
    }

    // ************************************************************************

    struct Quote
    {
        public double Price;
        public int Volume;
        public QuoteType Type;

        public Quote(double price, int volume, QuoteType type)
        {
            this.Price = price;
            this.Volume = volume;
            this.Type = type;
        }
    }

    // ************************************************************************

    struct Spread
    {
        public readonly double Ask;
        public readonly double Bid;

        public Spread(double ask, double bid)
        {
            this.Ask = ask;
            this.Bid = bid;
        }
    }

    // ************************************************************************

    public struct Tick
    {
        public string SecCode;
        public double IntPrice;
        public double RawPrice;
        public int Volume;
        public TradeOp Op;
        public DateTime DateTime;
    }

    public struct Trade
    {
        public double TradeNum;
        public double OrderNum;
        public string SecCode;
        public double IntPrice;
        public double RawPrice;
        public int Quantity;
        public TradeOp Op;
        public DateTime DateTime;
    }

    // ************************************************************************

    struct OwnOrder
    {
        public readonly long Id;
        public readonly double Price;

        public readonly int Active;
        public readonly int Filled;

        public OwnOrder(long id, double price, int active, int filled)
        {
            this.Id = id;
            this.Price = price;
            this.Active = active;
            this.Filled = filled;
        }
    }

    // ************************************************************************

    struct OwnTrade
    {
        public double Price;
        public int Quantity;

        public OwnTrade(double price, int quantity)
        {
            Price = price;
            Quantity = quantity;
        }
    }

    // ************************************************************************
    public struct Setting
    {
        public string SecCode { get; set; } // код бумаги
        public string ClassCode { get; set; } // код класса
        public string ClassName { get; set; } // класс бумаги
        public string OptionBase { get; set; } // +1 базовый актив опциона
        public DateTime MatDate { get; set; } // +1 дата погашения
        public int DaysToMatDate { get; set; } // дней до погашения
        public int Numbids { get; set; } // количество заявкок на покупку
        public int Numoffers { get; set; } // количество заявок на продажу
        public int Biddeptht { get; set; } // суммарный спрос контрактов
        public int Offerdeptht { get; set; } // суммарное предложение контрактов
        public int Voltoday { get; set; } // количество во всех сделках 
        public double Valtoday { get; set; } // текущий оборот в деньгах
        public int Numtrades { get; set; } // количество сделок за сегодня
        public int Numcontracts { get; set; } // количество открытых позиций (Открытый Интерес)
        public double Selldepo { get; set; } // ГО продавца
        public double Buydepo { get; set; } // ГО покупателя
        public DateTime DateTime { get; set; }
        public double Strike { get; set; } // +1 цена страйк
        public string OptionType { get; set; } // +1 тип опциона CALL PUT
        public double Volatility { get; set; } // +1 волатильность опциона
    }

    // ************************************************************************


    // ************************************************************************
    // *                     Data Receiver Interface                          *
    // ************************************************************************

    interface IDataReceiver
    {
        void PutMessage(Message msg);
        void PutStock(Quote[] quotes, Spread spread);
        void PutTick(Tick tick);
        void PutSetting(Setting setting); //+ 11.08.2010 GAS

        void PutTrade(Trade trade);
        void PutOwnOrder(OwnOrder order);
        void PutPosition(int quantity, double price);
    }

    // ************************************************************************
}
