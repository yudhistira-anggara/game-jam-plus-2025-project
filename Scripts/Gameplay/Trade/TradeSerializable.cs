using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class TradeSerializable
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public double Duration { get; set; }
        public List<TradeOptionSerializable> Options { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Flags { get; set; }
    }

    public partial class TradeOptionSerializable : GodotObject
    {
        public string Option { get; set; }
        public int Odds { get; set; }
        public decimal Shares { get; set; }
        public decimal Trend { get; set; }
        public List<string> Tags { get; set; }
    }
}