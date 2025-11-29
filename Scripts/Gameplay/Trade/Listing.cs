using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class Listing : GodotObject
    {
        public int Index { get; set; }
        public string TargetID { get; set; }
        public string TargetOption { get; set; }
        public int Size { get; set; }
        public decimal Shares { get; set; }
        public decimal PriceOffer { get; set; }
        public double Duration { get; set; }
        public double RandomDie { get; set; } = 0.1;

        public void OnTradeResolved(Trade trade, TradeOption option)
        {
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
            GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
        }

        public void UpdateListing(double delta)
        {
            Duration -= delta;

            if (Duration <= 0)
            {
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
                GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
            }
        }
    }
}