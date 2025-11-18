using Godot;
using Godot.Collections;
using System;
using GameJam;

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
            var dialogueFile = new System.Collections.Generic.List<DialogueSerializeable>();
            foreach (var e in Dialogue)
            {
                dialogueFile.Add(new DialogueSerializeable(e));
            }

            var saveDialog = new EditorFileDialog
            {
                FileMode = EditorFileDialog.FileModeEnum.SaveFile,
                Access = EditorFileDialog.AccessEnum.Filesystem,
                Filters = ["*.json ; JSON files"]
            };

            var viewport = EditorInterface.Singleton.GetEditorMainScreen();
            viewport.AddChild(saveDialog);

            saveDialog.PopupFileDialog();

            saveDialog.FileSelected += path =>
            {
                FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                file.StoreString(System.Text.Json.JsonSerializer.Serialize(dialogueFile).ToString());
                file.Close();
            };
            // GD.Print(System.Text.Json.JsonSerializer.Serialize(dialogueFile));
        }
    }
}