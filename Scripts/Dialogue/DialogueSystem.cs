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
		public PackedScene TextDisplayer { get; set; }
		public IDialogueBox DialogueBox { get; set; }
		[Export]
		public PackedScene OptionDisplayer { get; set; }

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
		public JsonDialogueParser ParsedDialogue { get; set; }

		public int CurrentPage { get; set; } = 0;
		public int TotalPages { get; set; } = 1;

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
			if (SourceType == 0 && Path != null && TextDisplayer != null)
			{
				ParsedDialogue = new JsonDialogueParser(Path);

				if (TextDisplayer is IDialogueBox box)
				{
					TotalPages = ParsedDialogue.DialogueFile[0].Contents.Count;
					box.ReadValues(ParsedDialogue.DialogueFile[0].Contents[0].Style, 1, TotalPages);
				}
			}
		}

		public override void _Ready()
		{
			if (DialogueBox == null || !IsInstanceValid((Node)DialogueBox))
			{
				var instance = TextDisplayer.Instantiate();
				AddChild(instance);
				DialogueBox = instance as IDialogueBox;

				if (SourceType == 0 && Path != null && TextDisplayer != null)
				{
					ParsedDialogue = new JsonDialogueParser(Path);
					TotalPages = ParsedDialogue.DialogueFile[0].Contents.Count;

					DialogueBox.ReadValues(ParsedDialogue.DialogueFile[0].Contents[0].Style, 1, TotalPages);
				}
			}
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("DialogueTestLeft"))
			{
				if (CurrentPage - 1 < 0)
					return;

				if (ParsedDialogue.DialogueFile[0].Contents[CurrentPage - 1].Type != DialogueType.Talk.ToString())
					return;

				CurrentPage--;
				DialogueBox.ReadValues(ParsedDialogue.DialogueFile[0].Contents[CurrentPage].Style, CurrentPage + 1, TotalPages);
			}
			else if (@event.IsActionPressed("DialogueTestRight"))
			{
				if (CurrentPage + 1 >= TotalPages)
					return;

				if (ParsedDialogue.DialogueFile[0].Contents[CurrentPage + 1].Type != DialogueType.Talk.ToString())
					return;

				CurrentPage++;
				DialogueBox.ReadValues(ParsedDialogue.DialogueFile[0].Contents[CurrentPage].Style, CurrentPage + 1, TotalPages);
			}
		}

		public void HandleStyle()
        {
            //
        }

		public void HandleOptions()
        {
            //
        }
	}

	/*
	Parser for .json dialogue files
	*/

	[Tool]
	public partial class JsonDialogueParser
	{
		public List<DialogueSerializeable> DialogueFile { get; set; }

		public JsonDialogueParser(string path)
		{
			var content = Utils.LoadFromFile(path);
			DialogueFile = JsonSerializer.Deserialize<List<DialogueSerializeable>>(content);

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