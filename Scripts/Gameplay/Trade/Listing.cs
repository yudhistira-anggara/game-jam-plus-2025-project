using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class Listing : GodotObject
    {
        public int Index { get; set; }
        public ListingTarget Target { get; set; }
        public int Size { get; set; }
        public int Shares { get; set; }
        public int PriceOffer { get; set; } // Per shares
        public double Duration { get; set; }
        public double RandomDie { get; set; } = 0.1;

        public void OnTradeResolved(Trade trade)
        {
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
            GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
            // Free();
        }

        public void UpdateListing(double delta)
        {
            Duration -= delta;

            if (Duration <= 0)
            {
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
                GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
                // Free();
            }
        }
    }
    public partial class ListingTarget
    {
        public string ID { get; set; }
        public string Option { get; set; }

        public ListingTarget(string id, string op)
        {
            ID = id;
            Option = op;
        }
    }
}