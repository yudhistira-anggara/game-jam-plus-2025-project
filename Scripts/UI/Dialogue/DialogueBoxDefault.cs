using GameJam;
using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueBoxDefault : PanelContainer, IDialogueBox
    {
        public PanelContainer Instance { get; set; }
        public RichTextLabel StyleName { get; set; }
        public TextureRect StylePortrait { get; set; }
        public RichTextLabel StyleText { get; set; }
        public MarginContainer PreviousContainer { get; set; }
        public MarginContainer NextContainer { get; set; }
        public VBoxContainer OuterPortraitContainer { get; set; }
        public Label PageLabel { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public void ReadValues(DialogueContentsSerializable contents, int currentPage, int totalPages)
        {
            if (totalPages < currentPage)
            {
                GD.PrintErr();
                return;
            }

            DialogueContentsSerializable Contents = contents;

            StyleName.Text = Contents.Name != "" ? Contents.Name : "";

            ManageVisibility();

            if (Contents.Portrait != "" && FileAccess.FileExists(Contents.Portrait))
            {
                if (!Utils.IsValidImageExtension(Contents.Portrait))
                    return;
                    
                Texture2D image = (Texture2D)ResourceLoader.Load(Contents.Portrait);
                ImageTexture imageTexture = ImageTexture.CreateFromImage(image.GetImage());
                StylePortrait.Texture = imageTexture;
            }
            else
            {
                StylePortrait.Texture = null;
            }

            StyleText.Text = Contents.Text != "" ? Contents.Text : "";

            CurrentPage = currentPage + 1;
            TotalPages = totalPages + 1;

            ManagePagesVisibility();
            ManagePageLabel();
        }

        public void ManageVisibility()
        {
            OuterPortraitContainer.Visible = StyleName.Text != "";
        }

        public void ManagePageLabel()
        {
            PageLabel.Text = $"{CurrentPage}/{TotalPages}";
        }

        public void ManagePagesVisibility()
        {
            if (CurrentPage == 1)
            {
                PreviousContainer.Visible = false;
                NextContainer.Visible = true;
            }
            else if (TotalPages == 1)
            {
                PreviousContainer.Visible = false;
                NextContainer.Visible = false;
            }
            else if (CurrentPage == TotalPages)
            {
                PreviousContainer.Visible = true;
                NextContainer.Visible = false;
            }
            else
            {
                PreviousContainer.Visible = true;
                NextContainer.Visible = true;
            }
        }

        public void SetVisibility(bool toVisible)
        {
            Instance.Visible = toVisible;
        }

        public override void _Ready()
        {
            Instance = (PanelContainer)GetTree().GetFirstNodeInGroup("DialogueBoxDefault");
            StyleName = (RichTextLabel)GetTree().GetFirstNodeInGroup("NameLabel");
            StylePortrait = (TextureRect)GetTree().GetFirstNodeInGroup("PortraitImage");
            StyleText = (RichTextLabel)GetTree().GetFirstNodeInGroup("TextLabel");
            OuterPortraitContainer = (VBoxContainer)GetTree().GetFirstNodeInGroup("OuterPortraitContainer");
            PreviousContainer = (MarginContainer)GetTree().GetFirstNodeInGroup("PreviousContainer");
            NextContainer = (MarginContainer)GetTree().GetFirstNodeInGroup("NextContainer");
            PageLabel = (Label)GetTree().GetFirstNodeInGroup("PageLabel");
        }
    }
}