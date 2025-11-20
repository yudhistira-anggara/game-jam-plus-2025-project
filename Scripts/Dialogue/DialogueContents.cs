using Godot;
using Godot.Collections;
using System;
using GameJam;

namespace GameJam
{
    [Tool]
    [GlobalClass]
    public partial class DialogueContents : Resource
    {
        public DialogueType Type { get; set; }

        // Talk
        public string Name { get; set; } = "";
        public Image Portrait { get; set; }
        public AudioStream Voice { get; set; }

        // Selection
        public string Next { get; set; }

        // Shared
        [Export]
        public Script Script { get; set; }
        [Export(PropertyHint.MultilineText)]
        public string Text { get; set; } = "";

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = [];

            if (Engine.IsEditorHint())
            {
                if (Type == DialogueType.Talk)
                {
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Name"},
                        {"type", (int)Variant.Type.String},
                        {"hint", (int)PropertyHint.PlaceholderText},
                        {"hint_string", $"Name"}
                    });
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Portrait"},
                        {"type", (int)Variant.Type.Rid},
                        {"hint", (int)PropertyHint.File},
                        {"hint_string", $"*.png, *.bmp, *.jpg, *.jpeg, *.svg, *.tga, *.webp, *.image"}
                    });
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Voice"},
                        {"type", (int)Variant.Type.Rid},
                        {"hint", (int)PropertyHint.File},
                        {"hint_string", $"*.wav, *.mp3, *.ogg"}
                    });
                }
                else
                {
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Next"},
                        {"type", (int)Variant.Type.String},
                        {"hint", (int)PropertyHint.PlaceholderText},
                        {"hint_string", $"Next ID"}
                    });
                }
            }
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