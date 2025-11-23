using GameJam.DialogueScripts;
using Godot;
using System;

namespace GameJam
{
	public partial class TradesLabel : Label
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

			if (ListingManager.Instance is null)
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			_globalSignals = GlobalSignals.Instance;
			_tradeManager = TradeManager.Instance;
			_globalSignals.NewTrade += UpdateTrade;
			_globalSignals.ResolveTrade += UpdateTrade;
			_globalSignals.TradeModified += UpdateTrade;
		}

		public void UpdateTrade(Trade t)
		{
			Text = "\n";
			foreach (var tr in _tradeManager.Trades)
			{
				Text += $"{tr.Index}.{tr.Title}\n";
				foreach (var o in tr.Options)
				{
					Text += $"[{o.Option}] {o.Odds}% ({UpdateTradeLabel(o.Trend)}) {o.Shares}x\n";
				}
				Text += "\n";
			}
		}

		public string UpdateTradeLabel(decimal t)
		{
			string value = t >=0 ? $"+{t}" : $"{t}";
			return value;
		}

		public void UpdateTrade(Trade trade, TradeOption option)
		{
			Text = "\n";
			foreach (var tr in _tradeManager.Trades)
			{
				Text += $"{tr.Index}.{tr.Title}\n";
				foreach (var o in tr.Options)
				{
					Text += $"[{o.Option}] {o.Odds}% ({UpdateTradeLabel(o.Trend)}) {o.Shares}x\n";
				}
				Text += "\n";
			}
		}
	}
}
