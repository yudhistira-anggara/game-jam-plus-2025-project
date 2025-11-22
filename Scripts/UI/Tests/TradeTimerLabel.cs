using Godot;
using System;

namespace GameJam
{
    public partial class TradeTimerLabel : Label
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
            _globalSignals.DurationLeft += UpdateTimer;
        }

        public void UpdateTimer(double d)
        {
            Text = "";
            foreach (var tr in _tradeManager.Trades)
            {
                var ts = TimeSpan.FromSeconds(tr.Duration);
                var st = ts.ToString(@"mm\:ss");
                Text += $"\n{st}";
            }
        }
    }
}