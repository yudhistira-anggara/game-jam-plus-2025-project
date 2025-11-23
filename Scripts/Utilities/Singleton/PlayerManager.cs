using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
    public partial class PlayerManager : Node
    {
        public static PlayerManager Instance { get; private set; }
        private GameManager _gameManager { get; set; }
        private GlobalSignals _globalSignals { get; set; }
        public decimal PreviousWealth { get; set; } = 0;
        public decimal Wealth { get; set; } = 1000;
        public decimal Profit { get; set; } = 0;
        public decimal OverallProfit { get; set; } = 0;
        public decimal Income { get; set; } = 0;
        public List<TradeHistory> TradeHistory { get; set; } = [];
        public bool CanDoStuff { get; set; }
        public Trader PlayerTrader { get; set; }

        public override void _Ready()
        {
            Instance = this;

            if (GameManager.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _gameManager = GameManager.Instance;

            if (GlobalSignals.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;

            PlayerTrader = new Trader()
            {
                ID = "player",
                Name = "Player"
            };
            GlobalSignals.Instance.TradeDayStart += OnTradeStarted;
            GlobalSignals.Instance.TradeDayEnd += OnTradeEnd;
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

            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TraderUpdate, PlayerTrader);
        }

        public void OnTradeStarted()
        {
            PreviousWealth = Wealth;
            CanDoStuff = true;
        }

        public void OnTradeEnd()
        {
            CanDoStuff = false;
            Profit = Wealth - PreviousWealth;
            OverallProfit += Profit;
        }

        public void PurchaseListing(Listing list)
        {
            var th = new TradeHistory()
            {
                Purchaser = Name,
                Index = list.Index,
                Target = list.Target.ID,
                Option = list.Target.Option,
                Shares = list.Shares,
                Money = list.PriceOffer
            };

            TradeHistory.Add(th);
            Wealth -= list.PriceOffer;

            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.BuyListing, PlayerTrader, list);
        }
    }
}