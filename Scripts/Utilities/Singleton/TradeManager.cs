using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
	public partial class TradeManager : Node
	{
		public static TradeManager Instance { get; private set; }

		private List<TradeSerializable> _tradeQueue = [];
		private Dictionary<string, TradeSerializable> _trades = [];

		public List<Trade> ActiveTrades { get; set; } = [];
		public List<TradeRequest> TradeRequests { get; set; } = [];
		public List<TradeHistory> TradeHistory { get; set; } = [];
		public List<TradeHistory> OldTradeHistory { get; set; } = [];

		public int TradeIndex { get; set; } = 0;
		public int MaxTrades { get; set; } = 6;

		public double DecisionInterval { get; set; } = 1;
		public double TimeSinceLastDecision { get; set; } = 0;

		public override void _Ready()
		{
			Instance = this;
			LoadFromDirectory("res://Data/Trade/");
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

			foreach (var t in ActiveTrades.ToList())
			{
				t.UpdateTrade(delta);
			}
			
			if (TradeHistory.Count > 10)
			{
				while (TradeHistory.Count > 10)
				{
					var th = TradeHistory[0];
					OldTradeHistory.Add(th);
					TradeHistory.Remove(th);
					GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeHistoryUpdate, th);
				}
			}
			
			if (TimeSinceLastDecision < DecisionInterval)
				return;

			if (ActiveTrades.Count < MaxTrades)
				GenerateTrade();

			CreateTrade();

			foreach (var t in ActiveTrades.ToList())
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
			ActiveTrades.Clear();
		}

		public void UpdateTradeHistory(Trader t, Listing l)
		{
			var th = new TradeHistory()
			{
				Purchaser = t.ID,
				Index = l.Index,
				ID = l.TargetID,
				Option = l.TargetOption,
				Shares = l.Shares,
				Money = l.PriceOffer
			};
			TradeHistory.Add(th);
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeHistoryUpdate, th);
		}

		public void UpdateTradeManager(Trade t)
		{
			ActiveTrades.Remove(t);
		}

		public void HandleTradeRequest(TradeRequest request)
		{
			TradeRequests.Add(request);
		}

		public List<string> LoadFromDirectory(string path)
		{
			List<string> allPaths = [];
			var dir = DirAccess.Open(path);

			foreach (string file in dir.GetFiles())
			{
				string fullPath = $"{path}/{file}";

				if (Utils.ValidateJson<TradeSerializable>(fullPath, out var parsed))
				{
					foreach (var tr in parsed)
					{
						_trades[tr.ID] = tr;
						allPaths.Add(fullPath);
					}
				}
			}

			foreach (string sub in dir.GetDirectories())
			{
				string subPath = $"{path}/{sub}";
				allPaths.AddRange(LoadFromDirectory(subPath));
			}

			return allPaths;
		}

		public void CreateTrade()
		{
			foreach (var q in _tradeQueue.ToList())
			{
				if (ActiveTrades.Count >= MaxTrades)
					return;

				if (GD.Randf() < 0.3)
					return;

				if (_tradeQueue.Count == 0)
					return;

				_tradeQueue.Remove(q);

				double dur = q.Duration + GD.RandRange(0, 10d);

				if (dur >= GameManager.Instance.GameTimer.TimeLeft)
					dur = q.Duration;

				if (q.Duration > GameManager.Instance.GameTimer.TimeLeft)
					return;

				Trade trade = new(q)
				{
					Index = TradeIndex,
					Duration = dur
				};

				GlobalSignals.Instance.BuyListing += trade.OnListingPurchase;
				GlobalSignals.Instance.KillListing += trade.OnListingDie;
				ActiveTrades.Add(trade);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrade, trade);
				TradeIndex++;
			}
		}

		public void CreateSpecificTrade(string id, double duration = 0)
		{
			if (!_trades.TryGetValue(id, out TradeSerializable value))
			{
				GD.PushError("Trader not found!");
				return;
			}

			if (duration != 0)
				value.Duration = duration;

			if (value.Duration > GameManager.Instance.GameTimer.TimeLeft)
			{
				GD.PushError("Duration is more than remaining game time!");
				return;
			}

			_tradeQueue.Add(value);
		}

		public void GenerateTrade()
		{
			List<TradeSerializable> tList = [.. _trades.Values];
			tList.Shuffle();

			foreach (var tf in tList)
			{
				if (GD.Randf() < 0.3)
					break;

				if (tf.Flags.Contains("Disabled"))
					break;

				if (ActiveTrades.Exists(x => x.ID == tf.ID) && tf.Flags.Contains("Unique"))
					break;

				if (_tradeQueue.Exists(x => x.ID == tf.ID) && tf.Flags.Contains("Unique"))
					break;

				if (TradeHistory.Exists(x => x.ID == tf.ID) && tf.Flags.Contains("Unique"))
					break;

				if (OldTradeHistory.Exists(x => x.ID == tf.ID) && tf.Flags.Contains("Unique"))
					break;

				_tradeQueue.Add(tf);
			}
		}
	}
}
