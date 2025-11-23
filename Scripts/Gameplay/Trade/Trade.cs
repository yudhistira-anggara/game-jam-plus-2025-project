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

		public void ModifyTrend(string name, int value)
		{
			if (!Options.Exists(x => x.Option == name))
			{
				GD.PushError($"[{GetType().Name}]");
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
				GD.PushError($"[{GetType().Name}]");
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

			// var winOpt = Options.FindIndex(x => x.Option == winName);
			
			var winOpt = Options.Find(x => x.Option == winName);
			
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.ResolveTrade, this, winOpt);
		}

		public void UpdateTrade(double delta)
		{
			Duration -= delta;

			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.DurationLeft, Duration);

			if (Duration <= 0)
			{
				ResolveTrade();
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeExpire, this);
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
