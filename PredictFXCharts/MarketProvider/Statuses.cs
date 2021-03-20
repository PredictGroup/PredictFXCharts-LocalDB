﻿
using System;
using PredictFXCharts.Market.Internals;

namespace PredictFXCharts.Market
{
    // ************************************************************************

    abstract class Status
    {
        // --------------------------------------------------------------

        bool updated = true;
        MemLog log;

        // --------------------------------------------------------------

        public Status(string name) { log = new MemLog(name, 5); }

        // --------------------------------------------------------------

        public virtual bool Updated { get { return updated && !(updated = false); } }

        public virtual bool IsGood { get; protected set; }
        public virtual bool IsBad { get; protected set; }
        public virtual bool IsError { get; protected set; }

        // --------------------------------------------------------------

        public string Text { get { return log.GetData(); } }

        public void PutInfo(string text)
        {
            log.Put(text);
            updated = true;
        }

        protected void SetUpdated() { updated = true; }

        // --------------------------------------------------------------
    }

    // ************************************************************************

    sealed class ProviderStatus : Status
    {
        // --------------------------------------------------------------

        int connections;

        // --------------------------------------------------------------

        public ProviderStatus(string name) : base(name) { }

        // --------------------------------------------------------------

        public override bool Updated
        {
            get
            {
                if (DateTime.UtcNow.Ticks - DataReceived.Ticks
                  >= cfg.DataRcvTimeout * TimeSpan.TicksPerMillisecond)
                {
                    if (IsGood && !IsBad)
                    {
                        IsBad = true;
                        PutInfo("Отсутствие данных дольше " + cfg.DataRcvTimeout + " мс");
                    }
                }
                else if (IsGood && IsBad)
                {
                    IsBad = false;
                    PutInfo("Поступление данных возобновлено");
                }

                return base.Updated;
            }
        }

        // --------------------------------------------------------------

        public override bool IsGood { get { return connections > 0; } }
        public override bool IsBad { get { return IsGood ? base.IsBad : false; } }

        public override bool IsError
        {
            get
            {
                if (base.IsError)
                {
                    base.IsError = false;
                    SetUpdated();
                    return true;
                }

                return false;
            }
        }

        public DateTime DataReceived { get; set; }

        // --------------------------------------------------------------

        public void Connected(string text) { connections++; PutInfo(text); }
        public void SetError(string text) { IsError = true; PutInfo(text); }

        public void Disconnected(string text)
        {
            if (--connections <= 0)
            {
                connections = 0;
                DataReceived = DateTime.MinValue;
            }
            PutInfo(text);
        }

        // --------------------------------------------------------------
    }

    // ************************************************************************

    sealed class TraderStatus : Status
    {
        // --------------------------------------------------------------

        public TraderStatus(string name) : base(name) { }

        // --------------------------------------------------------------

        public bool CanTrade { get { return !(IsError || IsBad); } }

        // --------------------------------------------------------------

        public void Connected(string text) { IsError = false; IsBad = false; IsGood = true; PutInfo(text); }
        public void Emulated(string text) { IsError = false; IsBad = false; IsGood = false; PutInfo(text); }
        public void Partial(string text) { IsError = false; IsBad = true; IsGood = false; PutInfo(text); }
        public void Disconnected(string text) { IsError = true; IsBad = false; IsGood = false; PutInfo(text); }

        // --------------------------------------------------------------
    }

    // ************************************************************************
}
