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
        private DialogueType _type;

        public DialogueStyle Style { get; set; } = new DialogueStyle();
        public Array<DialogueOption> Option { get; set; } = [
            new DialogueOption()
        ];

        [Export]
        public DialogueType Type
        {
            get => _type;
            set
            {
                _type = value;
                NotifyPropertyListChanged();
                // GD.Print(GetPropertyList());
            }
        }

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = [];

            if (Engine.IsEditorHint())
            {
                if (_type == DialogueType.Talk)
                {
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Style"},
                        {"type", (int)Variant.Type.Object},
                        {"hint", (int)PropertyHint.ResourceType},
                        {"hint_string", $"DialogueStyle"}
                    });
                }
                else if (_type == DialogueType.Selection)
                {
                    properties.Add(new Dictionary()
                    {
                        {"name", $"Option"},
                        {"type", (int)Variant.Type.Array}
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