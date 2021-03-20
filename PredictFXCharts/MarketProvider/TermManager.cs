
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using PredictFXCharts.ObjItems;
using PredictFXCharts.QuikIO;

namespace PredictFXCharts
{
    // ************************************************************************
    // *                         Types & interfaces                           *
    // ************************************************************************

    enum TermConnection { None, Partial, Full, Emulation }

    // ************************************************************************

    interface ITerminal
    {
        string Name { get; }
        void Connect();
        void Disconnect();
        string SendBuyOrder(double price, int quantity, out int tid);
        string SendSellOrder(double price, int quantity, out int tid);
        string KillOrder(long oid);
        void PutLastPrice(double price);
    }

    // ************************************************************************

    class LoopbackTerminal : ITerminal
    {
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public string SendBuyOrder(double price, int quantity, out int tid)
        {
            throw new NotImplementedException();
        }

        public string SendSellOrder(double price, int quantity, out int tid)
        {
            throw new NotImplementedException();
        }

        public string KillOrder(long oid)
        {
            throw new NotImplementedException();
        }

        public void PutLastPrice(double price)
        {
            throw new NotImplementedException();
        }
    }

    // ************************************************************************
    // *                             TermManager                              *
    // ************************************************************************

    class TermManager
    {
        // **********************************************************************

        public enum TStatus { Execute, Cancel, Canceled }

        // ----------------------------------------------------------------------

        public class Transaction
        {
            public TStatus Status;
            public int TId;
            public long OId;
            public double Price;

            public OwnAction Action;

            public Transaction(OwnAction action)
            {
                this.Status = TStatus.Execute;
                this.Action = action;
            }
        }

        // **********************************************************************

        IDataReceiver dataReceiver;
        ITerminal terminal;

        StopOrders stopOrders;

        bool connectionUpdated;
        string connectionText;

        LinkedList<Transaction> tlist;
        bool tlistUpdated;
        StringBuilder tlistText;

        // **********************************************************************

        public double AskPrice { get; protected set; }
        public double BidPrice { get; protected set; }

        public Position Position { get; protected set; }

        // **********************************************************************

        public bool ConnectionUpdated
        {
            get { return connectionUpdated && !(connectionUpdated = false); }
        }

        public string ConnectionText
        {
            get { return connectionText; }
            private set { connectionText = value; connectionUpdated = true; }
        }

        public TermConnection Connected { get; private set; }

        // ----------------------------------------------------------------------

        public bool QueueUpdated
        {
            get { return tlistUpdated && !(tlistUpdated = false); }
        }

        public int QueueLength { get; private set; }
        public string QueueText { get; private set; }

        // **********************************************************************

        public TermManager(IDataReceiver dataReceiver)
        {
            this.dataReceiver = dataReceiver;

            terminal = new LoopbackTerminal();
            stopOrders = new StopOrders(this, dataReceiver);

            ConnectionText = string.Empty;

            Position = new Position(this, dataReceiver);

            tlist = new LinkedList<Transaction>();
            tlistText = new StringBuilder(128);

            ProcessTList();
        }

        // **********************************************************************
        // *                             Соединение                             *
        // **********************************************************************

        public void Connect()
        {
            terminal = new QuikTerminal(this);
            terminal.Connect();
        }

        // **********************************************************************

        public void Disconnect()
        {
            terminal.Disconnect();
        }

        // **********************************************************************

        public void ConnectionUpdate(TermConnection cs, string text)
        {
            Connected = cs;
            ConnectionText = text;
        }

        // **********************************************************************
        // *                               Сделки                               *
        // **********************************************************************

        public void PutOwnTrade(OwnTrade trade)
        {
            Position.PutOwnTrade(trade);
        }

        // **********************************************************************
        // *                               Заявки                               *
        // **********************************************************************

        static void ShowError(int tid, string op, int q, double p, string error)
        {
            MessageBox.Show("T" + tid + " " + op + " Q=" + q + " P=" + Price.GetString(p)
              + "\n\nТранзакция отвергнута торговой системой:\n" + error,
              cfg.ProgName, MessageBoxButtons.OK);
        }

        // **********************************************************************

        public void ActionReply(int tid, long oid, string error)
        {
            lock (tlist)
                for (LinkedListNode<Transaction> node = tlist.First; node != null; node = node.Next)
                    if (node.Value.TId == tid)
                    {
                        if (error == null)
                            node.Value.OId = oid;
                        else
                        {
                            ShowError(tid, node.Value.Action.Operation.ToString(),
                              node.Value.Action.Quantity, node.Value.Price, error);
                            tlist.Remove(node);
                        }

                        ProcessTList();
                        return;
                    }
        }

