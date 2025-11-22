using Godot;
using System;

namespace GameJam
{
    public partial class TradeHistoryLabel : Label
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
            _globalSignals.TradeHistoryUpdate += UpdateTradeHistory;
        }

        public void UpdateTradeHistory(TradeHistory his)
        {
            Text = "";
            foreach (var th in _tradeManager.TradeHistory)
            {
                Text += $"{th.Index}.{th.Target}.{th.Option}\n";
                Text += $"{th.Purchaser} bought {th.Shares} shares for ${th.Money}\n";
            }
        }
    }
}