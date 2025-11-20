using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueSerializeable
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Next { get; set; }
        public List<DialogueContentsSerializable> Contents { get; set; } = [];
    }
}