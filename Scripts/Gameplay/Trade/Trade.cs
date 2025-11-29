using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
	public partial class Trade : GodotObject
	{
		public int Index { get; set; }
		public string ID { get; set; }
		public string Title { get; set; }
		public string Desc { get; set; }
		public double BaseDuration { get; set; }
		public double Duration { get; set; }
		public List<TradeOption> Options { get; set; }
		public List<string> Tags { get; set; }
		public List<string> Flags { get; set; }

		public double DecisionInterval { get; set; } = 1;
		public double TimeSinceLastDecision { get; set; } = 0;

		public List<Listing> Listings = [];
		public int BaseListingDuration { get; set; } = 6;
		public int MaxListingPerTradeOptions { get; set; } = 5;

		public Trade(TradeSerializable ts)
		{
			ID = ts.ID;
			Title = ts.Title;
			Desc = ts.Desc;
			Duration = ts.Duration;
			BaseDuration = ts.Duration;
			Options = TradeOption.From(ts.Options);
			Tags = ts.Tags;
			Flags = ts.Flags;
		}

		public void UpdateTrade(double delta)
		{
			Duration -= delta;
			TimeSinceLastDecision += delta;

			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.DurationLeft, Duration);

			foreach (var ls in Listings.ToList())
			{
				ls.UpdateListing(delta);
			}

			if (Duration <= 0)
				ResolveTrade();

			if (TimeSinceLastDecision < DecisionInterval)
				return;

			CreateListing();

			TimeSinceLastDecision = 0;
		}

		public void ModifyTrend(string name, int value)
		{
			if (!Options.Exists(x => x.Option == name))
			{
				GD.PushError("Option doesn't exist!");
				return;
			}

			var op = Options.Find(x => x.Option == name);
			op.Trend += value;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, this);
		}

		public void ModifyOdds(string name, int value)
		{
			if (!Options.Exists(x => x.Option == name))
			{
				GD.PushError("Option doesn't exist!");
				return;
			}

			var op = Options.Find(x => x.Option == name);
			op.Odds += value;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, this);
		}

		public void UpdateTrend()
		{
			foreach (var op in Options)
			{
				var modMin = 0;
				var min = -2 + modMin;

				var modMax = 0;
				var max = 2 + modMax;

				var rand = Random.Shared.Next(min, max);

				op.Trend += rand;
			}
		}

		public void AddRandomShares()
		{
			decimal tShares = Options.Sum(t => t.Shares);

			foreach (var op in Options)
			{
				if (tShares == 0)
					tShares = 1;

				if (op.Shares == 0)
					op.Shares = 1;

				decimal relativeOdds = Math.Round(op.Shares / tShares * 100, 2);
				decimal curOdds = relativeOdds * (Math.Abs(op.Trend) + 1);
				decimal rand = (decimal)GD.RandRange((double)curOdds * 0.75, (double)curOdds * 1.25);

				// GD.Print($"{relativeOdds}; {curOdds}; {rand}");
				op.Shares += (int)rand;
			}
		}

		public void UpdateOdds()
		{
			decimal tShares = Options.Sum(t => t.Shares);

			foreach (var op in Options)
			{
				op.PreviousOdds = op.Odds;
				decimal relativeOdds = Math.Round(op.Shares / tShares * 100, 1);

				op.Odds = relativeOdds;
				op.Trend = relativeOdds - op.PreviousOdds;
			}
		}

		public void ResolveTrade()
		{
			var rand = new RandomNumberGenerator();
			string[] randStr = [.. Options.Select(a => a.Option)];
			float[] randDist = [.. Options.Select(a => (float)a.Odds)];

			var winName = randStr[rand.RandWeighted(randDist)];

			var winOpt = Options.Find(x => x.Option == winName);

			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeExpire, this);
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.ResolveTrade, this, winOpt);
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

		public void OnListingPurchase(Trader tr, Listing ls)
		{
			//
		}

		public void OnListingDie(Listing ls)
		{
			//
		}

		public void CreateListing()
		{
			foreach (var e in Options)
			{
				int listingCount = Listings.Count(d => d.TargetID == ID && d.TargetOption == e.Option);

				if (listingCount >= MaxListingPerTradeOptions)
					return;

				double listDur = Duration + GD.RandRange(-1d, 4d);

				if (listDur > GameManager.Instance.GameTimer.TimeLeft)
					listDur = Duration;

				if (listDur > Duration || listDur > GameManager.Instance.GameTimer.TimeLeft)
					return;

				decimal tShares = Options.Sum(t => t.Shares);

				decimal relativeOdds = Math.Round(e.Shares / tShares, 2);
				decimal avgOdds = 1 / Options.Count;
				decimal bestOpOdds = Options.Max(t => t.Shares) / tShares;

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
				decimal durLeft = (decimal)Duration;

				if (durLeft <= 0)
					durLeft = 0.01m;

				decimal durMod = 1 - (decimal)Math.Clamp(Duration / BaseDuration, 0, 1);
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
					Index = Index,
					TargetID = ID,
					TargetOption = e.Option,
					Size = tSize,
					Shares = sellAmount,
					PriceOffer = priceOffer,
					Duration = BaseListingDuration
				};

				GlobalSignals.Instance.ResolveTrade += ls.OnTradeResolved;

				Listings.Add(ls);

				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewListing, ls);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.AddListing, this, ls);
			}
		}
	}

	public partial class TradeOption : GodotObject
	{
		public string Option { get; set; }
		public decimal Odds { get; set; }
		public decimal PreviousOdds { get; set; }
		public decimal RealOdds { get; set; }
		public decimal PerceivedOdds { get; set; }
		public decimal Shares { get; set; }
		public decimal Trend { get; set; }
		public List<string> Tags { get; set; }

		public TradeOption(TradeOptionSerializable to)
		{
			Option = to.Option;
			Odds = to.Odds;
			PreviousOdds = to.Odds;
			RealOdds = to.Odds;
			PerceivedOdds = to.Odds;
			Shares = to.Shares;
			Trend = to.Trend;
			Tags = to.Tags;
		}

		public static List<TradeOption> From(List<TradeOptionSerializable> to)
		{
			List<TradeOption> value = [];
			foreach (var t in to)
			{
				value.Add(new TradeOption(t));
			}
			return value;
		}
	}

	public partial class TradeRequest : GodotObject
	{
		public string Requester { get; set; }
		public string Target { get; set; }
		public int Index { get; set; }
		public string Option { get; set; }
		public bool IsBuy { get; set; }
		public decimal Shares { get; set; }
		public decimal Money { get; set; }
	}

	public partial class TradeHistory : GodotObject
	{
		public string Purchaser { get; set; }
		public int Index { get; set; }
		public string Target { get; set; }
		public string Option { get; set; }
		public decimal Shares { get; set; }
		public decimal Money { get; set; }
	}
}
