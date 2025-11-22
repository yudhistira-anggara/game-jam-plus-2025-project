using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class TraderSerializeable
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Wealth { get; set; }
        public int Income { get; set; }
        public int Activeness { get; set; }
        public Dictionary<string, int> Interests { get; set; }
        public Dictionary<string, int> Personality { get; set; }
        public List<string> Flags { get; set; }
    }
}