        // **********************************************************************

        public void OrderUpdate(long oid, int active, int filled)
        {
            lock (tlist)
                for (LinkedListNode<Transaction> node = tlist.First; node != null; node = node.Next)
                    if (node.Value.OId == oid)
                    {
                        Transaction t = node.Value;

                        if (active == 0)
                        {
                            Position.ByOrders += filled;

                            tlist.Remove(node);
                            ProcessTList();
                        }

                        dataReceiver.PutOwnOrder(new OwnOrder(oid, t.Price, active, filled));

                        return;
                    }
        }

        // **********************************************************************
        // *                        Управление заявками                         *
        // **********************************************************************

        void KillOrder(long oid)
        {
            string error = terminal.KillOrder(oid);

            if (error != null)
                ShowError(0, "Kill", 0, 0, error);
        }

        // **********************************************************************
        // *                     Обработка очереди операций                     *
        // **********************************************************************

        void CreateBuyOrder(Transaction t, int quantity)
        {
            switch (t.Action.Quote)
            {
                case BaseQuote.Absolute:
                    t.Price = t.Action.Value;
                    break;

                case BaseQuote.Counter:
                    t.Price = Price.Floor(AskPrice + t.Action.Value);
                    break;

                case BaseQuote.Similar:
                    t.Price = Price.Floor(BidPrice + t.Action.Value);
                    break;

                default:
                    return;
            }

            string error = terminal.SendBuyOrder(t.Price, quantity, out t.TId);

            if (error != null)
                ShowError(t.TId, "Buy", quantity, t.Price, error);
        }

        // ----------------------------------------------------------------------

        void CreateSellOrder(Transaction t, int quantity)
        {
            switch (t.Action.Quote)
            {
                case BaseQuote.Absolute:
                    t.Price = t.Action.Value;
                    break;

                case BaseQuote.Counter:
                    t.Price = Price.Ceil(BidPrice - t.Action.Value);
                    break;

                case BaseQuote.Similar:
                    t.Price = Price.Ceil(AskPrice - t.Action.Value);
                    break;

                default:
                    return;
            }

            string error = terminal.SendSellOrder(t.Price, quantity, out t.TId);

            if (error != null)
                ShowError(t.TId, "Sell", quantity, t.Price, error);
        }

        // **********************************************************************

        void ProcessTList()
        {
            LinkedListNode<Transaction> next = tlist.First;
            LinkedListNode<Transaction> curr;

            while (next != null)
            {
                curr = next;
                next = next.Next;

                switch (curr.Value.Status)
                {
                    // ----------------------------------------------------------------

                    case TStatus.Cancel:
                        if (curr.Value.TId == 0)
                            tlist.Remove(curr);
                        else if (curr.Value.OId > 0)
                        {
                            KillOrder(curr.Value.OId);
                            curr.Value.Status = TStatus.Canceled;
                        }
                        break;

                    // ----------------------------------------------------------------

                    case TStatus.Canceled:
                        break;

                    // ----------------------------------------------------------------

                    case TStatus.Execute:
                        switch (curr.Value.Action.Operation)
                        {
                            // -----------------------------

                            case TradeOp.Buy:
                                if (curr.Value.TId == 0)
                                {
                                    CreateBuyOrder(curr.Value, curr.Value.Action.Quantity);

                                    if (curr.Value.TId == 0)
                                        tlist.Remove(curr);
                                }

                                break;

                            // -----------------------------

                            case TradeOp.Sell:
                                if (curr.Value.TId == 0)
                                {
                                    CreateSellOrder(curr.Value, curr.Value.Action.Quantity);

                                    if (curr.Value.TId == 0)
                                        tlist.Remove(curr);
                                }

                                break;

                            // -----------------------------

                            case TradeOp.Upsize:
                                if (curr.Value.TId == 0)
                                {
                                    if (Position.ByOrders > 0)
                                        CreateBuyOrder(curr.Value, curr.Value.Action.Quantity);
                                    else if (Position.ByOrders < 0)
                                        CreateSellOrder(curr.Value, curr.Value.Action.Quantity);

                                    if (curr.Value.TId == 0 || Position.ByOrders == 0)
                                        tlist.Remove(curr);
                                }

                                break;

                            // -----------------------------

                            case TradeOp.Downsize:
                                if (curr.Value.TId == 0)
                                {
                                    if (Position.ByOrders > 0)
                                        CreateSellOrder(curr.Value, curr.Value.Action.Quantity);
                                    else if (Position.ByOrders < 0)
                                        CreateBuyOrder(curr.Value, curr.Value.Action.Quantity);

                                    if (curr.Value.TId == 0 || Position.ByOrders == 0)
                                        tlist.Remove(curr);
                                }

                                break;

                            // -----------------------------

                            case TradeOp.Close:
                                if (curr == tlist.First && curr.Value.TId == 0)
                                {
                                    if (Position.ByOrders > 0)
                                        CreateSellOrder(curr.Value, Position.ByOrders);
                                    else if (Position.ByOrders < 0)
                                        CreateBuyOrder(curr.Value, -Position.ByOrders);

                                    if (curr.Value.TId == 0 || Position.ByOrders == 0)
                                    {
                                        tlist.Remove(curr);
                                        break;
                                    }
                                }

                                next = null;
                                break;

                            // -----------------------------

                            case TradeOp.Reverse:
                                if (curr == tlist.First && curr.Value.TId == 0)
                                {
                                    if (Position.ByOrders > 0)
                                        CreateSellOrder(curr.Value, Position.ByOrders * 2);
                                    else if (Position.ByOrders < 0)
                                        CreateBuyOrder(curr.Value, -Position.ByOrders * 2);

                                    if (curr.Value.TId == 0 || Position.ByOrders == 0)
                                    {
                                        tlist.Remove(curr);
                                        break;
                                    }
                                }

                                next = null;
                                break;

                                // -----------------------------
                        }
                        break;

                        // ----------------------------------------------------------------
                }
            }

            // --------------------------------------------------------------------

            tlistText.Length = 0;

            if (tlist.Count > 0)
            {
                tlistText.Append("Очередь операций:");
                foreach (Transaction t in tlist)
                {
                    tlistText.AppendLine();
                    tlistText.Append(TradeOpItem.ToString(t.Action.Operation));
                    switch (t.Status)
                    {
                        case TStatus.Cancel:
                            tlistText.Append(" - отмена");
                            break;
                        case TStatus.Canceled:
                            tlistText.Append(" (отменено)");
                            break;
                    }
                }
            }
            else
                tlistText.Append("Очередь операций пуста");

            QueueLength = tlist.Count;
            QueueText = tlistText.ToString();

            tlistUpdated = true;
        }

