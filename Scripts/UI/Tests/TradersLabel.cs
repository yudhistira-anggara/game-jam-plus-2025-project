using Godot;
using System;

namespace GameJam
{
    public partial class TradersLabel : Label
    {
        private GlobalSignals _globalSignals { get; set; }

        public override void _Ready()
        {
            if (GlobalSignals.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;
            _globalSignals.NewTrader += OnNewTrader;
        }

        public void OnNewTrader(Trader tr)
        {
            Text += $"\n{tr.Name}";
        }
    }
}