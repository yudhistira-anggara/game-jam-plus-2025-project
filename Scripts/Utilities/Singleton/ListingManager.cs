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
			GlobalSignals.Instance.TradeDayStart += OnTradeDayStarted;
			GlobalSignals.Instance.TradeDayEnd += OnTradeDayEnded;
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

		public void OnTradeDayStarted()
		{
			//
		}

		public void OnTradeDayEnded()
		{
			Listings.Clear();
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
		public static (int size, int sellAmount) RandomizeTradeVolume(decimal odds)
		{
			// min value of listing has to equal $1
			int minValue = (int)(1 / odds);

			int sellAmount;
			int tSize;

			double volume = GD.RandRange(0, 100);

			if (volume <= 3)
			{
				tSize = 32;
				sellAmount = GD.RandRange(minValue * tSize, minValue * tSize * tSize);
			}
			else if (volume <= 7)
			{
				tSize = 16;
				sellAmount = GD.RandRange(minValue * tSize, minValue * tSize * tSize);
			}
			else if (volume <= 16)
			{
				tSize = 8;
				sellAmount = GD.RandRange(minValue * tSize, minValue * tSize * tSize);
			}
			else if (volume <= 29)
			{
				tSize = 4;
				sellAmount = GD.RandRange(minValue * tSize, minValue * tSize * tSize);
			}
			else
			{
				tSize = 2;
				sellAmount = GD.RandRange(minValue, minValue * tSize * tSize);
			}

			return (tSize, sellAmount);
		}

		public void CreateListing(Trade trade)
		{
			if (BaseListingDuration > trade.Duration || BaseListingDuration > GameManager.Instance.GameTimer.TimeLeft)
				return;

			foreach (var e in trade.Options)
			{
				int listingCount = Listings.Count(d => d.Target.ID == trade.ID && d.Target.Option == e.Option);

				if (listingCount >= MaxListingPerTradeOptions)
					return;

				decimal tShares = trade.Options.Sum(t => t.Shares);

				decimal relativeOdds = Math.Round(e.Shares / tShares, 2);
				decimal avgOdds = 1 / trade.Options.Count;
				decimal bestOpOdds = trade.Options.Max(t => t.Shares) / tShares;

				// decimal minFinalOdds = e.Trend * (decimal)trade.Duration;
				// decimal maxFinalOdds = Math.Abs(e.Trend) * (decimal)trade.Duration;
				// decimal expectedFinalOdds = (decimal)(e.Odds + GD.RandRange((double)minFinalOdds, (double)maxFinalOdds));

				// GD.Print($"{minFinalOdds} <-> {maxFinalOdds} ~ {expectedFinalOdds}");

				var tV = RandomizeTradeVolume(relativeOdds);
				int tSize = tV.size;
				int sellAmount = tV.sellAmount;

				// GD.Print($"{e.Shares} / {tShares} ~ {relativeOdds}; Amount: {sellAmount}x");

				decimal baseProfit = Math.Round(1 - relativeOdds, 2);
				decimal basePayout = baseProfit * sellAmount;

				// Random base confidence from -1.0 to 1.0
				decimal confidence = (decimal)GD.RandRange(-1d, 1d);

				// If option is performing worse than average
				/*
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
				*/

				// Less time and worse odds = cheaper;  Less time and better odds = more expensive
				decimal dur = (decimal)trade.Duration;

				if (dur <= 0)
					dur = 0.01m;

				decimal durMod = 1 - (decimal)Math.Clamp(trade.Duration / trade.BaseDuration, 0, 1);
				// GD.Print($"durMod: {durMod}");

				if (confidence > 0)
					confidence += durMod;
				else
					confidence -= durMod;

				decimal adjustPricing = Math.Round(basePayout * confidence * durMod, 2);

				// GD.Print($"c: {confidence}; dur: [{Math.Round(trade.Duration, 4)} -> {durMod}]; adjP: ${adjustPricing}");

				decimal priceOffer = Math.Round(basePayout + adjustPricing, 2);

				if (priceOffer <= 0)
					priceOffer = basePayout;

				// GD.Print($"bPr: ${baseProfit}; bPa: ${basePayout}; Offer: ${priceOffer}\n");

				e.Shares += sellAmount;

				var ls = new Listing()
				{
					Index = trade.Index,
					Target = new ListingTarget(trade.ID, e.Option),
					Size = tSize,
					Shares = sellAmount,
					PriceOffer = priceOffer,
					Duration = BaseListingDuration
				};
				GlobalSignals.Instance.ResolveTrade += ls.OnTradeResolved;

				Listings.Add(ls);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewListing, ls);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.AddListing, ls, trade);
			}
		}
	}
}
