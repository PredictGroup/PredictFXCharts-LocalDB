﻿using System;

namespace PredictFXCharts
{
    /// <summary>
    /// Represents the function each demo chart module must provide
    /// </summary>
    public interface DemoModule
    {
        /// <summary>
        /// A human readable name for the module
        /// </summary>
        string getName();

        /// <summary>
        /// The number of demo charts generated by this module
        /// </summary>
        int getNoOfCharts();

        int getChartGroup();

        /// <summary>
        /// Generate a demo chart and display it in the given Windows.ChartViewer. 
        /// The chartNo argument indicate which demo chart to generate. It must be 
        /// a number from 0 to (n - 1), in ASCII string format.
        /// </summary>
        void createChart(ChartDirector.WinChartViewer viewer, string chartNo, int stock, int timeFrame, DateTime fromDt, DateTime toDt, int ballSize, int chartWidth, int chartHeighth, int priceType);
    }
}