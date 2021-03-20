
using System.Windows;
using System.Windows.Input;

namespace PredictFXCharts
{
    public sealed class UserSettings35
    {
        // **********************************************************************
        // *                              QUIK & DDE                            *
        // **********************************************************************

        public string QuikFolder = @"C:\QUIK";
        public bool AcceptAllTrades = false;

        public string DdeServerName = "pfxcharts";

        // **********************************************************************
        // *                                 Счет                               *
        // **********************************************************************

        public string QuikAccount = "SPBFUT000001";
        public string QuikClientCode = "";

        // **********************************************************************
        // *                              Инструмент                            *
        // **********************************************************************

        public string SecCode = "RiH9";
        public string ClassCode = "SPBFUT";
        public string ClassName = "FUT";

        public int PriceRatio = 1;
        public double PriceStep = 10;


        // **********************************************************************

        public int _1minTimeFrameSize = 60; // sec
        public int _2minTimeFrameSize = 120;
        public int _3minTimeFrameSize = 180;
        public int _4minTimeFrameSize = 240;
        public int _5minTimeFrameSize = 300;
        public int _10minTimeFrameSize = 600;
        public int _15minTimeFrameSize = 900;
        public int _20minTimeFrameSize = 1200;
        public int _30minTimeFrameSize = 1800;
        public int _60minTimeFrameSize = 3600;


        public int MaxPointSize = 9; // Максимальный размер точки на графике

        // **********************************************************************
        // *                                 Прочее                             *
        // **********************************************************************

        public string FontFamily = "Tahoma, Segoe UI, Microsoft Sans Serif";
        public double FontSize = 10.5;

        public bool WindowTopmost = false;
        public bool ConfirmExit = false;

        // **********************************************************************
        // *                               Clone()                              *
        // **********************************************************************

        public UserSettings35 Clone()
        {
            UserSettings35 u = (UserSettings35)MemberwiseClone();

            return u;
        }

        // **********************************************************************


        // **********************************************************************
        // *                        Автозакрытие позиции                        *
        // **********************************************************************

        public int StopOffset = 0;
        public int StopSlippage = 100;
        public bool StopTrail = false;
        public int TakeOffset = 0;

    }
}
