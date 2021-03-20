
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Linq;

namespace PredictFXCharts
{
    static class cfg
    {
        // **********************************************************************
        // *                        Constants & Properties                      *
        // **********************************************************************

        public const string ProgName = "PredictFXCharts";
        public static readonly string FullProgName;

        // **********************************************************************

        public const int TryConnectInterval = 1000;
        public const int QuikTryConnectInterval = 1000;

        // **********************************************************************

        public static readonly TimeSpan RefreshInterval = new TimeSpan(0, 0, 0, 0, 15);
        public static readonly TimeSpan SbUpdateInterval = new TimeSpan(0, 0, 0, 1, 000);

        // **********************************************************************

        public static UserSettings35 u { get; private set; }

        // **********************************************************************

        public static int DataRcvTimeout = 3000;

        // **********************************************************************

        public static int WorkSecKey { get; private set; }

        public static string MainFormTitle { get; private set; }

        public static CultureInfo BaseCulture { get; private set; }
        public static NumberFormatInfo PriceFormat { get; private set; }

        public static double QuoteHeight { get; private set; }
        public static double TextTopOffset { get; private set; }
        public static double TextMinWidth { get; private set; }

        // **********************************************************************

        public const string UserCfgFileExt = "cfg";

        // **********************************************************************

        public static readonly string AsmPath;

        public static readonly string ExecFile;
        public static readonly string UserCfgFile;
        public static readonly string StatCfgFile;


        // **********************************************************************
        // *                             Constructor                            *
        // **********************************************************************

        static cfg()
        {
            // ------------------------------------------------------------

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            FullProgName = ProgName + " " + ver.Major.ToString() + "." + ver.Minor.ToString();

            // ------------------------------------------------------------

            ExecFile = Assembly.GetExecutingAssembly().Location;
            string fs = ExecFile.Remove(ExecFile.LastIndexOf('.') + 1);

            UserCfgFile = fs + UserCfgFileExt;
            StatCfgFile = fs + "sc";


            // ------------------------------------------------------------

            BaseCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            BaseCulture.NumberFormat.NumberDecimalDigits = 0;

            PriceFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();

            // ------------------------------------------------------------

#if DEBUG
            u = new UserSettings35();
            Reinit();
#endif

            // ------------------------------------------------------------
        }


        // **********************************************************************
        // *                          Properties reinit                         *
        // **********************************************************************

        public static void Reinit()
        {
            WorkSecKey = Security.GetKey(cfg.u.SecCode, cfg.u.ClassCode);

            MainFormTitle = u.SecCode.Length > 0 ? u.SecCode + " - " + cfg.FullProgName : cfg.FullProgName;

            PriceFormat.NumberDecimalDigits = (int)Math.Log10(u.PriceRatio);

        }

        // **********************************************************************
        // *                        User config methods                         *
        // **********************************************************************

        public static void SaveUserConfig(string fn)
        {
            try
            {
                using (Stream fs = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(UserSettings35));
                    xs.Serialize(fs, u);
                }
            }
            catch (Exception e)
            {
                PredictFXChartsMain.ShowMessage("Ошибка сохранения конфигурационного файла:\n"
                + e.Message);
            }
        }

        // **********************************************************************

        public static void LoadUserConfig(string fn)
        {
            try
            {
                using (Stream fs = File.OpenRead(fn))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(UserSettings35));
                    u = (UserSettings35)xs.Deserialize(fs);
                }

                Reinit();
            }
            catch (Exception e)
            {
                if (!(u == null && e is FileNotFoundException))
                    PredictFXChartsMain.ShowMessage("Ошибка загрузки конфигурационного файла:\n" + e.Message
                    + "\nИспользованы исходные настройки.");

                if (u == null)
                {
                    u = new UserSettings35();
                    Reinit();
                }
            }
        }

        // **********************************************************************
    }
}
