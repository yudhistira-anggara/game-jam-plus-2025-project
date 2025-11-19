using Godot;
using Godot.Collections;
using GameJam;
using System;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;

namespace GameJam
{
	[Tool]
	public partial class DialogueSystem : Control
	{
		private int _sourceType;

		[Export]
		public Control Displayer { get; set; }

		[ExportGroup("Dialogue")]
		[Export(PropertyHint.Enum, ".JSON File, Manual")]
		public int SourceType
		{
			get => _sourceType;
			set
			{
				_sourceType = value;
				NotifyPropertyListChanged();
			}
		}

		[ExportToolButton("Test Button")]
		public Callable LoadFromJsonButton => Callable.From(LoadFromJSON);

		public string Path { get; set; }
		public DialogueFile Lines { get; set; } = new DialogueFile();

		[Signal]
		public delegate void LineChangedEventHandler();
		[Signal]
		public delegate void DialogueFinishedEventHandler();

		public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
		{
			Godot.Collections.Array<Godot.Collections.Dictionary> properties = [];

			if (Engine.IsEditorHint())
			{
				if (SourceType == 0)
				{
					properties.Add(new Godot.Collections.Dictionary()
					{
						{"name", $"Path"},
						{"type", (int)Variant.Type.String},
						{"hint", (int)PropertyHint.FilePath},
						{"hint_string", $"*.json"}
					});
				}
				else
				{
					properties.Add(new Godot.Collections.Dictionary()
					{
						{"name", $"Lines"},
						{"type", (int)Variant.Type.Object},
						{"hint", (int)PropertyHint.ResourceType},
						{"hint_string", $"DialogueFile"}
					});
				}
			}

			return properties;
		}

		/*

		private JsonElement _data;
		private int _index;

		public override void _Ready()
		{
			_index = 0;
		}

		public void LoadDialogue(string path)
		{
			var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			file.Close();

			var doc = JsonDocument.Parse(json);
			_data = doc.RootElement;
		}

		public void StartDialogue()
		{
			_index = 0;
			ShowLine();
		}

		public void StartDialogueFile(string path)
		{
			LoadDialogue(path);
			StartDialogue();
		}

		public void Next()
		{
			_index++;

			if (_index >= _data.GetProperty("lines").GetArrayLength())
			{
				EmitSignal(SignalName.DialogueFinished);
				return;
			}

			ShowLine();
		}

		private void ShowLine()
		{
			var line = _data.GetProperty("lines")[_index].GetString();
			EmitSignal(SignalName.LineChanged, line);
		}
		*/

		public void LoadFromJSON()
		{
			if (SourceType == 0 && Path != null)
			{
				var test = new JsonDialogueParser(Path);
			}
		}
	}

	/*
	Parser for .json dialogue files
	*/

	[Tool]
	public partial class JsonDialogueParser
	{
		List<DialogueSerializeable> DialogueFile { get; set; }

		public JsonDialogueParser(string path)
		{
			var content = Utils.LoadFromFile(path);
			DialogueFile = JsonSerializer.Deserialize<List<DialogueSerializeable>>(content);
			// GD.Print(Utils.LoadFromFile(path));
			// GD.Print("");

			foreach (var e in DialogueFile)
			{
				GD.Print("ID: " + e.ID);
				foreach (var f in e.Contents)
				{
					GD.Print("	Type: " + f.Type);
					if (f.Type == DialogueType.Talk.ToString())
					{
						GD.Print("		Name: " + f.Style.Name);
						GD.Print("		Portait: " + f.Style.Portrait);
						GD.Print("		Voice: " + f.Style.Voice);
						GD.Print("		Script: " + f.Style.Script);
						GD.Print("		Text: " + f.Style.Text);
					}
					else if (f.Type == DialogueType.Selection.ToString())
					{
						foreach (var g in f.Options)
						{
							GD.Print("		Option: ");
							GD.Print("			Next: " + g.Next);
							GD.Print("			Script:" + g.Script);
							GD.Print("			Text: " + g.Text);
						}
					}
					GD.Print("");
				}
			}

			/*
			Trying to fix assembly unloading errors, code from:
				https://github.com/godotengine/godot/issues/78513
				https://github.com/dotnet/runtime/issues/65323
			*/
			var assembly = typeof(JsonSerializerOptions).Assembly;
			var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
			var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
			clearCacheMethod?.Invoke(null, [null]);
		}
	}
}