
using System;
using System.Collections.Generic;

namespace PredictFXCharts
{
  class StopOrders
  {
    // **********************************************************************

    struct StopOrder
    {
      public int Id;
      public double StopPrice;
      public double ExecPrice;
      public int Quantity;

      public StopOrder(int id, double stopPrice, double execPrice, int quantity)
      {
        this.Id = id;
        this.StopPrice = stopPrice;
        this.ExecPrice = execPrice;
        this.Quantity = quantity;
      }
    }

    // **********************************************************************

    IDataReceiver dataReceiver;
    TermManager tmgr;

    static int lastId;

    List<StopOrder> orders;

    // **********************************************************************

    public StopOrders(TermManager tmgr, IDataReceiver dataReceiver)
    {
      this.tmgr = tmgr;
      this.dataReceiver = dataReceiver;

      orders = new List<StopOrder>();
    }

    // **********************************************************************

    public int CreateOrder(double stopPrice, double execPrice, int quantity)
    {
      StopOrder order = new StopOrder(--lastId, stopPrice, execPrice, quantity);

      lock(orders)
      {
        orders.Add(order);

        dataReceiver.PutOwnOrder(new OwnOrder(
          order.Id,
          order.StopPrice,
          order.Quantity,
          0));
      }

      return order.Id;
    }

    // **********************************************************************

    public void KillOrder(int id, double price)
    {
      lock(orders)
      {
        int i = 0;

        while(i < orders.Count)
        {
          StopOrder order = orders[i];

          if(order.Id == id || order.StopPrice == price)
          {
            orders.RemoveAt(i);
            dataReceiver.PutOwnOrder(new OwnOrder(order.Id, order.StopPrice, 0, 0));
          }
          else
            i++;
        }
      }
    }

    // **********************************************************************

    public void PutLastPrice(double price)
    {
      if(orders.Count > 0)
        lock(orders)
        {
          int i = 0;

          while(i < orders.Count)
          {
            StopOrder order = orders[i];

            if(order.Quantity > 0)
            {
              if(price >= order.StopPrice)
              {
                tmgr.ExecAction(new OwnAction(TradeOp.Buy, BaseQuote.Absolute, order.ExecPrice, order.Quantity));

                orders.RemoveAt(i);
                dataReceiver.PutOwnOrder(new OwnOrder(order.Id, order.StopPrice, 0, order.Quantity));
              }
              else
                i++;
            }
            else
            {
              if(price <= order.StopPrice)
              {
                tmgr.ExecAction(new OwnAction(TradeOp.Sell, BaseQuote.Absolute, order.ExecPrice, -order.Quantity));

                orders.RemoveAt(i);
                dataReceiver.PutOwnOrder(new OwnOrder(order.Id, order.StopPrice, 0, order.Quantity));
              }
              else
                i++;
            }
          }
        }
    }

    // **********************************************************************

    public void Clear()
    {
      if(orders.Count > 0)
        lock(orders)
        {
          for(int i = 0; i < orders.Count; i++)
          {
            StopOrder order = orders[i];
            dataReceiver.PutOwnOrder(new OwnOrder(order.Id, order.StopPrice, 0, 0));
          }

          orders.Clear();
        }
    }

    // **********************************************************************
  }
}
