using Godot;
using System;

namespace GameJam
{
    public partial class ListingTimerLabel : Label
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

            if (TradeManager.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;
            _tradeManager = TradeManager.Instance;
            _globalSignals.DurationLeft += UpdateTimer;
        }

        public void UpdateTimer(double d)
        {
            Text = "";
            foreach (var t in _tradeManager.ActiveTrades)
            {
                foreach (var ls in t.Listings)
                {
                    var ts = TimeSpan.FromSeconds(ls.Duration);
                    var st = ts.ToString(@"mm\:ss");
                    Text += $"\n{st}";
                }
            }
        }
    }
}