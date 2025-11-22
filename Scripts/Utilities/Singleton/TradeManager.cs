using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class TradeManager : Node
    {
        public static TradeManager Instance { get; private set; }

        public List<Trade> Trades { get; set; } = [];
        public List<TradeRequest> TradeRequests { get; set; } = [];

        public List<string> TradeFiles { get; set; } = [];

        public int TradeCount { get; set; } = 0;
        public int MaxTrades { get; set; } = 6;

        public double DecisionInterval { get; set; } = 1;
        public double TimeSinceLastDecision { get; set; } = 0;

        public override void _Ready()
        {
            Instance = this;
            ModifyTradeFile("res://Resources/Trade/trades.json");
            GlobalSignals.Instance.NewTradeRequest += HandleTradeRequest;
        }

        public override void _Process(double delta)
        {
            if (Trades.Count >= MaxTrades)
                return;

            TimeSinceLastDecision += delta;

            if (!GameManager.Instance.IsGameActive)
                return;

            if (TimeSinceLastDecision < DecisionInterval)
                return;

            GenerateTrade();

            TimeSinceLastDecision = 0;
        }

        public void ResolveTrade()
        {
            // 
        }

        public void HandleTradeRequest(TradeRequest request)
        {
            /*
            if (1 == 1)
                EmitSignal(GlobalSignals.SignalName.Refund, request);
            */

            TradeRequests.Add(request);
        }

        public void ModifyTradeFile(string filePath, bool addFile = true)
        {
            if (!FileAccess.FileExists(filePath))
            {
                GD.PushError("TraderManager.ModifyTraderFile; File does not exist.");
                return;
            }

            if (addFile)
            {
                TradeFiles.Add(filePath);
                return;
            }

            if (!TradeFiles.Contains(filePath))
            {
                GD.PushError("TradeManager.ModifyTraderFile; TradeFiles does not contain filePath.");
                return;
            }

            TradeFiles.Remove(filePath);
        }

        public void GenerateTrade(bool forced = false)
        {
            foreach (var tf in TradeFiles)
            {
                List<Trade> parsed = Utils.ParseJsonList<Trade>(tf);

                foreach (var t in parsed)
                {
                    if (Trades.Count >= MaxTrades)
                        return;

                    if (Trades.Exists(x => x.ID == t.ID) && t.Flags.Contains("Unique"))
                    {
                        //
                    }
                    else
                    {
                        if (Random.Shared.NextDouble() < 0.3 == false)
                            return;

                        t.Index = TradeCount;
                        Trades.Add(t);
                        GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTrade, t);
                        TradeCount++;
                    }
                }
            }
        }
    }
}