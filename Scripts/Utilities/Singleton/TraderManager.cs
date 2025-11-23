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

        public List<Trader> Traders { get; set; } = [];
        public List<string> TraderFiles { get; set; } = [];

        public int TraderCount { get; set; } = 0;
        public int MaxTraders { get; set; } = 6;

        public double DecisionInterval { get; set; } = 3;
        public double TimeSinceLastDecision { get; set; } = 0;

        public override void _Ready()
        {
            Instance = this;
            ModifyTraderFile("res://Resources/Trade/trader.json");
            GlobalSignals.Instance.TradeDayStart += OnTradeDayStarted;
        }

        public override void _Process(double delta)
        {
            TimeSinceLastDecision += delta;

            if (!GameManager.Instance.IsGameActive)
                return;

            if (TimeSinceLastDecision < DecisionInterval)
                return;

            foreach (var tr in Traders.ToList())
            {
                // GD.Print($"{t.ID}, {tr.Name} - {tr.Activeness}");

                foreach (var ls in ListingManager.Instance.Listings.ToList())
                {
                    tr.DecideAction(ls);
                }
            }

            TimeSinceLastDecision = 0;
        }

        public void ModifyTraderFile(string filePath, bool addFile = true)
        {
            if (!FileAccess.FileExists(filePath))
            {
                GD.PushError("Tradermanager.ModifyTraderFile; File does not exist.");
                return;
            }

            if (addFile)
            {
                TraderFiles.Add(filePath);
                return;
            }

            if (!TraderFiles.Contains(filePath))
            {
                GD.PushError("Tradermanager.ModifyTraderFile; TraderFiles does not contain filePath.");
                return;
            }

            TraderFiles.Remove(filePath);
        }

        public void OnTradeDayStarted()
        {
            //
        }

        public void OnTradeDayEnded()
        {
            foreach (var tr in Traders)
            {
                tr.Wealth += tr.Income;
            }
        }

        public void CreateAnonymousTrader()
        {
            while (Traders.Count < MaxTraders)
            {
                var trader = new Trader()
                {
                    Index = TraderCount,
                    ID = $"anon_{TraderCount}",
                    Name = $"Anon ({TraderCount})",
                    Wealth = 20,
                    Income = 20,
                    Activeness = 20
                };
                GlobalSignals.Instance.ResolveTrade += trader.OnTradeResolved;
                Traders.Add(trader);
                GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrader, trader);

                TraderCount++;
            }
        }

        public void CreateSpecificTrader(string filePath, string id)
        {
            // Trader parsed = Utils.ParseJson<Trader>(filePath);
        }

        public void InitializeTraders()
        {
            foreach (var tf in TraderFiles)
            {
                List<TraderSerializable> parsed = Utils.ParseJsonList<TraderSerializable>(tf);

                foreach (var tr in parsed)
                {
                    if (Traders.Count >= MaxTraders)
                        return;

                    if (Traders.Exists(x => x.Name == tr.Name) && tr.Flags.Contains("Unique"))
                    {
                        //
                    }
                    else if (!tr.Flags.Contains("Disabled"))
                    {
                        Trader nt = new(tr)
                        {
                            Index = TraderCount,
                        };

                        GlobalSignals.Instance.ResolveTrade += nt.OnTradeResolved;
                        Traders.Add(nt);
                        GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrader, nt);
                        TraderCount++;
                    }
                }
            }

            if (Traders.Count < MaxTraders)
                CreateAnonymousTrader();
        }
    }
}