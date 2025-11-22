using Godot;
using System;

namespace GameJam
{
    public partial class Listing : GodotObject
    {
        public int Index { get; set; }
        public string Target { get; set; }
        public string Option { get; set; }
        public int Shares { get; set; }
        public int PriceOffer { get; set; } // Per shares
        public double Duration { get; set; }
        public double RandomDie { get; set; } = 0.1;

        public void OnTradeResolved(Trade trade)
        {
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
            GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
            Free();
        }

        public void UpdateListing(double delta)
        {
            Duration -= delta;

            if (Duration <= 0 || Random.Shared.NextDouble() < RandomDie)
            {
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillListing, this);
                GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
                Free();
            }
        }
    }
}