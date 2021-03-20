using System;
using System.Linq;
using System.Collections.Generic;
using ChartDirector;

namespace PredictFXCharts
{
    public class PriceChart : DemoModule
    {
        consoledb2DataDataContext dataContext = new consoledb2DataDataContext();

        List<volquotes> VolQuotesLst = new List<volquotes>();

        // The timeStamps, volume, high, low, open and close data
        DateTime[] timeStamps = null;

        double[] highData = null;
        double[] lowData = null;
        double[] openData = null;
        double[] closeData = null;
        double[] VolumeData = null;


        //Name of demo module
        public string getName() { return "График цены"; }

        //Number of charts produced in this demo module
        public int getNoOfCharts() { return 1; }

        public int getChartGroup() { return 1; }

        //Main code for creating chart.
        //Note: the argument img is unused because this demo only has 1 chart.
        public void createChart(WinChartViewer WebChartViewer1, string img, int stockId, int timeFrameId, DateTime fromDt, DateTime toDt, int ballSize, int chartWidth, int chartHeighth, int priceType)
        {

            var stock = (from s in dataContext.stocks
                         where s.id == stockId
                         select s).FirstOrDefault();

            if (stock == null)
            {
                errMsg(WebChartViewer1, "Инструмент не найден");
                return;
            }

            var quotes = (from q in dataContext.volquotes
                          where q.stock == stockId
                          && q.timeframe == timeFrameId
                          && q.datetime >= fromDt
                          && q.datetime <= toDt.AddDays(1)
                          orderby q.datetime ascending
                          select q);
            VolQuotesLst = quotes.ToList();

            if (quotes == null || quotes.Count() == 0)
            {
                errMsg(WebChartViewer1, "Данные отсутствуют");
                return;
            }

            var timeFrame = (from t in dataContext.timeframes
                             where t.id == timeFrameId
                             select t).FirstOrDefault();

            if (timeFrame == null)
            {
                errMsg(WebChartViewer1, "Тайм фрейм отсутствуют");
                return;
            }

            string tickerKey = stock.ticker.Trim();

            // The data series we want to get.
            if (!getData(fromDt, toDt))
            {
                errMsg(WebChartViewer1, "Инструмент не найден");
                return;
            }

            //-------------


            // Create the chart object using the selected size
            FinanceChart m = new FinanceChart(chartWidth);

            string[] monthNames = { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек" };
            m.setMonthNames(monthNames);

            m.setDateLabelFormat("{value|yyyy}",
                "<*font=bold*>{value|mmm yy}",
                "{value|mm/yy}",
                "<*font=bold*>{value|d mmm}",
                "{value|d}",
                "<*font=bold*>{value|d mmm<*br*>h:nna}",
                "{value|h:nna}");



            // Set the data into the chart object
            m.setData(timeStamps, highData, lowData, openData, closeData, VolumeData,
                0);

            //
            // We configure the title of the chart. In this demo chart design, we put the
            // company name as the top line of the title with left alignment.
            //
            m.addPlotAreaTitle(Chart.TopLeft, tickerKey);
            m.addPlotAreaTitle(Chart.BottomLeft, "<*font=Arial,size=8*>" + m.formatValue(
                fromDt, "dd MMM, yyyy") + " - " + m.formatValue(
                toDt, "dd MMM, yyyy"));

            // A copyright message at the bottom left corner the title area
            m.addPlotAreaTitle(Chart.BottomRight,
                " ");

            //--
            int mainHeight = 350;
            int indicatorHeight = 110;

            //
            // Add the main chart
            //
            XYChart MainChart = m.addMainChart(mainHeight);

            //---------------------

            m.addCandleStick(0x33ff33, 0xff3333);
            m.addVolBars(indicatorHeight, 0x99ff99, 0xff9999, 0xc0c0c0);

            // Output the chart
            WebChartViewer1.Image = m.makeImage();
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

        /// <summary>
        /// Get the timeStamps, highData, lowData, openData, closeData and volData.
        /// </summary>
        /// <param name="ticker">The ticker symbol for the data series.</param>
        /// <param name="startDate">The starting date/time for the data series.</param>
        /// <param name="endDate">The ending date/time for the data series.</param>
        /// <param name="durationInDays">The number of trading days to get.</param>
        /// <param name="extraPoints">The extra leading data points needed in order to
        /// compute moving averages.</param>
        /// <returns>True if successfully obtain the data, otherwise false.</returns>
        protected bool getData(DateTime startDate, DateTime endDate)
        {
            // Get the required 3 min data
            get3MinData(startDate, endDate);

            return true;
        }


        /// <summary>
        /// Get 15 minutes data series for timeStamps, highData, lowData, openData, closeData
        /// and volData.
        /// </summary>
        /// <param name="ticker">The ticker symbol for the data series.</param>
        /// <param name="startDate">The starting date/time for the data series.</param>
        /// <param name="endDate">The ending date/time for the data series.</param>
        protected void get3MinData(DateTime startDate, DateTime endDate)
        {
            timeStamps = new DateTime[VolQuotesLst.Count()];

            highData = new Double[VolQuotesLst.Count()];
            lowData = new Double[VolQuotesLst.Count()];
            openData = new Double[VolQuotesLst.Count()];
            closeData = new Double[VolQuotesLst.Count()];
            VolumeData = new Double[VolQuotesLst.Count()];

            int i = 0;
            foreach (volquotes s in VolQuotesLst)
            {
                timeStamps[i] = s.datetime.Value;

                highData[i] = s.high.Value;
                lowData[i] = s.low.Value;
                openData[i] = s.open.Value;
                closeData[i] = s.close.Value;
                VolumeData[i] = s.volume.Value;

                i++;
            }

        }

    }
}
