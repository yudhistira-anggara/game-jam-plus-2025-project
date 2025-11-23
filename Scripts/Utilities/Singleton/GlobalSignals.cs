using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class GlobalSignals : Node
    {
        public static GlobalSignals Instance { get; private set; }

        [Signal]
        public delegate void TestSignalEventHandler();

        [Signal]
        public delegate void TradeDayStartEventHandler();
        [Signal]
        public delegate void TradeDayEndEventHandler();
        [Signal]
        public delegate void GameEndEventHandler();

        [Signal]
        public delegate void DurationLeftEventHandler(double duration);

        [Signal]
        public delegate void NewListingEventHandler(Listing listing);
        [Signal]
        public delegate void KillListingEventHandler(Listing listing);
        [Signal]
        public delegate void BuyListingEventHandler(Trader trader, Listing listing);

        [Signal]
        public delegate void NewTradeEventHandler(Trade trade);
        [Signal]
        public delegate void ResolveTradeEventHandler(Trade trade, TradeOption option);
        [Signal]
        public delegate void TradeExpireEventHandler(Trade trade);
        [Signal]
        public delegate void TradeModifiedEventHandler(Trade trade);

        [Signal]
        public delegate void NewTraderEventHandler(Trader trader);
        [Signal]
        public delegate void TraderUpdateEventHandler(Trader trader);

        [Signal]
        public delegate void TradeHistoryUpdateEventHandler(TradeHistory th);

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