using Godot;
using System;

namespace GameJam.DialogueScripts
{
    public partial class Test2 : Node, IDialogueScript
    {
        public void DialogueScriptCall()
        {
            GD.Print("Aye!");
        }
    }
}