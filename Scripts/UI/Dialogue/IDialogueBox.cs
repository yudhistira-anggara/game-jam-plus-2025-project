using Godot;
using System;

namespace GameJam
{
    public partial interface IDialogueBox
    {
        void ReadValues(DialogueContentsSerializable contents, int currentPage, int totalPages);
        void SetVisibility(bool toVisible);
    }
}