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
		public PackedScene SelectBox { get; set; }
		public DialogueSelectDefault DialogueSelection { get; set; }

		[ExportGroup("Dialogue")]
		[Export(PropertyHint.Enum, ".json, Editor")]
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

		public List<DialogueSerializeable> DialogueLines { get; set; }

		public string CurrentID { get; set; }
		public string TargetID { get; set; }

		public int CurrentDialogue { get; set; }
		public int TotalDialogue { get; set; }

		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }

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

		public void LoadFromJSON()
		{
			if (SourceType == 0 && Path != null && TextDisplayer != null)
			{
				if (TextDisplayer is IDialogueBox box)
				{
					// TotalPages = DialogueLines[0].Contents.Count;
					// box.ReadValues(DialogueLines[0].Contents[0].Style, 1, TotalPages);
				}
			}
		}

		public void ParseJson(string path)
		{
			var content = Utils.LoadFromFile(path);
			DialogueLines = JsonSerializer.Deserialize<List<DialogueSerializeable>>(content);
			CurrentPage = 0;
			TotalPages = DialogueLines[0].Contents.Count - 1;
			CurrentDialogue = 0;
			TotalDialogue = DialogueLines[0].Contents.Count;
			CurrentID = DialogueLines[0].ID;
			TargetID = DialogueLines[0].Next;

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

		public void HandleDialogueChange()
		{
			if (CurrentPage == TotalPages)
			{
				//
			}
		}

		public void HandleDialogueType(DialogueSerializeable dialogue)
		{
			//
		}

		public void LoadDialogue()
		{
			//
		}

		public void ChangeDialogue(string target = "")
		{
			var to = target != "" ? DialogueLines.FindIndex(x => x.ID == target) :
			DialogueLines.FindIndex(x => x.ID == TargetID);

			if (to < 0)
				return;

			CurrentDialogue = to;

			var dialogue = DialogueLines[to];

			CurrentPage = 0;
			TotalPages = DialogueLines[to].Contents.Count - 1;
			TotalDialogue = DialogueLines[to].Contents.Count;
			CurrentID = DialogueLines[to].ID;
			TargetID = DialogueLines[to].Next;

			if (dialogue.Type == DialogueType.Talk.ToString())
			{
				HandleTalk(dialogue);
			}
			else if (dialogue.Type == DialogueType.Select.ToString())
			{
				HandleSelect(dialogue);
			}
		}

		public void InstantiateUIElements()
		{
			if (DialogueBox == null || !IsInstanceValid((Node)DialogueBox))
			{
				var instance = TextDisplayer.Instantiate();
				AddChild(instance);
				DialogueBox = instance as IDialogueBox;

				if (SourceType == 0 && Path != null && TextDisplayer != null)
				{
					ParseJson(Path);

					DialogueBox.ReadValues(DialogueLines[0].Contents[0], CurrentPage, TotalPages);
					CurrentID = DialogueLines[0].ID;
				}
			}

			if (DialogueSelection == null || !IsInstanceValid((Node)DialogueSelection))
			{
				var instance = SelectBox.Instantiate();
				AddChild(instance);
				DialogueSelection = (DialogueSelectDefault)instance;
				DialogueSelection.SetVisibility(false);
			}
		}

		public override void _Ready()
		{
			InstantiateUIElements();
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("DialogueTestPrevious"))
			{
				HandlePage(false);
			}
			else if (@event.IsActionPressed("DialogueTestNext"))
			{
				HandlePage(true);
			}
		}

		public void HandlePage(bool next)
		{
			if (next == true && CurrentPage < TotalPages)
			{
				CurrentPage++;
			}
			else if (next == false && CurrentPage > 0)
				CurrentPage--;
			else if (CurrentPage == TotalPages)
				ChangeDialogue();

			// GD.Print($"Pages: {CurrentPage} / {TotalPages}");
			// GD.Print($"Dialogues: {CurrentDialogue} / {TotalDialogue}");
			// GD.Print($"ID: {CurrentID} / {TargetID}");

			DialogueBox.ReadValues(DialogueLines[CurrentDialogue].Contents[CurrentPage], CurrentPage, TotalPages);
		}

		public void HandleTalk(DialogueSerializeable dialogue)
		{
			DialogueBox.SetVisibility(true);
			DialogueSelection.SetVisibility(false);
			DialogueBox.ReadValues(dialogue.Contents[CurrentPage], CurrentPage, TotalPages);
		}

		public void HandleSelect(DialogueSerializeable dialogue)
		{
			DialogueSelection.OptionSelected += OnButtonSignalReceived;
			DialogueBox.SetVisibility(false);
			DialogueSelection.SetVisibility(true);
			DialogueSelection.ReadValues(dialogue.Contents);
		}

		public void OnButtonSignalReceived(string id)
        {
            ChangeDialogue(id);
			DialogueSelection.OptionSelected -= OnButtonSignalReceived;
        }
	}
}