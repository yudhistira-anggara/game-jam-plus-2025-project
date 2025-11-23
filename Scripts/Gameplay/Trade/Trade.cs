using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    /*
    DISPLAYED ODDS = (OPTION_VALUE / TOTAL_VALUE_OF_OPTIONS) * 100 -> %
    */

    public partial class Trade : GodotObject
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public double Duration { get; set; }
        public List<TradeOption> Options { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Flags { get; set; }

		public int Shares { get; set; }
		public int Value { get; set; }

		public void ModifyTrend(string name, int value)
		{
			if (!Options.Exists(x => x.Name == name))
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			var op = Options.Find(x => x.Name == name);
			op.Trend += value;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, this);
		}

		public void ModifyOdds(string name, int value)
		{
			if (!Options.Exists(x => x.Name == name))
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			var op = Options.Find(x => x.Name == name);
			op.Odds += value;
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, this);
		}

		public void UpdateTrend()
		{
			foreach (var op in Options)
			{
				// TASK: DO MODIFIER LOGIC

				var modMin = 0;
				var min = -2 + modMin;

				var modMax = 0;
				var max = 2 + modMax;

				var rand = Random.Shared.Next(min, max);

				op.Trend += rand;
			}
		}

		public void UpdateOdds()
		{
			foreach (var op in Options)
			{
				// TASK: DO MODIFIER LOGIC

				var modMin = 0;
				var min = -2 + modMin;

				var modMax = 0;
				var max = 2 + modMax;

				var rand = Random.Shared.Next(min, max);

				var val = op.Trend + rand;
				op.Odds += val;
			}
		}

		public void UpdateTrade(double delta)
		{
			Duration -= delta;

            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.DurationLeft, Duration);

            if (Duration <= 0)
            {
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.ResolveTrade, this);
            }
        }
    }

    public partial class TradeOption : GodotObject
    {
        public string Name { get; set; }
        public double Odds { get; set; }
        public double Trend { get; set; }
        public List<string> Tags { get; set; }
    }

    public partial class TradeRequest : GodotObject
    {
        public string Requester { get; set; }
        public string Target { get; set; }
        public int Index { get; set; }
        public string Option { get; set; }
        public bool IsBuy { get; set; }
        public int Shares { get; set; }
        public int Money { get; set; }
    }

    public partial class TradeHistory : GodotObject
    {
        public string Purchaser { get; set; }
        public int Index { get; set; }
        public string Target { get; set; }
        public string Option { get; set; }
        public int Shares { get; set; }
        public int Money { get; set; }
    }
}