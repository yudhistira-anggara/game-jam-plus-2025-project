using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
	public partial class TradeManager : Node
	{
		public static TradeManager Instance { get; private set; }

		public List<Trade> Trades { get; set; } = [];
		public List<TradeRequest> TradeRequests { get; set; } = [];
		public List<TradeHistory> TradeHistory { get; set; } = [];
		public List<TradeHistory> OldTradeHistory { get; set; } = [];

		public List<string> TradeFiles { get; set; } = [];

		public int TradeCount { get; set; } = 0;
		public int MaxTrades { get; set; } = 6;

		public double DecisionInterval { get; set; } = 1;
		public double TimeSinceLastDecision { get; set; } = 0;

		public override void _Ready()
		{
			Instance = this;
			ModifyTradeFile("res://Resources/Trade/trades.json");
			GlobalSignals.Instance.NewTradeRequest += HandleTradeRequest;
			GlobalSignals.Instance.TradeExpire += UpdateTradeManager;
			GlobalSignals.Instance.BuyListing += UpdateTradeHistory;
			GlobalSignals.Instance.TradeDayStart += OnTradeDayStarted;
			GlobalSignals.Instance.TradeDayEnd += OnTradeDayEnded;
		}

		public override void _Process(double delta)
		{
			TimeSinceLastDecision += delta;

			if (!GameManager.Instance.IsGameActive)
				return;

			foreach (var t in Trades.ToList())
			{
				t.UpdateTrade(delta);
			}

			if (TradeHistory.Count > 10)
			{
				while (TradeHistory.Count > 10)
				{
					var th = TradeHistory[0];
					TradeHistory.RemoveAt(0);
					GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeHistoryUpdate, th);
				}
			}

			if (TimeSinceLastDecision < DecisionInterval)
				return;

			if (Trades.Count < MaxTrades)
				GenerateTrade();

			foreach (var t in Trades.ToList())
			{
				t.AddRandomShares();
				t.UpdateOdds();
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, t);
			}

			TimeSinceLastDecision = 0;
		}

		public void OnTradeDayStarted()
		{
			//
		}

		public void OnTradeDayEnded()
		{
			foreach (var t in TradeHistory)
			{
				OldTradeHistory.Add(t);
			}
			TradeHistory.Clear();
			TradeCount = 0;
			Trades.Clear();
		}

		public void UpdateTradeHistory(Trader t, Listing l)
		{
			var th = new TradeHistory()
			{
				Purchaser = t.ID,
				Index = l.Index,
				Target = l.Target.ID,
				Option = l.Target.Option,
				Shares = l.Shares,
				Money = l.PriceOffer
			};
			TradeHistory.Add(th);
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeHistoryUpdate, th);
		}

		public void UpdateTradeManager(Trade t)
		{
			Trades.Remove(t);
		}

		public void HandleTradeRequest(TradeRequest request)
		{
			// Maybe a task? HANDLE TRADE REFUNDS

			/*
			if (1 == 1)
				EmitSignal(GlobalSignals.SignalName.Refund, request);
			*/

			TradeRequests.Add(request);
		}

		public void ModifyTradeFile(string filePath, bool addFile = true)
		{
			if (!FileAccess.FileExists(filePath))
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			if (addFile)
			{
				TradeFiles.Add(filePath);
				return;
			}

			if (!TradeFiles.Contains(filePath))
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			TradeFiles.Remove(filePath);
		}

		public void GenerateTrade()
		{
			foreach (var tf in TradeFiles)
			{
				List<TradeSerializable> parsed = Utils.ParseJsonList<TradeSerializable>(tf);

				parsed.Shuffle();

				foreach (var t in parsed)
				{
					if (Trades.Count >= MaxTrades)
						return;

					if (Trades.Exists(x => x.ID == t.ID) && t.Flags.Contains("Unique"))
					{
						//
					}
					else
					{
						var dur = t.Duration + GD.RandRange(-5d, 5d);

						if (dur > GameManager.Instance.GameTimer.TimeLeft)
							dur = t.Duration;

						if (t.Duration > GameManager.Instance.GameTimer.TimeLeft)
							return;

						if (GD.Randf() < 0.3)
							return;

						Trade nt = new(t)
						{
							Index = TradeCount
						};

						Trades.Add(nt);
						GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrade, nt);
						TradeCount++;
					}
				}
			}
		}
	}
}
