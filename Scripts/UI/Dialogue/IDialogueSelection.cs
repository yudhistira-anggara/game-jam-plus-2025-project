using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial interface IDialogueSelection
    {
        void ReadValues(List<DialogueContentsSerializable> contents);
        void SetVisibility(bool toVisible);
    }
}