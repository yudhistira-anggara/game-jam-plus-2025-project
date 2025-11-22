using Godot;
using System;

namespace GameJam.DialogueScripts
{
    public partial class Test : Node, IDialogueScript
    {
        public void DialogueScriptCall()
        {
            GD.Print("Yay!");
        }
    }
}