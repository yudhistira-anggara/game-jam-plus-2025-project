using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueSelectDefault : PanelContainer, IDialogueSelection
    {
        public Control ParentNode { get; set; }
        public PanelContainer DialogueSelectDefaultInstance { get; set; }
        public List<DialogueContentsSerializable> DialogueOptions { get; set; }
        public VBoxContainer VBoxContainer { get; set; }

        [Signal]
        public delegate void OptionSelectedEventHandler(string value);

        public void ReadValues(List<DialogueContentsSerializable> dialogueOptions)
        {
            Godot.Collections.Array<Node> child = VBoxContainer.GetChildren();
            if (child.Count > 0)
            {
                foreach (var i in child)
                {
                    VBoxContainer.RemoveChild(i);
                }
            }

            DialogueOptions = dialogueOptions;
            HandleButtons();
        }

        public void OnOptionSelected(string value)
        {
            EmitSignal(SignalName.OptionSelected, value);
        }

        public void HandleButtons()
        {
            if (DialogueOptions == null)
                return;

            foreach (var i in DialogueOptions)
            {
                var newButton = new Button
                {
                    Text = i.Text
                };

                newButton.Pressed += () => OnOptionSelected(i.Next);

                VBoxContainer.AddChild(newButton);
            }
        }

        public void SetVisibility(bool toVisible)
        {
            DialogueSelectDefaultInstance.Visible = toVisible;
        }

        public override void _Ready()
        {
            ParentNode = (Control)GetParent();
            DialogueSelectDefaultInstance = (PanelContainer)GetTree().GetFirstNodeInGroup("DialogueSelectDefault");
            VBoxContainer = (VBoxContainer)GetTree().GetFirstNodeInGroup("VBoxContainer");
        }
    }
}