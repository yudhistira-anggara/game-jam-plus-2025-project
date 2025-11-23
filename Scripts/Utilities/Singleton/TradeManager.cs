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

		public List<string> TradeFiles { get; set; } = [];

		public int TradeCount { get; set; } = 0;
		public int MaxTrades { get; set; } = 4;

		public double DecisionInterval { get; set; } = 1;
		public double TimeSinceLastDecision { get; set; } = 0;

<<<<<<< HEAD
		public override void _Ready()
		{
			Instance = this;
			ModifyTradeFile("res://Resources/Trade/trades.json");
			GlobalSignals.Instance.NewTradeRequest += HandleTradeRequest;
			GlobalSignals.Instance.ResolveTrade += UpdateTradeManager;
			GlobalSignals.Instance.BuyListing += UpdateTradeHistory;
		}
=======
        public override void _Ready()
        {
            Instance = this;
            ModifyTradeFile("res://Resources/Trade/trades.json");
            GlobalSignals.Instance.NewTradeRequest += HandleTradeRequest;
            GlobalSignals.Instance.TradeExpire += UpdateTradeManager;
            GlobalSignals.Instance.BuyListing += UpdateTradeHistory;
        }
>>>>>>> b71daeb9d505db7e8a5acd675e73bf817f451cb8

		public override void _Process(double delta)
		{
			TimeSinceLastDecision += delta;

			if (!GameManager.Instance.IsGameActive)
				return;

			foreach (var t in Trades.ToList())
			{
				t.UpdateTrade(delta);
			}

<<<<<<< HEAD
			if (TimeSinceLastDecision < DecisionInterval)
				return;
=======
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
>>>>>>> b71daeb9d505db7e8a5acd675e73bf817f451cb8

			if (Trades.Count < MaxTrades)
				GenerateTrade();

<<<<<<< HEAD
			foreach (var t in Trades.ToList())
			{
				t.UpdateOdds();
				t.UpdateTrend();
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, t);
			}
=======
            foreach (var t in Trades.ToList())
            {
                t.AddRandomShares();
                t.UpdateOdds();
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeModified, t);
            }
>>>>>>> b71daeb9d505db7e8a5acd675e73bf817f451cb8

			TimeSinceLastDecision = 0;
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
						// if (Random.Shared.NextDouble() < 0.3 == false)
						//    return;

						if (t.Duration > GameManager.Instance.GameTimer.TimeLeft)
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
