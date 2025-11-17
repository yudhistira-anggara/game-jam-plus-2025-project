using Godot;
using Godot.Collections;
using System;
using GameJam;
using GameJam.Utils;
using System.Text.Json;
using System.Collections.Generic;

namespace GameJam
{
    [Tool]
    [GlobalClass]
    public partial class DialogueFile : Resource
    {
        [Export]
        public Array<Dialogue> Dialogue { get; set; } =
        [
            new Dialogue()
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

        [ExportToolButton("Save Dialogue to .JSON", Icon = "New")]
        public Callable SaveAsJSONButton => Callable.From(SaveAsJSON);

        public void SaveAsJSON()
        {
            var dialogueFile = new List<DialogueSerializeable>();
            foreach (var e in Dialogue)
            {
                dialogueFile.Add(new DialogueSerializeable(e));
            }
            
            // GD.Print(JsonSerializer.Serialize(dialogueFile));
        }
    }
}