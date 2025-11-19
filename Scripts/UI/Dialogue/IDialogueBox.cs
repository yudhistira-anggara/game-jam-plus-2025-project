using Godot;
using System;

namespace GameJam
{
    public partial interface IDialogueBox
    {
        void ReadValues(DialogueStyleSerializable dialogueStyle, int currentPage, int totalPages);
        void ChangePage(bool previousPage);
        void SetVisibility(bool toVisible);
    }
}