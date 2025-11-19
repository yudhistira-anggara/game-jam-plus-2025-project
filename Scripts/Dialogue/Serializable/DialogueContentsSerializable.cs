using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameJam
{
    [Tool]
    public partial class DialogueContentsSerializable
    {
        public string Type { get; set; } = DialogueType.Talk.ToString();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DialogueStyleSerializable Style { get; set; } = new DialogueStyleSerializable();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DialogueOptionSerializable> Options { get; set; }

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

            if (contents.Type == DialogueType.Talk)
            {
                Style = new DialogueStyleSerializable(contents.Style);
                Options = null;
            }
            else if (contents.Type == DialogueType.Selection)
            {
                Style = null;
                if (contents.Option.Count > 0)
                {
                    foreach (var e in contents.Option)
                    {
                        Options.Add(new DialogueOptionSerializable(e));
                    }
                }
            }
        }
    }
}