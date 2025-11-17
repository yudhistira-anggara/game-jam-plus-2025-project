using Godot;
using System;
using System.Collections.Generic;

namespace GameJam.Utils
{
    public partial class DialogueSerializeable
    {
        public string ID { get; set; }
        public List<DialogueContentsSerializable> Contents { get; set; } = [];

        public DialogueSerializeable()
        {
            ID = "";
            Contents = [
                new DialogueContentsSerializable()
            ];
        }

        public DialogueSerializeable(Dialogue dialogue)
        {
            ID = dialogue.ID;
            if (dialogue.Contents.Count > 0)
            {
                foreach (var e in dialogue.Contents)
                {
                    Contents.Add(new DialogueContentsSerializable(e));
                }
            }
        }
    }

    public partial class DialogueContentsSerializable
    {
        public string Type { get; set; } = DialogueType.Talk.ToString();
        public DialogueStyleSerializable Style { get; set; } = new DialogueStyleSerializable();
        public List<DialogueOptionSerializable> Options { get; set; } = [];

        public DialogueContentsSerializable()
        {
            Type = DialogueType.Talk.ToString();
            Style = new DialogueStyleSerializable();
            Options = [
                new DialogueOptionSerializable()
            ];
        }

        public DialogueContentsSerializable(DialogueContents contents)
        {
            Type = contents.Type.ToString();
            Style = new DialogueStyleSerializable(contents.Style);
            if (contents.Option.Count > 0)
            {
                foreach (var e in contents.Option)
                {
                    Options.Add(new DialogueOptionSerializable(e));
                }
            }
        }
    }

    public partial class DialogueStyleSerializable
    {
        public string Name { get; set; }
        public string Portrait { get; set; }
        public string Voice { get; set; }
        public string Script { get; set; }
        public string Text { get; set; }

        public DialogueStyleSerializable()
        {
            Name = "";
            Portrait = "";
            Voice = "";
            Script = "";
            Text = "";
        }

        public DialogueStyleSerializable(DialogueStyle style)
        {
            if (style != null)
            {
                _ = style.Name != null ? Name = style.Name : Name = "";
                _ = style.Portrait != null ? Portrait = style.Portrait.ResourcePath : Portrait = "";
                _ = style.Voice != null ? Voice = style.Voice.ResourcePath : Voice = "";
                _ = style.Script != null ? Script = style.Script.ResourcePath : Script = "";
                _ = style.Text != null ? Text = style.Text : Text = "";
            }
        }
    }

    public partial class DialogueOptionSerializable
    {
        public string Next { get; set; }
        public string Script { get; set; }
        public string Text { get; set; }

        public DialogueOptionSerializable()
        {
            Next = "";
            Script = "";
            Text = "";
        }

        public DialogueOptionSerializable(DialogueOption option)
        {
            if (option != null)
            {
                _ = option.Next != null ? Next = option.Next : Next = "";
                _ = option.Script != null ? Script = option.Script.ResourcePath : Script = "";
                _ = option.Text != null ? Text = option.Text : Text = "";
            }
        }
    }
}