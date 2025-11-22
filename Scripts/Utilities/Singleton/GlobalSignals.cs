using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class GlobalSignals : Node
    {
        public static GlobalSignals Instance { get; private set; }

        [Signal]
        public delegate void NewTradeEventHandler(Trade trade);

        [Signal]
        public delegate void NewTraderEventHandler(Trader trader);
        [Signal]
        public delegate void NewTradeRequestEventHandler(TradeRequest request);
        [Signal]
        public delegate void RefundEventHandler(TradeRequest request);

        public override void _Ready()
        {
            Instance = this;
        }
    }
}