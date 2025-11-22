using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class Trade : GodotObject
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public int Duration { get; set; }
        public List<TradeOption> Options { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Flags { get; set; }

        public int Shares { get; set; }
        public int Value { get; set; }

        public void CreateShares()
        {
            //
        }
        public void UpdateTrade()
        {
            //
        }
    }

    public partial class TradeOption : GodotObject
    {
        public string Name { get; set; }
        public int Odds { get; set; }
        public List<string> Tags { get; set; }
    }

    public partial class TradeRequest : GodotObject
    {
        public string Requester { get; set; }
        public string Target { get; set; }
        public int Index { get; set; }
        public string Option { get; set; }
        public bool IsBuy { get; set; }
        public int Shares { get; set; }
        public int Money { get; set; }
    }
}