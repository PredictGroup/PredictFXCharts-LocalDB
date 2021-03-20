
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictFXCharts
{
    [Serializable()]
    public struct StockDataElement
    {
        public string ticker;

        public DateTime date;

        public double high;
        public double low;

        public double open;
        public double close;

        public double middle; // Middle price

        public double Volume;
        public double VolumeBuy;
        public double VolumeSell;

        public double PriceVolumeSum; 

        // ------------------------------------------------------------


        public StockDataElement(string Ticker, DateTime dt)
        {
            this.ticker = Ticker;
            this.date = dt;

            this.high = 0;
            this.low = double.MaxValue;

            this.open = 0;
            this.close = 0;

            this.middle = 0;

            this.Volume = 0;
            this.VolumeBuy = 0;
            this.VolumeSell = 0;

            this.PriceVolumeSum = 0;
        }

    }
}
