using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameJam
{
    [Tool]
    public partial class DialogueSerializeable
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Next { get; set; }
        public List<DialogueContentsSerializable> Contents { get; set; } =
        [
            new DialogueContentsSerializable(DialogueType.Talk)
        ];

        public DialogueSerializeable()
        {
            ID = "";
            Type = DialogueType.Talk.ToString();
            Contents = [new DialogueContentsSerializable(DialogueType.Talk)];
        }

        public DialogueSerializeable(Dialogue dialogue)
        {
            if (dialogue.Contents == null)
            {
                GD.PrintErr(new NullReferenceException());
                return;
            }

            ID = dialogue.ID;
            Type = dialogue.Type.ToString();
            Next = dialogue.Next;

            foreach (var e in dialogue.Contents)
            {
                Contents.Add(new DialogueContentsSerializable(e));
            }
        }
    }
}