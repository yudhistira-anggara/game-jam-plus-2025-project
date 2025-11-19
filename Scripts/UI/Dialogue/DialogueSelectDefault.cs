using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueSelectDefault : PanelContainer, IDialogueSelection
    {
        public PanelContainer DialogueSelectDefaultInstance { get; set; }
        public List<DialogueOptionSerializable> DialogueOptions { get; set; }
        public VBoxContainer VBoxContainer { get; set; }
        public List<Button> ButtonList { get; set; } = [];

        public void ReadValues(List<DialogueOptionSerializable> dialogueOptions)
        {
            DialogueOptions = dialogueOptions;
            HandleButtons();
        }

        public void HandleButtons()
        {
            if (DialogueOptions == null)
                return;

            if (ButtonList.Count == DialogueOptions.Count)
                return;

            foreach (var i in DialogueOptions)
            {
                var newButton = new Button
                {
                    Text = i.Text
                };

                ButtonList.Add(newButton);
                VBoxContainer.AddChild(newButton);
            }
        }

        public void SetVisibility(bool toVisible)
        {
            DialogueSelectDefaultInstance.Visible = toVisible;
        }

        public override void _Ready()
        {
            DialogueSelectDefaultInstance = (PanelContainer)GetTree().GetFirstNodeInGroup("DialogueSelectDefault");
            VBoxContainer = (VBoxContainer)GetTree().GetFirstNodeInGroup("VBoxContainer");
        }
    }
}