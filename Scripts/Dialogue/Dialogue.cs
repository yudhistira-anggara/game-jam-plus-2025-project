using Godot;
using Godot.Collections;
using System;
using GameJam;

namespace GameJam
{
    public enum DialogueType
    {
        Talk,
        Select
    }

    [Tool]
    [GlobalClass]
    public partial class Dialogue : Resource
    {
        [Export]
        public string ID { get; set; } = "";
        [Export(PropertyHint.Enum, "Talk, Select")]
        public DialogueType Type { get; set; } = DialogueType.Talk;
        public string Next { get; set; }
        [Export]
        public Array<DialogueContents> Contents { get; set; } =
        [
            new DialogueContents()
        ];

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = [];

            if (!Engine.IsEditorHint())
                return properties;

            if (Type == DialogueType.Select)
            {
                properties.Add(new Dictionary()
                    {
                        {"name", $"Next"},
                        {"type", (int)Variant.Type.String},
                        {"hint", (int)PropertyHint.PlaceholderText},
                        {"hint_string", $"Next ID"}
                    });
                Next = "";
            }
            else
                Next = null;

            return properties;
        }

        public override void _ValidateProperty(Dictionary property)
        {
            string value = property["name"].AsString();
            bool isMatch = value switch
            {
                "Resource" or "resource_local_to_scene" or "resource_path" or "resource_name" => true,
                _ => false
            };

            if (isMatch)
            {
                property["usage"] = (int)PropertyUsageFlags.None;
            }
        }
    }
}