using Godot;
using Godot.Collections;
using GameJam;
using System;
using System.Text.Json;

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

		public Resource Path { get; set; }
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
	}

	/*
	Parser for .json dialogue files
	*/

	public partial class JsonDialogueParser
	{
		//
	}
}