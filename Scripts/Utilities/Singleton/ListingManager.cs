using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class ListingManager : Node
    {
        public static ListingManager Instance { get; private set; }

        public List<Listing> Listings { get; set; } = [];

        public int BaseListingDuration { get; set; } = 9;
        public int MaxListingPerTradeOptions { get; set; } = 4;

        public double DecisionInterval { get; set; } = 1;
        public double TimeSinceLastDecision { get; set; } = 0;

        public override void _Ready()
        {
            Instance = this;
        }

        public override void _Process(double delta)
        {
            //
        }

        public void CreateListing(Trade trade)
        {
            if (BaseListingDuration > trade.Duration || BaseListingDuration > GameManager.Instance.GameTimer.TimeLeft)
                return;

            // TASK: OPTION CHOOSE LOGIC
            // TASK: SHARES AMOUNT LOGIC
            // TASK: PRICE OFFER LOGIC
            // SUGGESTIONS: BASED ON TRADE'S VALUE, SHARES, DURATION, ETC.

            var ls = new Listing()
            {
                Index = trade.Index,
                Target = trade.ID,
                Duration = BaseListingDuration
            };
            GlobalSignals.Instance.ResolveTrade += ls.OnTradeResolved;

            Listings.Add(ls);
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewListing, ls);
        }
    }
}