        // **********************************************************************
        // *                        Пользовательские ф-ции                      *
        // **********************************************************************

        public Transaction ExecAction(OwnAction action)
        {
            Transaction nt = null;

            if (Connected != TermConnection.None)
            {
                lock (tlist)
                {
                    if (action.Operation == TradeOp.Cancel)
                    {
                        if (action.Quote == BaseQuote.Absolute)
                        {
                            foreach (Transaction t in tlist)
                                if (t.Price == action.Value && t.Status != TStatus.Canceled)
                                    t.Status = TStatus.Cancel;

                            stopOrders.KillOrder(0, action.Value);
                        }
                        else
                        {
                            foreach (Transaction t in tlist)
                                if (t.Status != TStatus.Canceled)
                                    t.Status = TStatus.Cancel;

                            stopOrders.Clear();
                        }
                    }
                    else
                    {
                        if (action.Operation == TradeOp.Close || action.Operation == TradeOp.Reverse)
                        {
                            foreach (Transaction t in tlist)
                                if (t.Status != TStatus.Canceled)
                                    t.Status = TStatus.Cancel;

                            stopOrders.Clear();
                        }

                        tlist.AddLast(nt = new Transaction(action));
                    }

                    ProcessTList();
                }
            }

            return nt;
        }

        // **********************************************************************

        public void CancelAction(Transaction t)
        {
            lock (tlist)
                if (t.Status != TStatus.Canceled)
                {
                    t.Status = TStatus.Cancel;
                    ProcessTList();
                }
        }

        // **********************************************************************

        public int CreateStopOrder(double stopPrice, double execPrice, int quantity)
        {
            int id = stopOrders.CreateOrder(stopPrice, execPrice, quantity);

            return id;
        }

        // **********************************************************************

        public void KillStopOrder(int id)
        {
            stopOrders.KillOrder(id, 0);
        }

        // **********************************************************************

        public void DropState()
        {
            AskPrice = 0;
            BidPrice = 0;

            lock (tlist)
            {
                Position.Clear();
                tlist.Clear();
                ProcessTList();
                stopOrders.Clear();
            }
        }

        // **********************************************************************

        public void PutSpread(Spread s) { AskPrice = s.Ask; BidPrice = s.Bid; }

        // **********************************************************************

        public void PutLastPrice(double price)
        {
            stopOrders.PutLastPrice(price);
            terminal.PutLastPrice(price);
            Position.PutLastPrice(price);
        }

        // **********************************************************************
    }
}
