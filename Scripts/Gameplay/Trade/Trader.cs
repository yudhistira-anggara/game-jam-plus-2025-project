using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class Trader : Node
    {
        public string ID { get; set; }
        // public string Name { get; set; }
        public string Desc { get; set; }
        public int Wealth { get; set; }
        public int Income { get; set; }
        public int Activeness { get; set; }
        public Dictionary<string, int> Interests { get; set; }
        public Dictionary<string, int> Personality { get; set; }
        public List<string> Flags { get; set; }

        public Dictionary<string, int> TradeHistory { get; set; }
        public Dictionary<string, int> CurrentTrades { get; set; }

        public Trader() { }

        public Trader(TraderSerializeable t)
        {
            ID = t.ID;
            Name = t.Name;
            Desc = t.Desc;
            Wealth = t.Wealth;
            Income = t.Income;
            Activeness = t.Activeness;
            Interests = t.Interests;
            Personality = t.Personality;
            Flags = t.Flags;
        }

        public override void _Ready()
        {
            GlobalSignals.Instance.Refund += HandleRefund;
        }

        public int CalculateTradeAmount(Trade trade)
        {
            var interests = 0;
            var personality = 0;
            var amount = interests + personality;

            return amount;
        }

        public bool CalculateWillingness(Trade trade)
        {
            var interests = 0;
            var personality = 0;
            var willing = true;

            return willing;
        }

        public void DecideAction(Trade trade)
        {
            var willing = CalculateWillingness(trade);

            if (!willing)
                return;


        }

        public void HandleRefund(TradeRequest request)
        {
            if (request.Requester != Name)
                return;

        }

        public void CreateRequest(Trade trade)
        {
            var Request = new TradeRequest()
            {

            };
            CurrentTrades.Add("test", 5);
            EmitSignal(GlobalSignals.SignalName.NewTradeRequest, Request);
        }
    }
}