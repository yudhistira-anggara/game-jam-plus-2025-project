using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial interface IDialogueSelection
    {
        void ReadValues(List<DialogueOptionSerializable> dialogueOptions);
        void SetVisibility(bool toVisible);
    }
}