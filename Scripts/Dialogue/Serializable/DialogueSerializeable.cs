using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    [Tool]
    public partial class DialogueSerializeable
    {
        public string ID { get; set; }
        public List<DialogueContentsSerializable> Contents { get; set; }

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
}