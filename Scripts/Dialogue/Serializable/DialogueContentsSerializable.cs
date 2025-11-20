using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameJam
{
    [Tool]
    public partial class DialogueContentsSerializable
    {
        // Talk
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Portrait { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Voice { get; set; }

        // Selection
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Next { get; set; }

        // Shared
        public string Script { get; set; } = "";
        public string Text { get; set; } = "";

        public DialogueContentsSerializable()
        {
            Name = "";
            Portrait = "";
            Voice = "";
        }


        public DialogueContentsSerializable(DialogueType type)
        {
            if (type == DialogueType.Talk)
            {
                Name = "";
                Portrait = "";
                Voice = "";
            }
            else
            {
                Next = "";
            }
        }

        public DialogueContentsSerializable(DialogueContents contents)
        {
            var conditions = contents.Text;
            Text = conditions != "" ? conditions : "Text";

            conditions = contents.Script.ResourcePath;
            Script = conditions != "" ? conditions : "";

            if (contents.Type == DialogueType.Talk)
            {
                conditions = contents.Name;
                Name = conditions != "" ? conditions : "Name";

                conditions = contents.Portrait.ResourcePath;
                Portrait = conditions != "" ? conditions : "";

                conditions = contents.Voice.ResourcePath;
                Voice = conditions != "" ? conditions : "";
            }
            else
            {
                conditions = contents.Next;
                Next = conditions != "" ? conditions : "Next";
            }
        }
    }
}