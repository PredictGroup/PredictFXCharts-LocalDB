﻿
using System;

namespace PredictFXCharts
{
  static class Price
  {
    // **********************************************************************

    public static double Floor(double price)
    {
      return price / cfg.u.PriceStep * cfg.u.PriceStep;
    }

    // **********************************************************************

    public static double Ceil(double price)
    {
      return ((price - 1) / cfg.u.PriceStep + 1) * cfg.u.PriceStep;
    }

    // **********************************************************************

    public static int GetInt(double raw)
    {
      return (int)Math.Round(raw * cfg.u.PriceRatio);
    }

    // **********************************************************************

    public static int GetInt(double raw, int ratio)
    {
      return (int)Math.Round(raw * ratio);
    }

    // **********************************************************************

    public static double GetRaw(double price)
    {
      return price / cfg.u.PriceRatio;
    }

    // **********************************************************************

    public static double GetRaw(double price, int ratio)
    {
      return price / ratio;
    }

    // **********************************************************************

    public static string GetString(double price)
    {
      return (price / cfg.u.PriceRatio).ToString("N", cfg.PriceFormat);
    }

    // **********************************************************************

    public static string GetString(double price, int ratio)
    {
      return (price / ratio).ToString("N" + (int)Math.Log10(ratio));
    }

    // **********************************************************************
  }
}
