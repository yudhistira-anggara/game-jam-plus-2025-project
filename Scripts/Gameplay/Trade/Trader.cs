using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
	public partial class Trader : GodotObject
	{
		public int Index { get; set; }
		public string ID { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public decimal Wealth { get; set; }
		public decimal Income { get; set; }
		public Dictionary<string, int> Interests { get; set; } = [];
		public Dictionary<string, int> Personality { get; set; } = [];
		public List<string> Flags { get; set; } = [];

		public List<TradeHistory> TradeHistory { get; set; } = [];

		public Trader() { }

		public Trader(TraderSerializable t)
		{
			ID = t.ID;
			Name = t.Name;
			Desc = t.Desc;
			Wealth = t.Wealth;
			Income = t.Income;
			Interests = t.Interests;
			Personality = t.Personality;
			Flags = t.Flags;
		}

		public void HandleBankruptcy()
		{
			if (!(Wealth < 1))
				return;
		}

		public void KillTrader()
		{
			GlobalSignals.Instance.ResolveTrade -= OnTradeResolved;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.KillTrader, this);
		}

		public bool CalculateWillingness(Listing list)
		{
			if (Wealth < list.PriceOffer)
				return false;

			var tr = TradeManager.Instance.ActiveTrades.Find(t => t.ID == list.TargetID);
			decimal tShares = tr.Options.Sum(t => t.Shares);

			var op = tr.Options.Find(op => op.Option == list.TargetOption);

			var ps = Personality;

			decimal relativeOdds = Math.Round(op.Shares / tShares, 2);
			decimal avgOdds = 1 / tr.Options.Count;
			decimal bestOpOdds = tr.Options.Max(t => t.Shares) / tShares;

			decimal minFinalOdds = op.Trend * (decimal)tr.Duration;
			// decimal maxFinalOdds = Math.Abs(op.Trend) * tr.Duration;
			// decimal expectedFinalOdds = op.Odds + GD.RandRange(minFinalOdds, maxFinalOdds);

			GD.Print($"minPredict: {minFinalOdds}; {op.Trend} * {Math.Round(tr.Duration, 4)}");

			// var potentialWinning = bestOpOdds * 100 * list.Shares;
			// var potentialLosing = potentialWinning - list.PriceOffer;
			// var gamble = Math.Clamp(potentialLosing / potentialWinning, 0.0, 1);

			decimal basePricePerShare = Math.Round(1 - relativeOdds, 2);
			decimal offerPricePerShare = Math.Round(list.PriceOffer / list.Shares, 2);

			decimal basePrice = Math.Round(basePricePerShare * list.Shares);

			// Predicted price

			var opinion = GD.RandRange(-100, 100);

			foreach (var t in Interests)
			{
				foreach (var tags in tr.Tags)
				{
					if (t.Key == tags)
						opinion += t.Value;
				}

				foreach (var tags in op.Tags)
				{
					if (t.Key == tags)
						opinion += t.Value;
				}
			}

			// Personality
			var lsTime = list.Duration;
			var tTime = tr.Duration;
			var fomo = ps.TryGetValue("Fomo", out int value) ? value : 15;

			double durMod = 1 - Math.Clamp(lsTime + tTime / fomo, 0.0, 0.5);

			var willing = ps.TryGetValue("Willingness", out value) ? value : 10;
			willing -= ps.TryGetValue("Caution", out value) ? value : 10;
			willing += opinion;

			var finalThoughts = (double)willing / 100;

			if (finalThoughts > 0)
			{
				// GD.Print($"{ID}: {list.Index}.{list.Target.ID} {list.Shares}x [{list.Target.Option}] @ ${list.PriceOffer} ({op.Odds}%) -> {finalThoughts}");
				// GD.Print($"base: ${basePricePerShare} -> ${basePrice} : offer: ${offerPricePerShare} -> ${list.PriceOffer}");
				return true;
			}
			else
				return false;
		}

		public void OnTradeResolved(Trade trade, TradeOption option)
		{
			var allTrade = TradeHistory.FindAll(d => d.Index == trade.Index && d.Target == trade.ID);

			decimal money = 0;

			foreach (var t in allTrade.ToList())
			{
				var op = trade.Options.Find(o => o.Option == t.Option);

				var totalShares = trade.Options.Sum(t => t.Shares);

				decimal relativeOdds = op.Shares / totalShares;
				decimal basePayout = Math.Round(1 - relativeOdds, 2);
				decimal totalPayout = Math.Round(basePayout * t.Shares, 2);

				if (op.Option == option.Option)
				{
					money += totalPayout;
					// GD.Print($"{ID}: {t.Index}.{t.Target}.{t.Option} - {op.Odds}% - {t.Shares}x => ${totalPayout} ");
				}

				allTrade.Remove(t);
			}

			Wealth += money;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TraderUpdate, this);
		}

		public void DecideAction(Listing list)
		{
			var act = (double)Personality["Activeness"] / 100;

			if (!(Random.Shared.NextDouble() < act))
				return;

			if (!CalculateWillingness(list))
				return;

			PurchaseListing(this, list);
		}

		public void PurchaseListing(Trader trader, Listing list)
		{
			var memory = Personality.TryGetValue("Memory", out int value) ? value : 1;

			if (TradeHistory.Count > memory)
				TradeHistory.RemoveAt(0);

			var th = new TradeHistory()
			{
				Purchaser = Name,
				Index = list.Index,
				Target = list.TargetID,
				Option = list.TargetOption,
				Shares = list.Shares,
				Money = list.PriceOffer
			};

			TradeHistory.Add(th);
			Wealth -= list.PriceOffer;

			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.BuyListing, trader, list);
		}
	}
}
