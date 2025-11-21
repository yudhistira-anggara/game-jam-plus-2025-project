using Godot;
using System;

namespace GameJam
{
    public partial class SingletonManager : Node
    {
        public static SingletonManager Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }
    }
}