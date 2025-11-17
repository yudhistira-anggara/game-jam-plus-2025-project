using Godot;
using Godot.Collections;
using System;

namespace GameJam
{
    [Tool]
    [GlobalClass]
    public partial class DialogueStyle : Resource
    {
        [Export]
        public string Name { get; set; }
        [Export(PropertyHint.ResourceType, "Image")]
        public Image Portrait { get; set; }
        [Export(PropertyHint.ResourceType, "AudioStream")]
        public AudioStream Voice { get; set; }
        [Export(PropertyHint.ResourceType, "Script")]
        public Script Script { get; set; }
        [Export(PropertyHint.MultilineText)]
        public string Text { get; set; }

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