using System;
using System.Linq;
using System.Collections.Generic;
using ChartDirector;

namespace PredictFXCharts
{
    public class Open_Positions : DemoModule
    {
        consoledb2DataDataContext dataContext = new consoledb2DataDataContext();

        // The timeStamps, volume, high, low, open and close data
        DateTime[] timeStamps = null;
        double[] volData = null;
        double[] highData = null;
        double[] lowData = null;
        double[] openData = null;
        double[] closeData = null;

        //Name of demo module
        public string getName() { return "Количество контрактов"; }

        //Number of charts produced in this demo module
        public int getNoOfCharts() { return 1; }

        public int getChartGroup() { return 2; }

        /// <summary>
        /// Get daily data series for timeStamps, highData, lowData, openData, closeData
        /// and volData.
        /// </summary>
        /// <param name="ticker">The ticker symbol for the data series.</param>
        /// <param name="startDate">The starting date/time for the data series.</param>
        /// <param name="endDate">The ending date/time for the data series.</param>
        protected void getDailyData(WinChartViewer WebChartViewer1, int stockId, int timeFrameId, DateTime startDate, DateTime endDate)
        {

            List<settingquotes> SettingQuotesLst = new List<settingquotes>();

            var stock = (from s in dataContext.stocks
                         where s.id == stockId
                         select s).FirstOrDefault();

            if (stock == null)
            {
                errMsg(WebChartViewer1, "Инструмент не найден");
                return;
            }

            var stockQuotes = from sq in dataContext.settingquotes
                              where sq.stock == stock.id
                              && sq.datetime >= startDate
                              && sq.datetime <= endDate.AddDays(1)
                              && sq.timeframe == timeFrameId
                              && sq.numcontractsclose != 0
                              orderby sq.datetime ascending
                              select sq;

            SettingQuotesLst = stockQuotes.ToList();

            if (stockQuotes == null || stockQuotes.Count() == 0)
            {
                errMsg(WebChartViewer1, "Данные отсутствуют");
                return;
            }

            timeStamps = new DateTime[SettingQuotesLst.Count()];
            highData = new Double[SettingQuotesLst.Count()];
            lowData = new Double[SettingQuotesLst.Count()];
            openData = new Double[SettingQuotesLst.Count()];
            closeData = new Double[SettingQuotesLst.Count()];
            volData = new Double[SettingQuotesLst.Count()];

            int i = 0;
            foreach (settingquotes s in SettingQuotesLst)
            {
                timeStamps[i] = s.datetime.Value;
                highData[i] = s.numcontractshigh.Value;
                lowData[i] = s.numcontractslow.Value;
                openData[i] = s.numcontractsopen.Value;
                closeData[i] = s.numcontractsclose.Value;
                volData[i] = 0;
                i++;
            }
        }


        //Main code for creating chart.
        //Note: the argument img is unused because this demo only has 1 chart.
        public void createChart(WinChartViewer WebChartViewer1, string img, int stockId, int timeFrameId, DateTime fromDt, DateTime toDt, int ballSize, int chartWidth, int chartHeighth, int priceType)
        {

            var stock = (from s in dataContext.stocks
                         where s.id == stockId
                         select s).FirstOrDefault();

            var timeFrame = (from t in dataContext.timeframes
                             where t.id == timeFrameId
                             select t).FirstOrDefault();

            if (timeFrame == null)
            {
                errMsg(WebChartViewer1, "Тайм фрейм отсутствуют");
                return;
            }

            var stockQuotes = from sq in dataContext.settingquotes
                              where sq.stock == stock.id
                              && sq.datetime >= fromDt
                              && sq.datetime <= toDt.AddDays(1)
                              && sq.timeframe == timeFrameId
                              && sq.numcontractsclose != 0
                              orderby sq.datetime ascending
                              select sq;

            if (stockQuotes == null || stockQuotes.Count() == 0)
            {
                errMsg(WebChartViewer1, "Данные отсутствуют");
                return;
            }

            // To compute moving averages starting from the first day, we need to get
            // extra data points before the first day
            int extraDays = 0;

            getDailyData(WebChartViewer1, stockId, timeFrameId, fromDt, toDt);


            // Create a FinanceChart object of width 640 pixels
            FinanceChart c = new FinanceChart(chartWidth);

            c.setDateLabelFormat("{value|yyyy}",
                    "<*font=bold*>{value|mm yy}",
                    "{value|mm/yy}",
                    "<*font=bold*>{value|d/mm}",
                    "{value|d}",
                    "<*font=bold*>{value|d/mm<*br*>h:nn}",
                    "{value|h:nn}");
            //c.setNumberLabelFormat("{={value}/1000}");


            // Add a title to the chart using 18 pts Times Bold Itatic font.
            c.addTitle("Num contracts open positions " + stock.ticker + " " + timeFrame.timeframename + " ", "Times New Roman", 12);

            // Set the data into the finance chart object
            c.setData(timeStamps, highData, lowData, openData, closeData, volData,
                extraDays);

            // Add the main chart with 240 pixels in height
            c.addMainChart(chartHeighth - 100);

            // Add an HLOC symbols to the main chart, using green/red for up/down
            // days
            c.addCandleStick(0x00ff00, 0xff0000);

            c.setAxisOnRight(true);


            // Output the chart
            WebChartViewer1.Image = c.makeImage();

        }

        /// <summary>
        /// Creates a dummy chart to show an error message.
        /// </summary>
        /// <param name="msg">The error message.
        /// <returns>The BaseChart object containing the error message.</returns>
        protected void errMsg(WinChartViewer WebChartViewer1, string msg)
        {
            MultiChart m = new MultiChart(400, 200);
            m.addTitle2(Chart.Center, msg, "Arial", 10).setMaxWidth(m.getWidth());

            WebChartViewer1.Image = m.makeImage();
        }
    }
}
