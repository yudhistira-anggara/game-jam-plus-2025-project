using Godot;
using System;

namespace GameJam
{
    public partial class ListingLabel : Label
    {
        private GlobalSignals _globalSignals { get; set; }
        private TradeManager _tradeManager { get; set; }

        public override void _Ready()
        {
            if (GlobalSignals.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            if (TradeManager.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;
            _tradeManager = TradeManager.Instance;
            _globalSignals.NewListing += UpdateListing;
            _globalSignals.KillListing += UpdateListing;
            _globalSignals.TestSignal += ClearLabel;
        }

        public void ClearLabel()
        {
            Text = "";
        }

        public void UpdateListing(Listing ls)
        {
            Text = "";
            foreach (var t in _tradeManager.ActiveTrades)
            {
                foreach (var lis in t.Listings)
                {
                    Text += $"\n{lis.Index}.{lis.TargetID} [{lis.TargetOption}] -> Shares: {lis.Shares}, Offer: ${lis.PriceOffer}";
                }
            }
        }
    }
}