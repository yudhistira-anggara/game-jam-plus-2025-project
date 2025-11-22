using Godot;
using System;

namespace GameJam
{
    public partial class TradeRequestsLabel : Label
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
            _globalSignals.NewTradeRequest += OnNewTradeRequest;
            _globalSignals.TestSignal += ClearLabel;
        }

        public void ClearLabel()
        {
            Text = "";
        }

        public void OnNewTradeRequest(TradeRequest tr)
        {
            Text += $"\n{tr.Requester} -> {tr.Index}";
        }
    }
}