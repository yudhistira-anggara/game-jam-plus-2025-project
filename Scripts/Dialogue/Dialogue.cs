using Godot;
using Godot.Collections;
using System;
using GameJam;

namespace GameJam
{
    public enum DialogueType
    {
        Talk,
        Selection
    }

    [Tool]
    [GlobalClass]
    public partial class Dialogue : Resource
    {
        [Export]
        public string ID { get; set; }
        [Export]
        public Array<DialogueContents> Contents { get; set; } =
        [
            new DialogueContents()
        ];

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