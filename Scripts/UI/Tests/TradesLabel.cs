using Godot;
using System;

namespace GameJam
{
    public partial class TradesLabel : Label
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
            _globalSignals.NewTrade += OnNewTrade;
        }

        public void OnNewTrade(Trade t)
        {
            Text += $"\n{t.Index}.{t.Title}";
        }
    }
}