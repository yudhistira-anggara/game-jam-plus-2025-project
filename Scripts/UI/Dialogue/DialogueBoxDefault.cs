using GameJam;
using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueBoxDefault : PanelContainer, IDialogueBox
    {
        public DialogueStyleSerializable DialogueStyle { get; set; }
        public RichTextLabel StyleName { get; set; }
        public TextureRect StylePortrait { get; set; }
        public RichTextLabel StyleText { get; set; }
        public AudioStream StyleVoice { get; set; }
        public MarginContainer PreviousContainer { get; set; }
        public MarginContainer NextContainer { get; set; }
        public Label PageLabel { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public void ReadValues(DialogueStyleSerializable dialogueStyle, int currentPage, int totalPages)
        {
            if (totalPages < currentPage)
            {
                GD.PrintErr();
                return;
            }

            DialogueStyle = dialogueStyle;

            StyleName.Text = DialogueStyle.Name != "" ? DialogueStyle.Name : "Nameless";

            if (DialogueStyle.Portrait != "" && FileAccess.FileExists(DialogueStyle.Portrait))
            {
                Image image = Image.LoadFromFile(DialogueStyle.Portrait);
                ImageTexture imageTexture = ImageTexture.CreateFromImage(image);
                StylePortrait.Texture = imageTexture;
            }

            StyleText.Text = DialogueStyle.Text != "" ? DialogueStyle.Text : "Textless.";

            TotalPages = totalPages;
            CurrentPage = currentPage != 0 ? currentPage : 1;

            ManagePagesVisibility();
            ManagePageLabel();
        }

        public void ChangePage(bool previousPage)
        {
            if (previousPage && CurrentPage > 1)
            {
                CurrentPage--;
            }
            else if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        public void LoadPage(int pageNumber)
        {
            //
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

        public override void _Ready()
        {
            StyleName = (RichTextLabel)GetTree().GetFirstNodeInGroup("NameLabel");
            StylePortrait = (TextureRect)GetTree().GetFirstNodeInGroup("PortaitImage");
            StyleText = (RichTextLabel)GetTree().GetFirstNodeInGroup("TextLabel");
            PreviousContainer = (MarginContainer)GetTree().GetFirstNodeInGroup("PreviousContainer");
            NextContainer = (MarginContainer)GetTree().GetFirstNodeInGroup("NextContainer");
            PageLabel = (Label)GetTree().GetFirstNodeInGroup("PageLabel");

            // GD.Print($"1. {StyleName}");
            // GD.Print($"2. {StylePortrait}");
            // GD.Print($"3. {StyleText}");
            // GD.Print($"4. {PreviousContainer}");
            // GD.Print($"5. {NextContainer}");
        }
    }
}