using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class TraderSerializable
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Wealth { get; set; }
        public decimal Income { get; set; }
        public Dictionary<string, int> Interests { get; set; }
        public Dictionary<string, int> Personality { get; set; }
        public List<string> Flags { get; set; }
    }
}
