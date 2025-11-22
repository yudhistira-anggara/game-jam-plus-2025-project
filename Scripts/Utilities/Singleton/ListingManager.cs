using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameJam
{
    public partial class ListingManager : Node
    {
        public static ListingManager Instance { get; private set; }

        public List<Listing> Listings { get; set; } = [];

        public int BaseListingDuration { get; set; } = 6;
        public int MaxListingPerTradeOptions { get; set; } = 5;

        public double DecisionInterval { get; set; } = 1;
        public double TimeSinceLastDecision { get; set; } = 0;

        public override void _Ready()
        {
            Instance = this;
            GlobalSignals.Instance.KillListing += UpdateListingManager;
            GlobalSignals.Instance.BuyListing += OnListingPurchase;
        }

        public override void _Process(double delta)
        {
            TimeSinceLastDecision += delta;

            if (!GameManager.Instance.IsGameActive)
                return;

            foreach (var ls in Listings.ToList())
            {
                ls.UpdateListing(delta);
            }

            if (TimeSinceLastDecision < DecisionInterval)
                return;

            var trades = TradeManager.Instance.Trades;

            foreach (var t in trades)
            {
                CreateListing(t);
            }

            TimeSinceLastDecision = 0;
        }

        public void OnListingPurchase(Trader tr, Listing ls)
        {
            UpdateListingManager(ls);
        }

        public void UpdateListingManager(Listing ls)
        {
            Listings.Remove(ls);
        }

        // Decide trade volume: vs 45%, s 29%, m 16%, ml 7%, l 3%
        public static (int size, int sellAmount) RandomizeTradeVolume()
        {
            int sellAmount;
            int tSize;

            double volume = GD.Randf();

            if (volume <= 0.03)
            {
                tSize = 4; // Large = Over 1001
                sellAmount = GD.RandRange(1001, 10000);
            }
            else if (volume <= 0.07)
            {
                tSize = 3; // Medium-Large = 501 to 1000
                sellAmount = GD.RandRange(501, 1000);
            }
            else if (volume <= 0.16)
            {
                tSize = 2; // Medium = 101 to 500
                sellAmount = GD.RandRange(101, 500);
            }
            else if (volume <= 0.29)
            {
                tSize = 1; // Small = 26 to 100
                sellAmount = GD.RandRange(26, 100);
            }
            else
            {
                tSize = 0; // Very Small = 1 to 25
                sellAmount = GD.RandRange(1, 25);
            }

            return (tSize, sellAmount);
        }

        public void CreateListing(Trade trade)
        {
            if (BaseListingDuration > trade.Duration || BaseListingDuration > GameManager.Instance.GameTimer.TimeLeft)
                return;

            foreach (var e in trade.Options)
            {
                if (Listings.Count(l => l.Target.Option == e.Name) > MaxListingPerTradeOptions)
                    // if (Listings.Count(l => l.Target[trade.ID] == e.Name) >= MaxListingPerTradeOptions)
                    return;

                double tOdds = trade.Options.Sum(t => t.Odds);

                var tV = RandomizeTradeVolume();
                int tSize = tV.size;
                int sellAmount = tV.sellAmount;

                double relativeOdds = e.Odds / tOdds;
                double avgOdds = 1 / trade.Options.Count;
                double bestOpOdds = trade.Options.Max(t => t.Odds) / tOdds;

                double minFinalOdds = e.Trend * trade.Duration;
                double maxFinalOdds = Math.Abs(e.Trend) * trade.Duration;
                double expectedFinalOdds = e.Odds + GD.RandRange(minFinalOdds, maxFinalOdds);

                int basePayout = (int)((1 - relativeOdds) * sellAmount);

                // Random base confidence from -1.0 to 1.0
                double confidence = GD.RandRange(-1.0, 1.0);

                // If option is performing worse than average
                if (relativeOdds < avgOdds)
                {
                    // If option is expected to perform better later on
                    if (expectedFinalOdds > bestOpOdds)
                    {
                        confidence += relativeOdds;
                    }// If option is expected to perform worse later on
                    else
                    {
                        confidence -= relativeOdds;
                    }
                }
                // If option is performing better than average
                else
                {
                    // If option is expected to perform better later on
                    if (expectedFinalOdds > bestOpOdds)
                    {
                        confidence += relativeOdds;
                    }
                    else
                    // If option is expected to perform worse later on
                    {
                        confidence -= relativeOdds;
                    }
                }

                // Less time and worse odds = cheaper;  Less time and better odds = more expensive
                double durMod = 1 - Math.Clamp(trade.Duration / 15, 0.0, 0.5);
                durMod = GD.RandRange(confidence, durMod);

                int confidencePricing = (int)(basePayout * confidence);
                int durationPricing = (int)(basePayout * durMod);

                var priceOffer = basePayout + confidencePricing + durationPricing;

                var ls = new Listing()
                {
                    Index = trade.Index,
                    Target = new ListingTarget(trade.ID, e.Name),
                    Size = tSize,
                    Shares = sellAmount,
                    PriceOffer = priceOffer,
                    Duration = BaseListingDuration
                };
                GlobalSignals.Instance.ResolveTrade += ls.OnTradeResolved;

                Listings.Add(ls);
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewListing, ls);
            }
        }
    }
}