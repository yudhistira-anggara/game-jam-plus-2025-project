using GameJam;
using Godot;
using System;

namespace GameJam
{
    public partial class DialogueManager : Node
    {
        public static DialogueManager Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }
    }
}