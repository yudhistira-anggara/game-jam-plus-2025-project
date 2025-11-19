using Godot;
using System;

namespace GameJam
{
    [Tool]
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
}