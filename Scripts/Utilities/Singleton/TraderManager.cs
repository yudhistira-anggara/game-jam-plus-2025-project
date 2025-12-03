using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace GameJam
{
	public partial class TraderManager : Node
	{
		public static TraderManager Instance { get; private set; }

		private List<TraderSerializable> _traderQueue = [];
		private Dictionary<string, TraderSerializable> _traders = [];

		public List<Trader> ActiveTraders { get; set; } = [];

		public int TraderIndex { get; set; } = 0;
		public int MaxTraders { get; set; } = 4;

		public double DecisionInterval { get; set; } = 3;
		public double TimeSinceLastDecision { get; set; } = 0;

		public override void _Ready()
		{
			Instance = this;
			LoadFromDirectory("res://Data/Trader/");
			GlobalSignals.Instance.TradeDayStart += OnTradeDayStarted;
		}

		public override void _Process(double delta)
		{
			TimeSinceLastDecision += delta;

			if (!GameManager.Instance.IsGameActive)
				return;

			if (TimeSinceLastDecision < DecisionInterval)
				return;

			InitializeTraders();
			CreateTrader();

			foreach (var tr in ActiveTraders.ToList())
			{
				// GD.Print($"{t.ID}, {tr.Name} - {tr.Activeness}");

				foreach (Trade at in TradeManager.Instance.ActiveTrades.ToList())
				{
					foreach (Listing ls in at.Listings.ToList())
					{
						tr.DecideAction(ls);
					}
				}
			}

			TimeSinceLastDecision = 0;
		}

		public List<string> LoadFromDirectory(string path)
		{
			List<string> allPaths = [];
			var dir = DirAccess.Open(path);

			foreach (string file in dir.GetFiles())
			{
				string fullPath = $"{path}/{file}";

				if (Utils.ValidateJson<TraderSerializable>(fullPath, out var parsed))
				{
					foreach (var tr in parsed)
					{
						_traders[tr.ID] = tr;
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

		public void OnTradeDayStarted()
		{
			//
		}

		public void OnTradeDayEnded()
		{
			foreach (var tr in ActiveTraders)
			{
				tr.Wealth += tr.Income;
			}
		}

		public void CreateTrader()
		{
			List<Trader> anons = ActiveTraders.FindAll(x => x.ID.StartsWith("anon_"));

			if (_traderQueue.Count == 0)
				return;

			if (_traderQueue.Count > 0 && anons.Count != 0)
			{
				anons[0].KillTrader();
				anons.RemoveAt(0);
			}

			if (ActiveTraders.Count >= MaxTraders)
				return;

			TraderSerializable queue = _traderQueue[0];
			_traderQueue.RemoveAt(0);

			Trader trader = new(queue)
			{
				Index = TraderIndex
			};

			GlobalSignals.Instance.ResolveTrade += trader.OnTradeResolved;
			ActiveTraders.Add(trader);
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrader, trader);
			TraderIndex++;
		}

		public void CreateAnonymousTrader()
		{
			while (ActiveTraders.Count < MaxTraders)
			{
				Trader trader = new()
				{
					Index = TraderIndex,
					ID = $"anon_{TraderIndex}",
					Name = $"Anon ({TraderIndex})",
					Wealth = 20,
					Income = 20,
					Personality = { { "Activeness", 20 } }
				};

				GlobalSignals.Instance.ResolveTrade += trader.OnTradeResolved;
				ActiveTraders.Add(trader);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrader, trader);

				TraderIndex++;
			}
		}

		public void CreateSpecificTrader(string id)
		{
			if (!_traders.TryGetValue(id, out TraderSerializable value))
			{
				GD.PushError("Trader not found!");
				return;
			}

			_traderQueue.Add(value);
		}

		public void InitializeTraders()
		{
			List<TraderSerializable> trList = [.. _traders.Values];
			trList.Shuffle();

			foreach (var tf in trList)
			{
				if (GD.Randf() < 0.3)
					break;

				if (tf.Flags.Contains("Disabled"))
					break;

				if (ActiveTraders.Exists(x => x.Name == tf.Name) && tf.Flags.Contains("Unique"))
					break;

				if (_traderQueue.Exists(x => x.ID == tf.ID) && tf.Flags.Contains("Unique"))
					break;

				_traderQueue.Add(tf);
			}
		}
	}
}
